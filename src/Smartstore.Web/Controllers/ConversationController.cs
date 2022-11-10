using Microsoft.AspNetCore.SignalR;
using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Companies.Proc;
using Smartstore.Core.Data;
using Smartstore.Core.Localization.Routing;
using Smartstore.Web.Models.Conversation;
using Smartstore.Web.Models.System;

namespace Smartstore.Web.Controllers
{
    public class ConversationController : ApiPublicController
    {
        #region Fields

        private readonly SmartDbContext _db;
        private readonly IHubContext<ChatHub> _hubContext;

        #endregion

        #region Ctor

        public ConversationController(SmartDbContext db, 
            IHubContext<ChatHub> hubContext)
        {
            _db = db;
            _hubContext = hubContext;
        }

        #endregion

        #region MyRegion

        [HttpPost]
        [LocalizedRoute("/api/messages")]
        public async Task<IActionResult> Messages()
        {
            // todo change
            IList<CompanyMessageDto> messages = _db.CompanyMessage_GetList(companyId: 1,
                companyGuestCustomerId: 1,
                companyCustomerId: 1).ToList();

            return ApiJson(new GenericApiModel<IList<CompanyMessageDto>>().Success(messages.ToArray()));
        }

        [HttpPost]
        [LocalizedRoute("/api/sendText")]
        public async Task<IActionResult> SendText([FromBody]MessageModel model)
        {
            var resultModel = new GenericApiModel<CompanyMessageDto>();
            if (model != null)
            {
                var messageDto = new CompanyMessageDto()
                {
                    Message = model.Message,
                    CompanyGuestCustomerId = 1,
                    CompanyCustomerId = 1,
                    CompanyId = 1,
                    Sent = false
                };

                var companyMessageId = _db.CompanyMessage_Insert(messageDto);

                if (companyMessageId.HasValue)
                {
                    messageDto.Id = companyMessageId.GetValueOrDefault();
                    await _hubContext.Clients.All.SendAsync($"guest_{messageDto.CompanyId}_new_message", messageDto);
                    await _hubContext.Clients.All.SendAsync($"company_{messageDto.CompanyId}_new_message", messageDto);
                    //todo generic proc response, created int
                    return ApiJson(resultModel.Success(messageDto));
                }

            }

            resultModel.IsValid = false;
            return ApiJson(resultModel.Error());
        } 

        #endregion
    }
}
