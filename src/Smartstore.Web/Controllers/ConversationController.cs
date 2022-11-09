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

        #endregion

        #region Ctor

        public ConversationController(SmartDbContext db)
        {
            _db = db;
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
            var resultModel = new GenericApiModel<int?>();
            if (model != null)
            {
                var responseBool = _db.CompanyMessage_Insert(new CompanyMessageDto()
                {
                    Message = model.Message,
                    CompanyGuestCustomerId = 1,
                    CompanyCustomerId = 1,
                    CompanyId = 1
                });

                //todo generic proc response, created int
                return ApiJson(resultModel.Success(null));
            }

            resultModel.IsValid = false;
            return ApiJson(resultModel.Error());
        } 

        #endregion
    }
}
