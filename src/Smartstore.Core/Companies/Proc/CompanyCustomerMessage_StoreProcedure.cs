using Smartstore.Core.Companies.Domain;
using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Data;
using Smartstore.Core.Data.Bootstrapping;

namespace Smartstore.Core.Companies.Proc
{
    public static class CompanyCustomerMessage_StoreProcedure
    {
        public static IList<CompanyMessageDto> CompanyUserMessage_GetList(this SmartDbContext db)
        {
            return db.ExecStoreProcedure<CompanyMessageDto>($"{nameof(CompanyMessage)}_GetList").ToList();
        }

        public static IList<bool> CompanyUserMessage_Insert(this SmartDbContext db,
            CompanyMessageDto model)
        {
            return db.ExecStoreProcedure<bool>($"{nameof(CompanyMessage)}_Insert").ToList();
        }
    }
}
