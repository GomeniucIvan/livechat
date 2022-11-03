using Microsoft.AspNetCore.Html;
using Smartstore.Core.Localization;
using Smartstore.Core.Seo;

namespace Smartstore.Core.Widgets
{
    public partial class PageAssetBuilder : IPageAssetBuilder
    {
        private readonly SeoSettings _seoSettings;

        private List<string> _titleParts;
        private List<string> _metaDescriptionParts;
        private List<string> _metaKeywordParts;

        public PageAssetBuilder(
            SeoSettings seoSettings)
        {
            _seoSettings = seoSettings;
        }


        public virtual void AddMetaDescriptionParts(IEnumerable<string> parts, bool prepend = false)
            => AddPartsInternal(ref _metaDescriptionParts, parts, prepend);

        public virtual void AddMetaKeywordParts(IEnumerable<string> parts, bool prepend = false)
            => AddPartsInternal(ref _metaKeywordParts, parts, prepend);

        public virtual IHtmlContent GetDocumentTitle(bool addDefaultTitle)
        {
            if (_titleParts == null)
                return HtmlString.Empty;

            var result = string.Empty;
            var currentTitle = string.Join(_seoSettings.PageTitleSeparator, _titleParts.Distinct(StringComparer.CurrentCultureIgnoreCase).Reverse().ToArray());

            if (currentTitle.HasValue())
            {
                if (addDefaultTitle)
                {
                    // Store name + page title
                    switch (_seoSettings.PageTitleSeoAdjustment)
                    {
                        case PageTitleSeoAdjustment.PagenameAfterStorename:
                            result = string.Join(_seoSettings.PageTitleSeparator, _seoSettings.GetLocalizedSetting(x => x.MetaTitle).Value, currentTitle);
                            break;
                        case PageTitleSeoAdjustment.StorenameAfterPagename:
                        default:
                            result = string.Join(_seoSettings.PageTitleSeparator, currentTitle, _seoSettings.GetLocalizedSetting(x => x.MetaTitle).Value);
                            break;
                    }
                }
                else
                {
                    // Page title only
                    result = currentTitle;
                }
            }
            else
            {
                // Store name only
                result = _seoSettings.GetLocalizedSetting(x => x.MetaTitle).Value;
            }

            return new HtmlString(result);
        }

        public virtual IHtmlContent GetMetaDescription()
        {
            string result = null;

            if (_metaDescriptionParts != null)
            {
                result = string.Join(", ", _metaDescriptionParts.Distinct(StringComparer.CurrentCultureIgnoreCase).Reverse().ToArray());
            }

            return new HtmlString(result?.AttributeEncode()?.NullEmpty() ?? _seoSettings.GetLocalizedSetting(x => x.MetaDescription).Value);
        }

        public virtual IHtmlContent GetMetaKeywords()
        {
            string result = null;

            if (_metaKeywordParts != null)
            {
                result = string.Join(", ", _metaKeywordParts.Distinct(StringComparer.CurrentCultureIgnoreCase).Reverse().ToArray());
            }

            return new HtmlString(result?.AttributeEncode()?.NullEmpty() ?? _seoSettings.GetLocalizedSetting(x => x.MetaKeywords).Value);
        }

        #region Utils

        private static void AddPartsInternal<T>(ref List<T> list, IEnumerable<T> partsToAdd, bool prepend = false)
        {
            var parts = (partsToAdd ?? Enumerable.Empty<T>()).Where(IsValidPart);

            if (list == null)
            {
                list = new List<T>(parts);
            }
            else if (parts.Any())
            {
                if (prepend)
                {
                    // Insertion of multiple parts at the beginning
                    // should keep order (and not vice-versa as it was originally)
                    list.InsertRange(0, parts);
                }
                else
                {
                    list.AddRange(parts);
                }
            }
        }

        private static bool IsValidPart<T>(T part)
        {
            return part != null || (part is string str && str.HasValue());
        }

        #endregion
    }
}
