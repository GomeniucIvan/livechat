using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Companies.Proc;
using Smartstore.Core.Data;
using Smartstore.Core.Localization.Routing;
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
        [LocalizedRoute("/api/launcher/messages")]
        public async Task<IActionResult> Messages()
        {
            var headerData = LauncherHeaderData();

            IList<CompanyMessageDto> messages = _db.CompanyMessage_GetList(companyId: headerData.Company.Id,
                companyGuestCustomerId: headerData.CompanyGuestCompany.Id,
                companyCustomerId: null,
                guestCall: true).ToList();

            return ApiJson(new GenericApiModel<IList<CompanyMessageDto>>().Success(messages.ToArray()));
        }

        [HttpPost]
        [LocalizedRoute("/api/launcher/sendText")]
        public async Task<IActionResult> SendText([FromBody]LauncherMessageModel model)
        {
            var resultModel = new GenericApiModel<CompanyMessageDto>();
            if (model != null)
            {
                var headerData = LauncherHeaderData();

                var messageDto = new CompanyMessageDto()
                {
                    Message = model.Message,
                    CompanyGuestCustomerId = headerData.CompanyGuestCompany.Id,
                    CompanyCustomerId = null,
                    CompanyId = headerData.Company.Id,
                    Sent = true
                };

                var responseBool = _db.CompanyMessage_Insert(messageDto);

                //todo generic proc response, created int
                return ApiJson(resultModel.Success(messageDto));
            }

            resultModel.IsValid = false;
            return ApiJson(resultModel.Error());
        } 

        #endregion
    }
}
