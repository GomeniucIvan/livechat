namespace Smartstore.Core.Localization
{
    /// <summary>
    /// Language service interface
    /// </summary>
    public partial interface ILanguageService
    {
        /// <summary>
        /// Gets all (cached) languages.
        /// </summary>
        /// <param name="includeHidden">A value indicating whether to include hidden records</param>
        /// <param name="tracked">Whether to put entities to EF change tracker.</param>
        /// <returns>Language collection</returns>
        List<Language> GetAllLanguages(bool includeHidden = false, bool tracked = false);

        /// <summary>
        /// Gets all (cached) languages.
        /// </summary>
        /// <param name="includeHidden">A value indicating whether to include hidden records</param>
        /// <param name="tracked">Whether to put entities to EF change tracker.</param>
        /// <returns>Language collection</returns>
        Task<List<Language>> GetAllLanguagesAsync(bool includeHidden = false, bool tracked = false);

        /// <summary>
        /// Determines whether a language is active/published
        /// </summary>
        /// <param name="languageId">The id of the language to check</param>
        /// <returns><c>true</c> when the language is published, <c>false</c> otherwise</returns>
        bool IsPublishedLanguage(int languageId);

        /// <summary>
        /// Determines whether a language is active/published
        /// </summary>
        /// <param name="languageId">The id of the language to check</param>
        /// <returns><c>true</c> when the language is published, <c>false</c> otherwise</returns>
        Task<bool> IsPublishedLanguageAsync(int languageId);

        /// <summary>
        /// Determines whether a language is active/published
        /// </summary>
        /// <param name="seoCode">The SEO code of the language to check</param>
        /// <returns><c>true</c> when the language is published, <c>false</c> otherwise</returns>
        bool IsPublishedLanguage(string seoCode);

        /// <summary>
        /// Determines whether a language is active/published
        /// </summary>
        /// <param name="seoCode">The SEO code of the language to check</param>
        /// <returns><c>true</c> when the language is published, <c>false</c> otherwise</returns>
        Task<bool> IsPublishedLanguageAsync(string seoCode);

        /// <summary>
        /// Gets the seo code of the master (first) active language
        /// </summary>
        /// <returns>The seo code</returns>
        string GetMasterLanguageSeoCode();

        /// <summary>
        /// Gets the seo code of the master (first) active language
        /// </summary>
        /// <returns>The seo code</returns>
        Task<string> GetMasterLanguageSeoCodeAsync();

        /// <summary>
        /// Gets the id of the master (first) active language
        /// </summary>
        /// <returns>The language id</returns>
        int GetMasterLanguageId();

        /// <summary>
        /// Gets the id of the master (first) active language
        /// </summary>
        /// <returns>The language id</returns>
        Task<int> GetMasterLanguageIdAsync();
    }
}