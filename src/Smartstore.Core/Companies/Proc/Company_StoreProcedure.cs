using Smartstore.Core.Companies.Domain;
using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Data;
using Smartstore.Core.Data.Bootstrapping;

namespace Smartstore.Core.Companies.Proc;

public static class Company_StoreProcedure
{
    public static CompanyDto Company_GetDetails(this SmartDbContext db,
        int companyId)
    {
        var pCompanyIdDbParameter = db.DataProvider.CreateIntParameter("CompanyId", companyId);

        return db.ExecStoreProcedure<CompanyDto>($"{nameof(Company)}_GetDetails",
            pCompanyIdDbParameter).FirstOrDefault();
    }
}