using Smartstore.Core.Companies.Domain;
using Smartstore.Core.Data;
using Smartstore.Core.Data.Bootstrapping;

namespace Smartstore.Core.Companies.Proc
{
    public static class CompanyUserMessage_StoreProcedure
    {
        public static IList<CompanyUserMessageDto> CompanyUserMessage_GetList(this SmartDbContext db)
        {
            return db.ExecStoreProcedure<CompanyUserMessageDto>($"{nameof(CompanyUserMessage)}_GetList").ToList();
        }

        public static IList<bool> CompanyUserMessage_Insert(this SmartDbContext db,
            CompanyUserMessageDto model)
        {
            return db.ExecStoreProcedure<bool>($"{nameof(CompanyUserMessage)}_Insert").ToList();
        }
    }
}
