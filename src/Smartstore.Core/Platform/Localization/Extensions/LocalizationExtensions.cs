using System.Runtime.CompilerServices;
using Smartstore.Core.Configuration;

namespace Smartstore.Core.Localization
{
    public static partial class LocalizationExtensions
    {
        #region Entity

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="detectEmptyHtml">When <c>true</c>, additionally checks whether the localized value contains empty HTML only and falls back to the default value if so.</param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<string> GetLocalized<T>(this T entity, Expression<Func<T, string>> keySelector, bool detectEmptyHtml = false)
            where T : class, ILocalizedEntity
        {
            var invoker = keySelector.CompileFast();
            return EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>()?.GetLocalizedValue(
                entity,
                entity.Id,
                entity.GetEntityName(),
                invoker.Property.Name,
                (Func<T, string>)invoker,
                null,
                detectEmptyHtml: detectEmptyHtml) ?? new LocalizedValue<string>(invoker.Invoke(entity));
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <param name="detectEmptyHtml">When <c>true</c>, additionally checks whether the localized value contains empty HTML only and falls back to the default value if so.</param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<string> GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector,
            int languageId,
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where T : class, ILocalizedEntity
        {
            var invoker = keySelector.CompileFast();
            return EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>()?.GetLocalizedValue<T, string>(
                entity,
                entity.Id,
                entity.GetEntityName(),
                invoker.Property.Name,
                invoker,
                languageId,
                returnDefaultValue,
                ensureTwoPublishedLanguages,
                detectEmptyHtml) ?? new LocalizedValue<string>(invoker.Invoke(entity));
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="localeKey">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <param name="detectEmptyHtml">When <c>true</c>, additionally checks whether the localized value contains empty HTML only and falls back to the default value if so.</param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<TProp> GetLocalized<T, TProp>(this T entity,
            string localeKey,
            TProp fallback,
            object requestLanguageIdOrObj, // Id or Language
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where T : class, ILocalizedEntity
        {
            return EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>()?.GetLocalizedValue<T, TProp>(
                entity,
                entity.Id,
                entity.GetEntityName(),
                localeKey,
                x => fallback,
                requestLanguageIdOrObj,
                returnDefaultValue,
                ensureTwoPublishedLanguages,
                detectEmptyHtml) ?? new LocalizedValue<TProp>(fallback);
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="language">Language</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <param name="detectEmptyHtml">When <c>true</c>, additionally checks whether the localized value contains empty HTML only and falls back to the default value if so.</param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<string> GetLocalized<T>(this T entity,
            Expression<Func<T, string>> keySelector,
            Language language,
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where T : class, ILocalizedEntity
        {
            var invoker = keySelector.CompileFast();
            return EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>()?.GetLocalizedValue<T, string>(
                entity,
                entity.Id,
                entity.GetEntityName(),
                invoker.Property.Name,
                invoker,
                language,
                returnDefaultValue,
                ensureTwoPublishedLanguages,
                detectEmptyHtml) ?? new LocalizedValue<string>(invoker.Invoke(entity));
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <param name="detectEmptyHtml">When <c>true</c>, additionally checks whether the localized value contains empty HTML only and falls back to the default value if so.</param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<TProp> GetLocalized<T, TProp>(this T entity,
            Expression<Func<T, TProp>> keySelector,
            int languageId,
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where T : class, ILocalizedEntity
        {
            var invoker = keySelector.CompileFast();
            return EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>()?.GetLocalizedValue(
                entity,
                entity.Id,
                entity.GetEntityName(),
                invoker.Property.Name,
                (Func<T, TProp>)invoker,
                languageId,
                returnDefaultValue,
                ensureTwoPublishedLanguages,
                detectEmptyHtml) ?? new LocalizedValue<TProp>(invoker.Invoke(entity));
        }

        /// <summary>
        /// Get localized property of an entity
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <typeparam name="TProp">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="language">Language</param>
        /// <param name="returnDefaultValue">A value indicating whether to return default value (if localized is not found)</param>
        /// <param name="ensureTwoPublishedLanguages">A value indicating whether to ensure that we have at least two published languages; otherwise, load only default value</param>
        /// <param name="detectEmptyHtml">When <c>true</c>, additionally checks whether the localized value contains empty HTML only and falls back to the default value if so.</param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<TProp> GetLocalized<T, TProp>(this T entity,
            Expression<Func<T, TProp>> keySelector,
            Language language,
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where T : class, ILocalizedEntity
        {
            var invoker = keySelector.CompileFast();
            return EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>()?.GetLocalizedValue(
                entity,
                entity.Id,
                entity.GetEntityName(),
                invoker.Property.Name,
                (Func<T, TProp>)invoker,
                language,
                returnDefaultValue,
                ensureTwoPublishedLanguages,
                detectEmptyHtml) ?? new LocalizedValue<TProp>(invoker.Invoke(entity));
        }

        #endregion

        #region Settings

        /// <summary>
        /// Get localized property of an <see cref="ISettings"/> implementation
        /// </summary>
        /// <param name="settings">The settings instance</param>
        /// <param name="keySelector">Key selector</param>
        /// <returns>Localized property</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LocalizedValue<string> GetLocalizedSetting<TSetting>(this TSetting settings,
            Expression<Func<TSetting, string>> keySelector,
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where TSetting : class, ISettings
        {
            return GetLocalizedSetting(settings, keySelector, null, returnDefaultValue, ensureTwoPublishedLanguages, detectEmptyHtml);
        }

        /// <summary>
        /// Get localized property of an <see cref="ISettings"/> implementation
        /// </summary>
        /// <param name="settings">The settings instance</param>
        /// <param name="keySelector">Key selector</param>
        /// <param name="requestLanguageIdOrObj">Language id, <see cref="Language"/> object instance or <c>null</c></param>
        /// <returns>Localized property</returns>
        public static LocalizedValue<string> GetLocalizedSetting<TSetting>(this TSetting settings,
            Expression<Func<TSetting, string>> keySelector,
            object requestLanguageIdOrObj, // Id or Language
            bool returnDefaultValue = true,
            bool ensureTwoPublishedLanguages = true,
            bool detectEmptyHtml = false)
            where TSetting : class, ISettings
        {
            var helper = EngineContext.Current.Scope.ResolveOptional<LocalizedEntityHelper>();
            var invoker = keySelector.CompileFast();

            if (helper == null)
            {
                return new LocalizedValue<string>(invoker.Invoke(settings));
            }

            // Make fallback only when storeId is 0 and the paramter says so.
            var localizedValue = GetValue(returnDefaultValue);


            return localizedValue;

            LocalizedValue<string> GetValue(bool doFallback)
            {
                return helper.GetLocalizedValue(
                    settings,
                    0,
                    typeof(TSetting).Name,
                    invoker.Property.Name,
                    (Func<TSetting, string>)invoker,
                    requestLanguageIdOrObj,
                    doFallback,
                    ensureTwoPublishedLanguages,
                    detectEmptyHtml);
            }
        }

        #endregion

        #region Enums

        /// <summary>
        /// Gets the localized value of an enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="enumValue">Enum value.</param>
        /// <param name="languageId">Language identifier.</param>
        /// <param name="hint">A value indicating whether to load the hint.</param>
        /// <returns>Localized value of an enum.</returns>
        public static string GetLocalizedEnum<T>(this T enumValue, int languageId = 0, bool hint = false)
            where T : struct
        {
            return EngineContext.Current.ResolveService<ILocalizationService>()
                .GetLocalizedEnum(enumValue, languageId, hint) ?? enumValue.ToString();
        }

        /// <summary>
        /// Gets the localized value of an enum.
        /// </summary>
        /// <typeparam name="T">Enum type.</typeparam>
        /// <param name="enumValue">Enum value.</param>
        /// <param name="languageId">Language identifier.</param>
        /// <param name="hint">A value indicating whether to load the hint.</param>
        /// <returns>Localized value of an enum.</returns>
        public static async Task<string> GetLocalizedEnumAsync<T>(this T enumValue, int languageId = 0, bool hint = false)
            where T : struct
        {
            return await EngineContext.Current.ResolveService<ILocalizationService>()
                .GetLocalizedEnumAsync(enumValue, languageId, hint) ?? enumValue.ToString();
        }

        #endregion
    }
}
