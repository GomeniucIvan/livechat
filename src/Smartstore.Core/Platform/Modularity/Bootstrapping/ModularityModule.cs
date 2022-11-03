using Autofac;
using Autofac.Builder;
using Autofac.Core;
using FluentValidation.Internal;
using Humanizer;
using Smartstore.Core.Content.Media.Storage;
using Smartstore.Core.OutputCache;
using Smartstore.Core.Widgets;
using Smartstore.Domain;
using Smartstore.Engine.Modularity;
using static Smartstore.Core.Security.Permissions.Cms;

namespace Smartstore.Core.Bootstrapping
{
    internal class ModularityModule : Module
    {
        private readonly IApplicationContext _appContext;

        public ModularityModule(IApplicationContext appContext)
        {
            _appContext = appContext;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProviderManager>().As<IProviderManager>().InstancePerLifetimeScope();

            var providerTypes = _appContext.TypeScanner.FindTypes<IProvider>().ToList();

            foreach (var type in providerTypes)
            {

                var groupName = ProviderTypeToKnownGroupName(type);
                var systemName = GetSystemName(type);
                var friendlyName = GetFriendlyName(type);
                var displayOrder = GetDisplayOrder(type);
                var dependentWidgets = GetDependentWidgets(type);
                var resPattern = "Providers" + ".{1}.{0}"; 
                var settingPattern = "Providers" + ".{0}.{1}";
                var isConfigurable = typeof(IConfigurable).IsAssignableFrom(type);
                var isEditable = typeof(IUserEditable).IsAssignableFrom(type);
                var isHidden = GetIsHidden(type);
                //var exportFeature = GetExportFeature(type);

                var registration = builder
                    .RegisterType(type)
                    .Named<IProvider>(systemName)
                    .Keyed<IProvider>(systemName)
                    .InstancePerAttributedLifetime()
                    .PropertiesAutowired(PropertyWiringOptions.None);

                registration.WithMetadata<ProviderMetadata>(m =>
                {
                    m.For(em => em.GroupName, groupName);
                    m.For(em => em.SystemName, systemName);
                    m.For(em => em.ImplType, type);
                    m.For(em => em.ResourceKeyPattern, resPattern);
                    m.For(em => em.SettingKeyPattern, settingPattern);
                    m.For(em => em.FriendlyName, friendlyName.Name);
                    m.For(em => em.Description, friendlyName.Description);
                    m.For(em => em.DisplayOrder, displayOrder);
                    m.For(em => em.DependentWidgets, dependentWidgets);
                    m.For(em => em.IsConfigurable, isConfigurable);
                    m.For(em => em.IsEditable, isEditable);
                    m.For(em => em.IsHidden, isHidden);
                    //m.For(em => em.ExportFeatures, exportFeature);
                });

                RegisterAsSpecificProvider<IOutputCacheProvider>(type, systemName, registration);
                RegisterAsSpecificProvider<IMediaStorageProvider>(type, systemName, registration);
            }
        }

        
        private static void RegisterAsSpecificProvider<T>(Type implType, string systemName, IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registration) where T : IProvider
        {
            if (typeof(T).IsAssignableFrom(implType))
            {
                try
                {
                    registration.As<T>().Named<T>(systemName);
                    registration.WithMetadata<ProviderMetadata>(m =>
                    {
                        m.For(em => em.ProviderType, typeof(T));
                    });
                }
                catch
                {
                }
            }
        }

        private static bool GetIsHidden(Type type)
        {
            var attr = type.GetAttribute<IsHiddenAttribute>(false);
            if (attr != null)
            {
                return attr.IsHidden;
            }

            return false;
        }

        private static string GetSystemName(Type type)
        {
            var attr = type.GetAttribute<SystemNameAttribute>(false);
            if (attr != null)
            {
                return attr.Name;
            }

            return type.FullName;
            //throw Error.Application("The 'SystemNameAttribute' must be applied to a provider type if the provider does not implement 'IModule' (provider type: {0}, plugin: {1})".FormatInvariant(type.FullName, descriptor != null ? descriptor.SystemName : "-"));
        }

        private static string ProviderTypeToKnownGroupName(Type implType)
        {
            if (typeof(IOutputCacheProvider).IsAssignableFrom(implType))
            {
                return "OutputCache";
            }

            return null;
        }

        private static (string Name, string Description) GetFriendlyName(Type type)
        {
            string name = null;
            string description = name;

            var attr = type.GetAttribute<FriendlyNameAttribute>(false);
            if (attr != null)
            {
                name = attr.Name;
                description = attr.Description;
            }
            else
            {
                name = type.Name.Titleize();
                //throw Error.Application("The 'FriendlyNameAttribute' must be applied to a provider type if the provider does not implement 'IPlugin' (provider type: {0}, plugin: {1})".FormatInvariant(type.FullName, descriptor != null ? descriptor.SystemName : "-"));
            }

            return (name, description);
        }

        private static int GetDisplayOrder(Type type)
        {
            var attr = type.GetAttribute<OrderAttribute>(false);
            if (attr != null)
            {
                return attr.Order;
            }

            return 0;
        }

        private string[] GetDependentWidgets(Type type)
        {
            var attr = type.GetAttribute<DependentWidgetsAttribute>(false);
            if (attr != null)
            {
                return attr.WidgetSystemNames;
            }

            return Array.Empty<string>();
        }
    }
}
