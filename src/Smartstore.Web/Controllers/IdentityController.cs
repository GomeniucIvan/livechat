using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization.Routing;
using Smartstore.Core.Security;
using Smartstore.Web.Models.Identity;
using Smartstore.Web.Models.System;

namespace Smartstore.Web.Controllers
{
    public class IdentityController : PublicController
    {
        #region Fields

        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;

        #endregion

        #region Ctor

        public IdentityController(UserManager<Customer> userManager,
            SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #endregion

        [HttpPost]
        //[TypeFilter(typeof(DisplayExternalAuthWidgets))]
        [AllowAnonymous, NeverAuthorize]
        //[ValidateAntiForgeryToken]
        [LocalizedRoute("/api/login")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {
            var customer = await _userManager.FindByEmailAsync(model.Email.TrimSafe());

            var result = await _signInManager.PasswordSignInAsync(customer, model.Password, true, lockoutOnFailure: false);

            var resultModel = new GenericApiModel<int>();

            if (result.Succeeded)
            {
                await Services.EventPublisher.PublishAsync(new CustomerSignedInEvent { Customer = customer });

                resultModel.IsValid = true;
            }
            else
            {
                resultModel.Message = T("App.Login.WrongCredentials");
            }

            return ApiJson(resultModel);
        }
    }
}
