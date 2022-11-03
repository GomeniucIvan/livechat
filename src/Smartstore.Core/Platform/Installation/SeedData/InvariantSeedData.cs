using System.Globalization;
using Autofac;
using Smartstore.Caching.Tasks;
using Smartstore.Core.Common.Tasks;
using Smartstore.Core.Configuration;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Content.Media.Tasks;
using Smartstore.Core.Content.Menus;
using Smartstore.Core.Data;
using Smartstore.Core.Identity;
using Smartstore.Core.Identity.Rules;
using Smartstore.Core.Identity.Tasks;
using Smartstore.Core.Localization;
using Smartstore.Core.Logging;
using Smartstore.Core.Logging.Tasks;
using Smartstore.Core.Messaging;
using Smartstore.Core.Messaging.Tasks;
using Smartstore.Core.Rules;
using Smartstore.Core.Security;
using Smartstore.Core.Seo;
using Smartstore.Core.Stores;
using Smartstore.Scheduling;

namespace Smartstore.Core.Installation
{
    public abstract partial class InvariantSeedData
    {
        private SmartDbContext _db;
        private Language _language;
        private IApplicationContext _appContext;
        private SampleMediaUtility _mediaUtility;

        protected InvariantSeedData()
        {
        }

        public void Initialize(SmartDbContext db, SeedDataConfiguration configuration, IApplicationContext appContext)
        {
            _db = db;
            _language = configuration.Language;
            _appContext = appContext;
            _mediaUtility = new SampleMediaUtility(db, "/App_Data/Samples");
        }

        #region Mandatory data creators

        public IList<MediaFile> Pictures()
        {
            var entities = new List<MediaFile>
            {
                CreatePicture("company-logo.png"),
            };

            Alter(entities);
            return entities;
        }

        public IList<Store> Stores()
        {
            var imgCompanyLogo = _db.MediaFiles.Where(x => x.Name == "company-logo.png").FirstOrDefault();

            var entities = new List<Store>
            {
                new Store
                {
                    Name = "Your store name",
                    Url = "http://www.yourStore.com/",
                    Hosts = "yourstore.com,www.yourstore.com",
                    SslEnabled = false,
                    DisplayOrder = 1,
                    LogoMediaFileId = imgCompanyLogo?.Id ?? 0,
                }
            };

            Alter(entities);
            return entities;
        }

        public IList<CustomerRole> CustomerRoles()
        {
            var entities = new List<CustomerRole>
            {
                new CustomerRole
                {
                    Name = "Administrators",
                    Active = true,
                    IsSystemRole = true,
                    SystemName = SystemCustomerRoleNames.Administrators,
                },
                new CustomerRole
                {
                    Name = "Registered",
                    Active = true,
                    IsSystemRole = true,
                    SystemName = SystemCustomerRoleNames.Registered,
                },
                new CustomerRole
                {
                    Name = "Guests",
                    Active = true,
                    IsSystemRole = true,
                    SystemName = SystemCustomerRoleNames.Guests,
                }
            };

            Alter(entities);
            return entities;
        }

        public Customer SearchEngineUser()
        {
            var entity = new Customer
            {
                Email = "builtin@search-engine-record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            Alter(entity);
            return entity;
        }

        public Customer BackgroundTaskUser()
        {
            var entity = new Customer
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            Alter(entity);
            return entity;
        }

        public Customer PdfConverterUser()
        {
            var entity = new Customer
            {
                Email = "builtin@pdf-converter-record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system record used for the PDF converter.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.PdfConverter,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            Alter(entity);
            return entity;
        }

        public IList<EmailAccount> EmailAccounts()
        {
            var entities = new List<EmailAccount>
            {
                new EmailAccount
                {
                    Email = "test@mail.com",
                    DisplayName = "Store name",
                    Host = "smtp.mail.com",
                    Port = 25,
                    Username = "123",
                    Password = "123",
                    EnableSsl = false,
                    UseDefaultCredentials = false
                }
            };

            Alter(entities);
            return entities;
        }

        public IList<ISettings> Settings()
        {
            var typeScanner = EngineContext.Current.Application.TypeScanner;
            var settings = typeScanner.FindTypes<ISettings>()
                .Select(x => Activator.CreateInstance(x))
                .OfType<ISettings>()
                .ToList();

            var defaultLanguageId = _language.Id;
            var localizationSettings = settings.OfType<LocalizationSettings>().FirstOrDefault();
            if (localizationSettings != null)
            {
                localizationSettings.DefaultAdminLanguageId = defaultLanguageId;
            }

            var defaultEmailAccountId = _db.EmailAccounts.FirstOrDefault()?.Id ?? 0;
            var emailAccountSettings = settings.OfType<EmailAccountSettings>().FirstOrDefault();
            if (emailAccountSettings != null)
            {
                emailAccountSettings.DefaultEmailAccountId = defaultEmailAccountId;
            }

            Alter(settings);
            return settings;
        }

        public IList<TaskDescriptor> TaskDescriptors()
        {
            var entities = new List<TaskDescriptor>
            {
                new TaskDescriptor
                {
                    Name = "Send emails",
                    CronExpression = "* * * * *", // every Minute
					Type = nameof(QueuedMessagesSendTask),
                    Enabled = true,
                    StopOnError = false,
                    Priority = TaskPriority.High
                },
                new TaskDescriptor
                {
                    Name = "Clear email queue",
                    CronExpression = "0 2 * * *", // At 02:00
					Type = nameof(QueuedMessagesClearTask),
                    Enabled = true,
                    StopOnError = false,
                },
                new TaskDescriptor
                {
                    Name = "Delete guests",
                    CronExpression = "*/10 * * * *", // Every 10 minutes
					Type = nameof(DeleteGuestsTask),
                    Enabled = true,
                    StopOnError = false,
                },
                new TaskDescriptor
                {
                    Name = "Delete logs",
                    CronExpression = "0 1 * * *", // At 01:00
					Type = nameof(DeleteLogsTask),
                    Enabled = true,
                    StopOnError = false,
                },
                new TaskDescriptor
                {
                    Name = "Clear cache",
                    CronExpression = "0 */12 * * *", // Every 12 hours
					Type = nameof(ClearCacheTask),
                    Enabled = false,
                    StopOnError = false,
                },
                new TaskDescriptor
                {
                    Name = "Clear transient uploads",
                    CronExpression = "30 1,13 * * *", // At 01:30 and 13:30
					Type = nameof(TransientMediaClearTask),
                    Enabled = true,
                    StopOnError = false,
                },
                new TaskDescriptor
                {
                    Name = "Cleanup temporary files",
                    CronExpression = "30 3 * * *", // At 03:30
					Type = nameof(TempFileCleanupTask),
                    Enabled = true,
                    StopOnError = false
                },
                new TaskDescriptor
                {
                    Name = "Update assignments of customers to customer roles",
                    CronExpression = "15 2 * * *", // At 02:15
                    Type = nameof(TargetGroupEvaluatorTask),
                    Enabled = true,
                    StopOnError = false
                }
            };

            Alter(entities);
            return entities;
        }

        #endregion

        #region Sample data creators

        #region Alterations

        protected virtual void Alter(IList<CustomerRole> entities)
        {
        }

        protected virtual void Alter(Customer entity)
        {
        }

        protected virtual void Alter(IList<EmailAccount> entities)
        {
        }

        protected virtual void Alter(IList<MessageTemplate> entities)
        {
        }

        protected virtual void Alter(IList<Store> entities)
        {
        }

        protected virtual void Alter(IList<MediaFile> entities)
        {
        }

        protected virtual void Alter(IList<ISettings> settings)
        {
        }

        protected virtual void Alter(IList<EmailAccountSettings> settings)
        {
        }

        protected virtual void Alter(IList<TaskDescriptor> entities)
        {
        }

        protected virtual void Alter(UrlRecord entity)
        {
        }

        #endregion Alterations

        #endregion Sample data creators

        #region Helpers

        protected SmartDbContext DbContext
            => _db;

        protected SampleMediaUtility MediaUtility
            => _mediaUtility;

        public virtual UrlRecord CreateUrlRecordFor<T>(T entity) where T : BaseEntity, ISlugSupported, new()
        {
            string name;

            name = BuildSlug(entity.GetDisplayName()).Truncate(400);

            if (name.HasValue())
            {
                var result = new UrlRecord
                {
                    EntityId = entity.Id,
                    EntityName = entity.GetEntityName(),
                    LanguageId = 0,
                    Slug = name,
                    IsActive = true
                };

                Alter(result);
                return result;
            }

            return null;
        }

        protected MediaFile CreatePicture(string fileName, string seoFileName = null)
        {
            return _mediaUtility.CreateMediaFileAsync(fileName, seoFileName).GetAwaiter().GetResult();
        }

        protected string BuildSlug(string name)
            => SlugUtility.Slugify(name);

        #endregion
    }
}