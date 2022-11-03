using System.Reflection;
using Autofac;
using Smartstore.Caching;
using Smartstore.Data.Hooks;
using Smartstore.Engine;
using Smartstore.Engine.Modularity;
using Smartstore.Events;

namespace Smartstore.Bootstrapping
{
    public class EventsModule : Autofac.Module
    {
        public readonly static Type[] IgnoredInterfaces = new Type[]
        {
            typeof(IDisposable),
            typeof(IAsyncDisposable),
            typeof(IScopedService)
        };

        private readonly IApplicationContext _appContext;

        public EventsModule(IApplicationContext appContext)
        {
            _appContext = appContext;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<NullMessageBus>()
                .As<IMessageBus>()
                .SingleInstance();

            builder.RegisterType<EventPublisher>()
                .As<IEventPublisher>()
                .SingleInstance();

            builder.RegisterType<ConsumerRegistry>()
                .As<IConsumerRegistry>()
                .SingleInstance();

            builder.RegisterType<ConsumerResolver>()
                .As<IConsumerResolver>()
                .SingleInstance();

            builder.RegisterType<ConsumerInvoker>()
                .As<IConsumerInvoker>()
                .SingleInstance();

            DiscoverConsumers(builder);
        }

        private void DiscoverConsumers(ContainerBuilder builder)
        {

        }
    }
}