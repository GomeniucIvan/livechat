using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Smartstore.Caching;
using Smartstore.Core.Widgets;
using Smartstore.Utilities;

namespace Smartstore.Core.Localization
{
    /// <summary>
    /// Uses culture from current working language and sets globalization clients scripts accordingly.
    /// </summary>
    public class RequestCultureMiddleware
    {
        // DIN 5008.
        private static string[] _deMonthAbbreviations = new[] { "Jan.", "Feb.", "März", "Apr.", "Mai", "Juni", "Juli", "Aug.", "Sept.", "Okt.", "Nov.", "Dez.", "" };

        private readonly RequestDelegate _next;
        private readonly ICacheManager _cache;

        public RequestCultureMiddleware(RequestDelegate next, ICacheManager cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context, IWorkContext workContext)
        {
            var request = context.Request;
            var language = workContext.WorkingLanguage;

            var culture = workContext.CurrentCustomer != null && language != null
                ? new CultureInfo(language.LanguageCulture)
                : new CultureInfo("en-US");

            if (language?.UniqueSeoCode?.EqualsNoCase("de") ?? false)
            {
                culture.DateTimeFormat.AbbreviatedMonthNames = culture.DateTimeFormat.AbbreviatedMonthGenitiveNames = _deMonthAbbreviations;
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            await _next(context);
        }
    }
}