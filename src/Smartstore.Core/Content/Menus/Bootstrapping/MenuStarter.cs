using Autofac;
using Smartstore.Core.Content.Menus;
using Smartstore.Engine.Builders;

namespace Smartstore.Core.Bootstrapping
{
    internal class MenuStarter : StarterBase
    {
        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext)
        {
            builder.RegisterType<LinkResolver>().As<ILinkResolver>().InstancePerLifetimeScope();
            //builder.RegisterType<DefaultLinkProvider>().As<ILinkProvider>().InstancePerLifetimeScope();
            builder.RegisterType<MenuPublisher>().As<IMenuPublisher>().InstancePerLifetimeScope();
            builder.RegisterType<MenuService>().As<IMenuService>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultBreadcrumb>().As<IBreadcrumb>().InstancePerLifetimeScope();

            var menuResolverTypes = appContext.TypeScanner.FindTypes<IMenuResolver>();
            foreach (var type in menuResolverTypes)
            {
                builder.RegisterType(type).As<IMenuResolver>().PropertiesAutowired(PropertyWiringOptions.None).InstancePerLifetimeScope();
            }

            var menuTypes = appContext.TypeScanner.FindTypes<IMenu>();
            foreach (var type in menuTypes.Where(x => x.IsVisible))
            {
                builder.RegisterType(type).As<IMenu>().PropertiesAutowired(PropertyWiringOptions.None).InstancePerLifetimeScope();
            }
        }
    }
}