namespace Smartstore.Core.Localization
{
    public static partial class LanguageQueryExtensions
    {
        /// <summary>
        /// Applies standard filter/>.
        /// </summary>
        /// <param name="query">Language query.</param>
        /// <param name="includeHidden">Applies filter by <see cref="Language.Published"/>.</param>
        /// <returns>Language query.</returns>
        public static IOrderedQueryable<Language> ApplyStandardFilter(this IQueryable<Language> query, bool includeHidden = false)
        {
            Guard.NotNull(query, nameof(query));

            if (!includeHidden)
            {
                query = query.Where(x => x.Published);
            }

            return query.OrderBy(x => x.DisplayOrder);
        }
    }
}