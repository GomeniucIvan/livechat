using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smartstore.Collections;
using Smartstore.Engine.Builders;
using Smartstore.Engine.Initialization;
using Smartstore.Engine.Modularity;

namespace Smartstore.Engine
{
    public class SmartEngine : IEngine
    {
        const string SmartstoreNamespace = "Smartstore";

        private bool _isStarted;

        public IApplicationContext Application { get; private set; }
        public ScopedServiceContainer Scope { get; set; }
        public bool IsInitialized { get; private set; }

        public bool IsStarted
        {
            get => _isStarted;
            set
            {
                if (_isStarted && value == false)
                {
                    throw new InvalidOperationException($"After the engine has been started the '{nameof(IsStarted)}' property is readonly.");
                }

                _isStarted = value;
            }
        }

        public virtual IEngineStarter Start(IApplicationContext application)
        {
            Guard.NotNull(application, nameof(application));

            Application = application;

            // Set IsInitialized prop after init completes.
            ApplicationInitializerMiddleware.Initialized += (s, e) => IsInitialized = true;

            return new EngineStarter(this);
        }

        class EngineStarter : EngineStarter<SmartEngine>
        {
            public EngineStarter(SmartEngine engine)
                : base(engine)
            {
            }

            protected override IEnumerable<Assembly> ResolveCoreAssemblies()
            {
                var assemblies = new HashSet<Assembly>();
                var nsPrefix = SmartstoreNamespace + '.';

                var libs = DependencyContext.Default.CompileLibraries
                    .Where(x => x.Name == SmartstoreNamespace || x.Name.StartsWith(nsPrefix))
                    .Select(x => new CoreAssembly
                    {
                        Name = x.Name,
                        DependsOn = x.Dependencies
                            .Where(y => y.Name.StartsWith(nsPrefix))
                            .Where(y => !y.Name.StartsWith(nsPrefix + "Data.")) // Exclude data provider projects
                            .Select(y => y.Name)
                            .ToArray()
                    })
                    .ToArray()
                    .SortTopological()
                    .Cast<CoreAssembly>()
                    .ToArray();

                foreach (var lib in libs)
                {
                    try
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                        assemblies.Add(assembly);

                        Engine.Application.Logger.Debug("Assembly '{0}' discovered and loaded.", lib.Name);
                    }
                    catch (Exception ex)
                    {
                        Engine.Application.Logger.Error(ex);
                    }
                }

                return assemblies;
            }

            public override void ConfigureServices(IServiceCollection services)
            {
                base.ConfigureServices(services);

                // Add MVC core services
                var mvcBuilder = services.AddControllersWithViews();

                // React init
                // In production, the React files will be served from this directory
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "ClientApp/build";
                });

                // Configure all modular MVC starters
                foreach (var starter in Starters)
                {
                    // Call modular MVC configurers
                    starter.ConfigureMvc(mvcBuilder, services, Engine.Application);
                }
            }

            public override void ConfigureApplication(IApplicationBuilder app)
            {
                base.ConfigureApplication(app);

                // Configure all modular pipelines
                var pipelineBuilder = new RequestPipelineBuilder { ApplicationBuilder = app, ApplicationContext = Engine.Application };
                foreach (var starter in Starters)
                {
                    starter.BuildPipeline(pipelineBuilder);
                }
                pipelineBuilder.Build(app);

                // Map all modular endpoints
                app.UseEndpoints(endpoints =>
                {
                    var routeBuilder = new EndpointRoutingBuilder { ApplicationBuilder = app, ApplicationContext = Engine.Application, RouteBuilder = endpoints };
                    foreach (var starter in Starters)
                    {
                        starter.MapRoutes(routeBuilder);
                    }
                    routeBuilder.Build(endpoints);
                });

                // React init
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "ClientApp";
 
                    if (Engine.Application.HostEnvironment.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            }

            class CoreAssembly : ITopologicSortable<string>
            {
                public string Name { get; init; }
                public Assembly Assembly { get; init; }
                string ITopologicSortable<string>.Key
                {
                    get => Name;
                }

                public string[] DependsOn { get; init; }
            }
        }
    }
}
