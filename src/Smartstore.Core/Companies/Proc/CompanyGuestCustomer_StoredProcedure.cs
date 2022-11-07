using Smartstore.Core.Companies.Domain;
using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Data;
using Smartstore.Core.Data.Bootstrapping;

namespace Smartstore.Core.Companies.Proc;

public static class CompanyGuestCustomer_StoredProcedure
{
    public static CompanyGuestCustomerDto CompanyGuestCustomer_CreateAndOrGetDetails(this SmartDbContext db,
        int companyId,
        string uniqueId,
        string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            guid = Guid.NewGuid().ToString();
        }

        var pCompanyIdDbParameter = db.DataProvider.CreateIntParameter("CompanyId", companyId);
        var pUniqueIdParameter = db.DataProvider.CreateParameter("UniqueId", uniqueId);
        var pGuidParameter = db.DataProvider.CreateParameter("Guid", guid);

        return db.ExecStoreProcedure<CompanyGuestCustomerDto>($"{nameof(CompanyGuestCustomer)}_CreateAndOrGetDetails",
            pCompanyIdDbParameter,
            pUniqueIdParameter,
            pGuidParameter).FirstOrDefault();
    }
}