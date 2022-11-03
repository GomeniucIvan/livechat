using Smartstore.Core.Configuration;
using Smartstore.Core.Identity;
using Smartstore.Data.Hooks;
using Smartstore.Events;

namespace Smartstore.Web
{
    internal partial class WebCacheInvalidator : AsyncDbSaveHook<BaseEntity>, IConsumer
    {
        public override async Task<HookResult> OnAfterSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            if (entry.Entity is CustomerRole)
            {
               
            }
            else if (entry.Entity is Setting setting && entry.InitialState == EntityState.Modified)
            {
             
            }
            else
            {
                return HookResult.Void;
            }

            return HookResult.Ok;
        }

        public override Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
            return Task.CompletedTask;
        }
    }
}
