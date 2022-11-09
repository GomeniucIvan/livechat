using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<ChatHub> _hubContext;

        #endregion

        #region Ctor

        public LauncherController(SmartDbContext db, 
            IHubContext<ChatHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
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

                var companyMessageId = _db.CompanyMessage_Insert(messageDto);
                if (companyMessageId.HasValue)
                {
                    messageDto.Id = companyMessageId.GetValueOrDefault();
                    await _hubContext.Clients.All.SendAsync($"company_{messageDto.CompanyId}_new_message", messageDto);
                }

                //todo generic proc response, created int
                return ApiJson(resultModel.Success(messageDto));
            }

            resultModel.IsValid = false;
            return ApiJson(resultModel.Error());
        } 

        #endregion
    }
}
