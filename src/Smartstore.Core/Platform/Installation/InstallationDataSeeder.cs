using System.Xml;
using Smartstore.Caching;
using Smartstore.Core.Common;
using Smartstore.Core.Configuration;
using Smartstore.Core.Content.Media.Storage;
using Smartstore.Core.Data;
using Smartstore.Core.Data.Migrations;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;
using Smartstore.Core.Messaging;
using Smartstore.Core.Security;
using Smartstore.Core.Seo;
using Smartstore.Data.Hooks;
using Smartstore.IO;
using Smartstore.Core.Companies.Domain;

namespace Smartstore.Core.Installation
{
    public partial class InstallationDataSeeder : DataSeeder<SmartDbContext>
    {
        private readonly DbMigrator<SmartDbContext> _migrator;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly SeedDataConfiguration _config;
        private readonly InvariantSeedData _data;

        private IXmlResourceManager _xmlResourceManager;
        private int _defaultStoreId;

        public InstallationDataSeeder(
            IApplicationContext appContext,
            DbMigrator<SmartDbContext> migrator,
            IMessageTemplateService messageTemplateService,
            SeedDataConfiguration configuration,
            ILogger logger)
            : base(appContext, logger)
        {
            Guard.NotNull(migrator, nameof(migrator));
            Guard.NotNull(configuration, nameof(configuration));
            Guard.NotNull(configuration.Language, "SeedDataConfiguration.Language");
            Guard.NotNull(configuration.Data, "SeedDataConfiguration.SeedData");

            _migrator = migrator;
            _messageTemplateService = messageTemplateService;
            _config = configuration;
            _data = configuration.Data;
        }

        #region IDataSeeder

        protected override async Task SeedCoreAsync()
        {
            Context.ChangeTracker.AutoDetectChangesEnabled = false;
            Context.MinHookImportance = HookImportance.Essential;

            _config.ProgressMessageCallback("Progress.CreatingRequiredData");

            // Special mandatory (non-visible) settings
            await Context.MigrateSettingsAsync(x =>
            {
                x.Add("Media.Storage.Provider", _config.StoreMediaInDB ? DatabaseMediaStorageProvider.SystemName : FileSystemMediaStorageProvider.SystemName);
            });

            // Initialize seeder AFTER we added "Media.Storage.Provider" key,
            // because SampleMediaUtility depends on it.
            _data.Initialize(Context, _config, EngineContext.Current.Application);

            Populate("PopulatePictures", () => _data.Pictures());
            await PopulateAsync("PopulateStores", PopulateStores);
            await PopulateAsync("InstallLanguages", () => PopulateLanguage(_config.Language));
            await PopulateAsync("PopulateCustomersAndUsers", async () => await PopulateCustomersAndUsers(_config.DefaultUserName, _config.DefaultUserPassword));
            await PopulateAsync("PopulateEmailAccounts", _data.EmailAccounts());
            //await PopulateAsync("PopulateMessageTemplates", PopulateMessageTemplates);
            await PopulateAsync("PopulateSettings", PopulateSettings);
            await PopulateAsync("PopulateCustomersAndUsers", async () => await HashDefaultCustomerPassword(_config.DefaultUserName, _config.DefaultUserPassword));
            await PopulateAsync("PopulateScheduleTasks", _data.TaskDescriptors());
            await PopulateAsync("PopulateLocaleResources", async () => await PopulateLocaleResources(_config.Language));

            // Perf
            Context.DetachEntities<BaseEntity>();
        }

        #endregion

        #region Populate

        private async Task PopulateStores()
        {
            var stores = _data.Stores();
            await SaveRangeAsync(stores);
            _defaultStoreId = stores.First().Id;
        }

        private Task PopulateLanguage(Language primaryLanguage)
        {
            primaryLanguage.Published = true;
            return SaveAsync(primaryLanguage);
        }

        private async Task PopulateLocaleResources(Language language)
        {
            var appDataRoot = EngineContext.Current.Application.AppDataRoot;

            var locDir = appDataRoot.GetDirectory("Localization/App/" + language.LanguageCulture);
            if (!locDir.Exists)
            {
                // Fallback to neutral language folder (de, en etc.)
                locDir = appDataRoot.GetDirectory("Localization/App/" + language.UniqueSeoCode);
            }

            if (!locDir.Exists)
            {
                return;
            }

            // Perf
            Context.DetachEntities<BaseEntity>();

            await SeedPendingLocaleResources(locDir);
        }

        private async Task SeedPendingLocaleResources(IDirectory locDir)
        {
            var fs = locDir.FileSystem;
            var headFile = fs.GetFile(PathUtility.Join(locDir.SubPath, "head.txt"));

            if (!headFile.Exists)
            {
                return;
            }

            var resHead = headFile.ReadAllText().Trim();
            if (resHead.HasValue())
            {
                if (long.TryParse(resHead, out var version))
                {
                    await _migrator.SeedPendingLocaleResourcesAsync(version);
                }
                else
                {
                    throw new ArgumentException("Wrong head value (head.txt) for seeding pending locale resources."
                        + $" Must be a migration version number of type 'long'. See {nameof(MigrationHistory.Version)}.");
                }
            }
        }

        private async Task PopulateCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var customerRoles = _data.CustomerRoles();
            await SaveRangeAsync(customerRoles.Where(x => x != null));

            //admin user
            var adminUser = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Password = defaultUserPassword,
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            var adminRole = customerRoles.First(x => x.SystemName == SystemCustomerRoleNames.Administrators);
            var registeredRole = customerRoles.First(x => x.SystemName == SystemCustomerRoleNames.Registered);

            adminUser.CustomerRoleMappings.Add(new CustomerRoleMapping { CustomerId = adminUser.Id, CustomerRoleId = adminRole.Id });
            adminUser.CustomerRoleMappings.Add(new CustomerRoleMapping { CustomerId = adminUser.Id, CustomerRoleId = registeredRole.Id });
            await SaveAsync(adminUser);

            await Context.SaveChangesAsync();

            // Built-in user for search engines (crawlers)
            var guestRole = customerRoles.FirstOrDefault(x => x.SystemName == SystemCustomerRoleNames.Guests);

            var customer = _data.SearchEngineUser();
            customer.CustomerRoleMappings.Add(new CustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
            await SaveAsync(customer);

            // Built-in user for background tasks
            customer = _data.BackgroundTaskUser();
            customer.CustomerRoleMappings.Add(new CustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
            await SaveAsync(customer);

            // Built-in user for the PDF converter
            customer = _data.PdfConverterUser();
            customer.CustomerRoleMappings.Add(new CustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = guestRole.Id });
            await SaveAsync(customer);

            var encryptor = new Encryptor(new SecuritySettings());
            var saltKey = encryptor.CreateSaltKey(5);

            // Company
            var company = new Company()
            {
                Name = defaultUserEmail,
                HashSalt = saltKey,
                CreatedOnUtc = DateTime.UtcNow,
                Hash = encryptor.CreatePasswordHash("1", saltKey, new CustomerSettings().HashedPasswordFormat) //TODO change and pass id
            };
            await SaveAsync(company);

            //admin company
            var customerCompany = new CompanyCustomer()
            {
                CompanyId = company.Id,
                CustomerId = adminUser.Id
            };
            await SaveAsync(customerCompany);
        }

        private async Task HashDefaultCustomerPassword(string defaultUserEmail, string defaultUserPassword)
        {
            var encryptor = new Encryptor(new SecuritySettings());
            var saltKey = encryptor.CreateSaltKey(5);
            var adminUser = await Context.Customers.FirstOrDefaultAsync(x => x.Email == _config.DefaultUserName);

            adminUser.PasswordSalt = saltKey;
            adminUser.PasswordFormat = PasswordFormat.Hashed;
            adminUser.Password = encryptor.CreatePasswordHash(defaultUserPassword, saltKey, new CustomerSettings().HashedPasswordFormat);

            await Context.SaveChangesAsync();
        }

        private async Task PopulateSettings()
        {
            var settings = _data.Settings();
            foreach (var setting in settings)
            {
                await SettingFactory.SaveSettingsAsync(Context, setting);
            }
        }

        #endregion
    }
}
