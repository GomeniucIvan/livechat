using Smartstore.Engine.Builders;

namespace Smartstore.Web.Infrastructure
{
    internal class HubStarter : StarterBase
    {
        public override void BuildPipeline(RequestPipelineBuilder builder)
        {
            if (builder.ApplicationContext.IsInstalled)
            {
                // Run as last bundling middleware
                builder.Configure(StarterOrdering.LastMiddleware, app =>
                {
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<ChatHub>("/chatHub");
                    });
                });
            }
        }

        public override void ConfigureServices(IServiceCollection services, IApplicationContext appContext)
        {
            services.AddSignalR();

            //services.AddCors(o => {
            //    o.AddPolicy("Limited", builder => builder
            //        .AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //    );
            //});
        }
    }
}
