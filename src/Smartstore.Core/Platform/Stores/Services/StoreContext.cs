using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Smartstore.Caching;
using Smartstore.Core.Data;
using Smartstore.Data.Caching;
using Smartstore.Data.Hooks;
using Smartstore.Utilities;

namespace Smartstore.Core.Stores
{
    [Important]
    public class StoreContext : DbSaveHook<BaseEntity>, IStoreContext
    {
        internal const string OverriddenStoreIdKey = "OverriddenStoreId";
        const string CacheKey = "stores:all";

        private readonly IComponentContext _scope;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICacheFactory _cacheFactory;
        private readonly IDbContextFactory<SmartDbContext> _dbContextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Store _currentStore;

        public StoreContext(
            IHttpContextAccessor httpContextAccessor,
            ICacheFactory cacheFactory,
            IDbContextFactory<SmartDbContext> dbContextFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _cacheFactory = cacheFactory;
            _dbContextFactory = dbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        internal StoreContext(
            IComponentContext scope,
            IHttpContextAccessor httpContextAccessor,
            ICacheFactory cacheFactory,
            IDbContextFactory<SmartDbContext> dbContextFactory,
            IActionContextAccessor actionContextAccessor)
        {
            _scope = scope;
            _cacheFactory = cacheFactory;
            _dbContextFactory = dbContextFactory;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
        }

        #region Hook

        public override HookResult OnAfterSave(IHookedEntity entry)
        {
            if (entry.Entity is Store)
            {
                _cacheFactory.GetHybridCache().Remove(CacheKey);
                return HookResult.Ok;
            }
            else
            {
                return HookResult.Void;
            }
        }

        #endregion

        public virtual Store CurrentStore
        {
            get
            {
                if (_currentStore == null)
                {
                    var cachedStores = GetCachedStores();

                    int? storeOverride = GetRequestStore();
                    if (storeOverride.HasValue)
                    {
                        // The store to be used can be overwritten on request basis (e.g. for theme preview, editing etc.)
                        _currentStore = cachedStores.GetStoreById(storeOverride.Value);
                    }

                    if (_currentStore == null)
                    {
                        // Try to determine the current store by HTTP_HOST
                        var hostName = _httpContextAccessor.HttpContext?.Request?.Host.Value;

                        _currentStore =
                            // Try to resolve the current store by HTTP_HOST
                            cachedStores.GetStoreByHostName(hostName) ??
                            // Then resolve primary store
                            cachedStores.GetPrimaryStore() ??
                            // No way
                            throw new Exception("No store could be loaded.");
                    }
                }

                return _currentStore;
            }
            set => _currentStore = value;
        }

        public StoreEntityCache GetCachedStores()
        {
            return _cacheFactory.GetMemoryCache().Get(CacheKey, (o) =>
            {
                o.ExpiresIn(TimeSpan.FromDays(1));

                using var _ = GetOrCreateDbContext(out var db);

                db.ChangeTracker.LazyLoadingEnabled = false;

                var allStores = db.Stores
                    .AsNoTracking()
                    .AsNoCaching()
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.Name)
                    .ToList();

                return new StoreEntityCache(allStores);
            }, allowRecursion: true);
        }

        public int? GetRequestStore()
        {
            return _actionContextAccessor.ActionContext?.HttpContext?.GetItem<int?>(OverriddenStoreIdKey, forceCreation: false);
        }

        public void SetRequestStore()
        {
            var items = _actionContextAccessor.ActionContext?.HttpContext?.Items;

            if (items != null)
            {
                if (items.ContainsKey(OverriddenStoreIdKey))
                {
                    items.Remove(OverriddenStoreIdKey);
                }

                _currentStore = null;
            }
        }

        private IDisposable GetOrCreateDbContext(out SmartDbContext db)
        {
            db = _scope?.ResolveOptional<SmartDbContext>() ??
                 _httpContextAccessor.HttpContext?.RequestServices?.GetService<SmartDbContext>();

            if (db != null)
            {
                // Don't dispose request scoped main db instance.
                return ActionDisposable.Empty;
            }

            db = _dbContextFactory.CreateDbContext();

            return db;
        }
    }
}
