namespace Smartstore.Core.Configuration
{
    public static class SettingQueryExtensions
    {
        /// <summary>
        /// Applies order by <see cref="Setting.Name"/>
        /// </summary>
        public static IOrderedQueryable<Setting> ApplySorting(this IQueryable<Setting> source)
        {
            return source.OrderBy(x => x.Name);
        }

        /// <summary>
        /// Gets all settings for given type <paramref name="settingsType"/>/>.
        /// Type must implement <see cref="ISettings"/>.
        /// </summary>
        /// Whether any store-neutral settings (Setting.StoreId = 0) should be fetched if store-specific entry does not exist.
        /// </param>
        public static IQueryable<Setting> ApplyClassFilter(this IQueryable<Setting> source, Type settingsType)
        {
            var prefix = settingsType.Name + ".";

            var query = source.Where(x => x.Name.StartsWith(prefix));

            return query;
        }
    }
}
