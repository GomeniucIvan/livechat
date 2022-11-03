using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Smartstore.Core.Widgets;
using Smartstore.Utilities;

namespace Smartstore
{
    public static class IViewInvokerExtensions
    {
        #region View

        /// <inheritdoc cref="IViewInvoker.InvokeViewAsync(string, string, ViewDataDictionary)"/>
        /// <param name="model">Model</param>
        public static Task<HtmlString> InvokeViewAsync(this IViewInvoker invoker, string viewName, object model)
        {
            return invoker.InvokeViewAsync(viewName, null, new ViewDataDictionary<object>(invoker.ViewData, model));
        }

        /// <inheritdoc cref="IViewInvoker.InvokeViewAsync(string, string, ViewDataDictionary)"/>
        /// <param name="model">Model</param>
        public static Task<HtmlString> InvokeViewAsync(this IViewInvoker invoker, string viewName, string module, object model)
        {
            return invoker.InvokeViewAsync(viewName, module, new ViewDataDictionary<object>(invoker.ViewData, model));
        }

        /// <inheritdoc cref="IViewInvoker.InvokeViewAsync(string, string, ViewDataDictionary)"/>
        public static Task<HtmlString> InvokeViewAsync(this IViewInvoker invoker, string viewName, ViewDataDictionary viewData)
        {
            return invoker.InvokeViewAsync(viewName, null, viewData);
        }

        /// <inheritdoc cref="IViewInvoker.InvokeViewAsync(string, string, ViewDataDictionary)"/>
        /// <param name="model">Model</param>
        /// <param name="additionalViewData">Additional view data</param>
        public static Task<HtmlString> InvokeViewAsync(this IViewInvoker invoker, string viewName, object model, object additionalViewData)
        {
            var viewData = new ViewDataDictionary<object>(invoker.ViewData, model);

            if (additionalViewData != null)
            {
                viewData.Merge(ConvertUtility.ObjectToDictionary(additionalViewData));

                if (additionalViewData is ViewDataDictionary vdd)
                {
                    viewData.TemplateInfo.HtmlFieldPrefix = vdd.TemplateInfo.HtmlFieldPrefix;
                }
            }

            return invoker.InvokeViewAsync(viewName, null, viewData);
        }

        #endregion

        #region Partial view

        /// <inheritdoc cref="IViewInvoker.InvokePartialViewAsync(string, string, ViewDataDictionary)"/>
        /// <param name="model">Model</param>
        public static Task<HtmlString> InvokePartialViewAsync(this IViewInvoker invoker, string viewName, object model)
        {
            return invoker.InvokePartialViewAsync(viewName, null, new ViewDataDictionary<object>(invoker.ViewData, model));
        }

        /// <inheritdoc cref="IViewInvoker.InvokePartialViewAsync(string, string, ViewDataDictionary)"/>
        /// <param name="model">Model</param>
        public static Task<HtmlString> InvokePartialViewAsync(this IViewInvoker invoker, string viewName, string module, object model)
        {
            return invoker.InvokePartialViewAsync(viewName, module, new ViewDataDictionary<object>(invoker.ViewData, model));
        }

        /// <inheritdoc cref="IViewInvoker.InvokePartialViewAsync(string, string, ViewDataDictionary)"/>
        public static Task<HtmlString> InvokePartialViewAsync(this IViewInvoker invoker, string viewName, ViewDataDictionary viewData)
        {
            return invoker.InvokePartialViewAsync(viewName, null, viewData);
        }

        /// <inheritdoc cref="IViewInvoker.InvokePartialViewAsync(string, string, ViewDataDictionary)"/>
        /// <param name="model">Model</param>
        /// <param name="additionalViewData">Additional view data</param>
        public static Task<HtmlString> InvokePartialViewAsync(this IViewInvoker invoker, string viewName, object model, object additionalViewData)
        {
            var viewData = new ViewDataDictionary<object>(invoker.ViewData, model);

            if (additionalViewData != null)
            {
                viewData.Merge(ConvertUtility.ObjectToDictionary(additionalViewData));

                if (additionalViewData is ViewDataDictionary vdd)
                {
                    viewData.TemplateInfo.HtmlFieldPrefix = vdd.TemplateInfo.HtmlFieldPrefix;
                }
            }

            return invoker.InvokePartialViewAsync(viewName, null, viewData);
        }

        #endregion

        private static async Task<HtmlString> ExecuteCapturedAsync(Func<TextWriter, Task> executor)
        {
            using var psb = StringBuilderPool.Instance.Get(out var sb);
            using (var writer = new StringWriter(sb))
            {
                await executor(writer);
            }

            return new HtmlString(sb.ToString());
        }
    }
}
