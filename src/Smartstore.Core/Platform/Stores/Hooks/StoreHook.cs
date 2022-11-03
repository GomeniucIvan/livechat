using Smartstore.Core.Data;
using Smartstore.Data.Hooks;

namespace Smartstore.Core.Stores
{
    internal class StoreHook : AsyncDbSaveHook<Store>
    {
        private readonly SmartDbContext _db;

        public StoreHook(SmartDbContext db)
        {
            _db = db;
        }

        protected override Task<HookResult> OnDeletedAsync(Store entity, IHookedEntity entry, CancellationToken cancelToken)
            => Task.FromResult(HookResult.Ok);

        public override async Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
           
        }
    }
}
