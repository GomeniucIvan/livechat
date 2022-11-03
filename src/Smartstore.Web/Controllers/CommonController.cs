using NuGet.Protocol;
using Smartstore.Core.Localization;
using Smartstore.Core.Localization.Routing;
using Smartstore.Core.Platform.Localization.Proc;
using Smartstore.Core.Seo;
using Smartstore.Web.Models.System;
using System;

namespace Smartstore.Web.Controllers
{
    public class CommonController : PublicController
    {
        #region Fields

        private readonly SeoSettings _seoSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IApplicationContext _appContext;

        #endregion

        #region Ctor

        public CommonController(SeoSettings seoSettings,
            ILocalizationService localizationService,
            IApplicationContext appContext)
        {
            _seoSettings = seoSettings;
            _localizationService = localizationService;
            _appContext = appContext;
        }

        #endregion

        #region Methods

        [LocalizedRoute("/api/startup")]
        public IActionResult Startup()
        {
            var titleRoute = "";
            var defaultTitle = _seoSettings.GetLocalizedSetting(x => x.MetaTitle).Value;

            switch (_seoSettings.PageTitleSeoAdjustment)
            {
                case PageTitleSeoAdjustment.PagenameAfterStorename:
                    titleRoute = "{0}" + _seoSettings.PageTitleSeparator + defaultTitle;
                    break;
                case PageTitleSeoAdjustment.StorenameAfterPagename:
                default:
                    titleRoute = defaultTitle + _seoSettings.PageTitleSeparator + "{0}";
                    break;
            }

            var customer = Services.WorkContext.CurrentCustomer;

            var model = new StartupModel()
            {
                DefaultTitle = defaultTitle,
                RouteTitle = titleRoute,
                IsRegistered = customer.IsRegistered(),
                IsAdmin = customer.IsRegistered() && customer.IsAdmin()
            }; 

            return ApiJson(new GenericApiModel<StartupModel>().Success(model));
        }

        [LocalizedRoute("/api/resources")]
        public async Task<IActionResult> Resources()
        {
            var resources = await _localizationService.GetPublicResourcesAsync();
            return ApiJson(new GenericApiModel<IList<LocaleStringResourceDto>>().Success(resources));
        }

        #endregion
    }
}
