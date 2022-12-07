using NuGet.Protocol;
using Smartstore.Core.Installation;
using Smartstore.Core.Localization.Routing;
using Smartstore.Threading;
using Smartstore.Web.Models.System;

namespace Smartstore.Web.Controllers
{
    public class InstallController : Controller
    {
        private readonly IInstallationService _installService;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IApplicationContext _appContext;
        private readonly IAsyncState _asyncState;

        public InstallController(
            IHostApplicationLifetime hostApplicationLifetime,
            IApplicationContext appContext,
            IAsyncState asyncState)
        {
            _installService = EngineContext.Current.Scope.ResolveOptional<IInstallationService>();
            _hostApplicationLifetime = hostApplicationLifetime;
            _appContext = appContext;
            _asyncState = asyncState;
        }

        private string T(string resourceName)
        {
            return _installService.GetResource(resourceName);
        }

        [LocalizedRoute("/api/install")]
        public async Task<IActionResult> Install(bool noAutoInstall = false)
        {
            var model = new GenericApiModel<InstallationModel>();
            model.IsValid = true;

            if (_appContext.IsInstalled)
            {
                model.Data = new InstallationModel()
                {
                    IsInstalled = _appContext.IsInstalled
                };

                return Ok(model.ToJson());
            }

            model.Data = new InstallationModel()
            {
                //AdminEmail = T("AdminEmailValue")
            };

            return Ok(model.ToJson());
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [LocalizedRoute("/api/install")]
        public async Task<JsonResult> Install([FromBody]InstallationModel model)
        {
            if (!ModelState.IsValid)
            {
                var result = new InstallationResult();
                ModelState.SelectMany(x => x.Value.Errors).Each(x => result.Errors.Add(x.ErrorMessage));
                return Json(result);
            }
            else
            {
                var result = await _installService.InstallAsync(model, HttpContext.RequestServices.AsLifetimeScope());
                return Json(result);
            }
        }

        [HttpPost]
        [LocalizedRoute("/api/progress")]
        public async Task<JsonResult> Progress()
        {
            var progress = await _asyncState.GetAsync<InstallationResult>();
            return Json(progress);
        }

        [HttpPost]
        [LocalizedRoute("/api/finalize")]
        public async Task<IActionResult> Finalize(bool restart)
        {
            await _asyncState.RemoveAsync<InstallationResult>();

            if (restart)
            {
                _hostApplicationLifetime.StopApplication();
            }

            return Json(new { Success = true });
        }

        public IActionResult ChangeLanguage(string language)
        {
            _installService.SaveCurrentLanguage(language);
            return RedirectToAction("Index");
        }

        [IgnoreAntiforgeryToken]
        public IActionResult RestartInstall()
        {
            // Redirect to home page
            return RedirectToAction("Index");
        }
    }
}
