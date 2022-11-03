using Microsoft.AspNetCore.Html;
using Smartstore.Core.Seo;

namespace Smartstore.Core.Widgets
{
    public partial interface IPageAssetBuilder
    {
        /// <summary>
        /// Pushes meta description parts to the currently rendered page.
        /// </summary>
        /// <param name="parts">The parts to push.</param>
        /// <param name="prepend"><c>true</c> to insert <paramref name="parts"/> at the beginning of the current parts list.</param>
        void AddMetaDescriptionParts(IEnumerable<string> parts, bool prepend = false);

        /// <summary>
        /// Pushes meta keyword parts to the currently rendered page.
        /// </summary>
        /// <param name="parts">The parts to push.</param>
        /// <param name="prepend"><c>true</c> to insert <paramref name="parts"/> at the beginning of the current parts list.</param>
        void AddMetaKeywordParts(IEnumerable<string> parts, bool prepend = false);

        /// <summary>
        /// Gets the document title which is composed of all current title parts
        /// separated by <see cref="SeoSettings.PageTitleSeparator"/>
        /// </summary>
        /// <param name="addDefaultTitle">
        /// Appends or prepends <see cref="SeoSettings.MetaTitle"/> according to
        /// <see cref="SeoSettings.PageTitleSeoAdjustment"/>. Separates both parts
        /// with <see cref="SeoSettings.PageTitleSeparator"/>.
        /// </param>
        /// <returns>Document title.</returns>
        IHtmlContent GetDocumentTitle(bool addDefaultTitle);

        /// <summary>
        /// Gets the document meta description which is composed of all current description parts separated by ", ".
        /// </summary>
        IHtmlContent GetMetaDescription();

        /// <summary>
        /// Gets the document meta keywords which is composed of all current keyword parts separated by ", ".
        /// </summary>
        IHtmlContent GetMetaKeywords();
    }
}
