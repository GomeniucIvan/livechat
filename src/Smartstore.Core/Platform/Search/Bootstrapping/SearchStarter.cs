using Autofac;
using Smartstore.Core.Search.Facets;
using Smartstore.Core.Search.Indexing;
using Smartstore.Engine.Builders;

namespace Smartstore.Core.Bootstrapping
{
    internal class SearchStarter : StarterBase
    {
        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext)
        {
            builder.RegisterType<DefaultIndexManager>().As<IIndexManager>().InstancePerLifetimeScope();
            builder.RegisterType<NullIndexingService>().As<IIndexingService>().SingleInstance();
            builder.RegisterType<NullIndexBacklogService>().As<IIndexBacklogService>().SingleInstance();

            builder.RegisterType<FacetUrlHelperProvider>().As<IFacetUrlHelperProvider>().InstancePerLifetimeScope();

            // Scopes
            builder.RegisterType<DefaultIndexScopeManager>().As<IIndexScopeManager>().InstancePerLifetimeScope();
        }
    }
}
