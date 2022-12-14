using Smartstore.Core.Companies.Domain;
using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Data;
using Smartstore.Core.Data.Bootstrapping;

namespace Smartstore.Core.Companies.Proc
{
    public static class CompanyMessage_StoredProcedure
    {
        public static IList<CompanyMessageDto> CompanyMessage_GetList(this SmartDbContext db,
            int companyId,
            int? companyGuestCustomerId,
            int? companyCustomerId,
            bool guestCall = false)
        {
            var pCompanyIdDbParameter = db.DataProvider.CreateIntParameter("CompanyId", companyId);
            var pCompanyGuestCustomerIdDbParameter = db.DataProvider.CreateIntParameter("CompanyGuestCustomerId", companyGuestCustomerId);
            var pCompanyCustomerIdDbParameter = db.DataProvider.CreateIntParameter("CompanyCustomerId", companyCustomerId);
            var pGuestCallDbParameter = db.DataProvider.CreateBooleanParameter("GuestCall", guestCall);

            return db.ExecStoreProcedure<CompanyMessageDto>($"{nameof(CompanyMessage)}_GetList",
                pCompanyIdDbParameter,
                pCompanyGuestCustomerIdDbParameter,
                pCompanyCustomerIdDbParameter,
                pGuestCallDbParameter).ToList();
        }

        public static int? CompanyMessage_Insert(this SmartDbContext db,
            CompanyMessageDto model)
        {
            var pCompanyIdDbParameter = db.DataProvider.CreateIntParameter("CompanyId", model.CompanyId);
            var pCompanyCustomerIdDbParameter = db.DataProvider.CreateIntParameter("CompanyCustomerId", model.CompanyCustomerId);
            var pCompanyGuestCustomerIdDbParameter = db.DataProvider.CreateIntParameter("CompanyGuestCustomerId", model.CompanyGuestCustomerId);
            var pMessageDbParameter = db.DataProvider.CreateParameter("Message", model.Message);

            return db.ExecStoreProcedure<int?>($"{nameof(CompanyMessage)}_Insert",
                pCompanyIdDbParameter,
                pCompanyCustomerIdDbParameter,
                pCompanyGuestCustomerIdDbParameter,
                pMessageDbParameter).FirstOrDefault();
        }
    }
}
