using Autofac;
using Smartstore.Core.Messaging;
using Smartstore.Engine.Builders;

namespace Smartstore.Core.Bootstrapping
{
    internal sealed class MessagingStarter : StarterBase
    {
        public override void ConfigureContainer(ContainerBuilder builder, IApplicationContext appContext)
        {
            builder.RegisterType<EmailAccountService>().As<IEmailAccountService>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<MessageFactory>().As<IMessageFactory>().InstancePerLifetimeScope();
            builder.RegisterType<MessageModelProvider>().As<IMessageModelProvider>().InstancePerLifetimeScope();
            builder.RegisterType<MessageModelHelper>().InstancePerLifetimeScope();
            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerLifetimeScope();
        }
    }
}