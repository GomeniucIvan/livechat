using Autofac;
using Smartstore.Core.Common.Services;
using Smartstore.Core.Web;
using Smartstore.Engine.Builders;

namespace Smartstore.Core.Bootstrapping
{
    internal sealed class CommonStarter : StarterBase
    {
        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext)
        {
            builder.RegisterType<GeoCountryLookup>().As<IGeoCountryLookup>().SingleInstance();
            builder.RegisterType<DateTimeHelper>().As<IDateTimeHelper>().InstancePerLifetimeScope();
            builder.RegisterType<GenericAttributeService>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<DefaultWebHelper>().As<IWebHelper>().InstancePerLifetimeScope();
            builder.RegisterType<UAParserUserAgent>().As<IUserAgent>().InstancePerLifetimeScope();
            builder.RegisterType<PreviewModeCookie>().As<IPreviewModeCookie>().InstancePerLifetimeScope();
        }
    }
}