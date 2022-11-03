using System.Reflection;
using Autofac;
using Smartstore.Engine;

namespace Smartstore.Events
{
    public class ConsumerResolver : IConsumerResolver
    {
        public virtual IConsumer Resolve(ConsumerDescriptor descriptor)
        {
            return null;
        }

        public virtual object ResolveParameter(ParameterInfo p, IComponentContext c = null)
        {
            return (c ?? EngineContext.Current.Scope.RequestContainer).Resolve(p.ParameterType);
        }
    }
}