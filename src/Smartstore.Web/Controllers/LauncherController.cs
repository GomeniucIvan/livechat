using Microsoft.AspNetCore.Authorization;
using Smartstore.Core.Companies.Proc;
using Smartstore.Core.Data;
using Smartstore.Core.Localization.Routing;
using Smartstore.Core.Security;
using Smartstore.Web.Models.Laucher;
using Smartstore.Web.Models.System;

namespace Smartstore.Web.Controllers
{
    public class LauncherController : ApiPublicController
    {
        #region Fields

        private readonly SmartDbContext _db;


        #endregion

        #region Ctor

        public LauncherController(SmartDbContext db)
        {
            _db = db;
        }

        #endregion

        #region Methods

        [HttpPost]
        [LocalizedRoute("/api/launcher/sendText")]
        public async Task<IActionResult> SendText([FromBody]LauncherMessageModel model)
        {
            var resultModel = new GenericApiModel<int>();
            if (model != null && !string.IsNullOrEmpty(model.Author))
            {
                var headerData = LauncherHeaderData();

                var responseBool = _db.CompanyUserMessage_Insert(new CompanyUserMessageDto()
                {
                    Message = model.Message,
                    CustomerUserId = headerData.CompanyCustomerId
                });
            }

            resultModel.IsValid = false;
            return ApiJson(resultModel);
        } 

        #endregion
    }
}
