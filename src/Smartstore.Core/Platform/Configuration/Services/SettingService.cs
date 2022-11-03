using Smartstore.Caching;
using Smartstore.Core.Data;
using Smartstore.Core.Localization;
using Smartstore.Core.Logging;
using Smartstore.Data.Hooks;

namespace Smartstore.Core.Configuration
{
    [Serializable]
    public class CachedSetting
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    [Important]
    public partial class SettingService : AsyncDbSaveHook<Setting>, ISettingService
    {
        // 0 = SettingType
        const string ClassCacheKeyPattern = "settings:{0}";

        // 0 = Setting.Name
        const string RawCacheKeyPattern = "rawsettings:{0}";

        internal readonly static TimeSpan DefaultExpiry = TimeSpan.FromHours(8);

        private readonly SmartDbContext _db;
        private readonly ICacheManager _cache;
        private readonly DbSet<Setting> _setSettings;

        public SettingService(ICacheManager cache, SmartDbContext db)
        {
            _cache = cache;
            _db = db;
            _setSettings = _db.Settings;
        }

        public Localizer T { get; set; } = NullLocalizer.Instance;
        public ILogger Logger { get; set; } = NullLogger.Instance;

        #region Hook

        public override Task<HookResult> OnAfterSaveAsync(IHookedEntity entry, CancellationToken cancelToken)
        {
            // Indicate that we gonna handle this
            return Task.FromResult(HookResult.Ok);
        }

        public override async Task OnAfterSaveCompletedAsync(IEnumerable<IHookedEntity> entries, CancellationToken cancelToken)
        {
            // Obtain distinct prefixes from all changed settings,
            // e.g.: 'catalogsettings.showgtin' > 'catalogsettings'
            var prefixes = entries
                .Select(x => x.Entity)
                .OfType<Setting>()
                .Select(x =>
                {
                    var index = x.Name.LastIndexOf('.');
                    return (index == -1 ? x.Name : x.Name[..index]).ToLowerInvariant();
                })
                .Distinct()
                .ToArray();

            foreach (var prefix in prefixes)
            {
                var numClasses = await _cache.RemoveByPatternAsync(BuildCacheKeyForClassAccess(prefix, "*"));
                var numRaw = await _cache.RemoveByPatternAsync(BuildCacheKeyForRawAccess(prefix, "*"));
            }

            // Log activity.
            var updatedEntities = entries
                .Where(x => x.InitialState == Smartstore.Data.EntityState.Modified)
                .Select(x => x.Entity)
                .OfType<Setting>()
                .ToList();

            if (updatedEntities.Any())
            {
                await _db.SaveChangesAsync(cancelToken);
            }
        }

        #endregion

        #region ISettingService

        /// <inheritdoc/>
        public virtual async Task<bool> SettingExistsAsync(string key)
        {
            Guard.NotEmpty(key, nameof(key));

            return await _setSettings.AnyAsync(x => x.Name == key);
        }

        /// <inheritdoc/>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default, bool doFallback = false)
        {
            Guard.NotEmpty(key, nameof(key));

            var cachedSetting = GetCachedSettingInternal(key, false).Await();

            return cachedSetting.Id > 0
                ? cachedSetting.Value.Convert<T>()
                : defaultValue;
        }

        /// <inheritdoc/>
        public virtual async Task<T> GetSettingByKeyAsync<T>(string key, T defaultValue = default, bool doFallback = false)
        {
            Guard.NotEmpty(key, nameof(key));

            var cachedSetting = await GetCachedSettingInternal(key, true);

            return cachedSetting.Id > 0
                ? cachedSetting.Value.Convert<T>()
                : defaultValue;
        }

        private async Task<CachedSetting> GetCachedSettingInternal(string key, bool async)
        {
            var cacheKey = BuildCacheKeyForRawAccess(key);
            return await _cache.GetAsync(cacheKey, GetEntry, independent: true, allowRecursion: true);

            async Task<CachedSetting> GetEntry(CacheEntryOptions o)
            {
                o.ExpiresIn(DefaultExpiry);

                var setting = async
                    ? await _setSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Name == key)
                    : _setSettings.AsNoTracking().FirstOrDefault(x => x.Name == key);

                return new CachedSetting
                {
                    Id = setting?.Id ?? 0,
                    Value = setting?.Value
                };
            }
        }

        /// <inheritdoc/>
        public virtual async Task<Setting> GetSettingEntityByKeyAsync(string key)
        {
            Guard.NotEmpty(key, nameof(key));

            var query = _setSettings.Where(x => x.Name == key);

            return await query.FirstOrDefaultAsync();
        }

        /// <inheritdoc/>
        public virtual async Task<ApplySettingResult> ApplySettingAsync<T>(string key, T value)
        {
            Guard.NotEmpty(key, nameof(key));

            key = key.ToLowerInvariant();

            var str = value.Convert<string>();
            var setting = await _setSettings.FirstOrDefaultAsync(x => x.Name == key);

            if (setting == null)
            {
                // Insert
                setting = new Setting
                {
                    Name = key,
                    Value = str
                };

                _setSettings.Add(setting);
                return ApplySettingResult.Inserted;
            }
            else
            {
                // Update
                if (setting.Value != str)
                {
                    setting.Value = str;
                    return ApplySettingResult.Modified;
                }
            }

            return ApplySettingResult.Unchanged;
        }

        /// <inheritdoc/>
        public virtual async Task<int> RemoveSettingsAsync(string rootKey)
        {
            if (rootKey.IsEmpty())
                return 0;

            var prefix = rootKey.EnsureEndsWith('.');

            var stubs = await _setSettings
                .AsNoTracking()
                .Where(x => x.Name.StartsWith(rootKey))
                .Select(x => new Setting { Id = x.Id, Name = x.Name})
                .ToListAsync();

            _setSettings.RemoveRange(stubs);

            return stubs.Count;
        }

        /// <inheritdoc/>
        public virtual async Task<bool> RemoveSettingAsync(string key)
        {
            if (key.HasValue())
            {
                key = key.Trim().ToLowerInvariant();

                var setting = await (
                    from s in _setSettings
                    where s.Name == key
                    select s).FirstOrDefaultAsync();

                if (setting != null)
                {
                    _setSettings.Remove(setting);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Utils

        internal static string BuildCacheKeyForClassAccess(Type settingsType)
        {
            return ClassCacheKeyPattern.FormatInvariant(settingsType.Name.ToLowerInvariant());
        }

        internal static string BuildCacheKeyForClassAccess(string prefix, string suffix)
        {
            return ClassCacheKeyPattern.FormatInvariant(prefix.ToLowerInvariant(), suffix);
        }

        internal static string BuildCacheKeyForRawAccess(string prefix)
        {
            return RawCacheKeyPattern.FormatInvariant(prefix.ToLowerInvariant());
        }

        internal static string BuildCacheKeyForRawAccess(string prefix, string suffix)
        {
            return RawCacheKeyPattern.FormatInvariant(prefix.ToLowerInvariant(), suffix);
        }

        #endregion
    }
}