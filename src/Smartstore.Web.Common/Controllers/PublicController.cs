using Smartstore.Core.Companies.Dtos;
using Smartstore.Core.Companies.Proc;
using System.Linq;

namespace Smartstore.Web.Controllers
{
    //[Authorize]
    //[AuthorizeAccess]
    //[TrackActivity(Order = 100)]
    [SaveChanges(typeof(SmartDbContext), Order = int.MaxValue)]
    public class PublicController : SmartController
    {
    }

    public class ApiPublicController : SmartController
    {
        protected StoredCompanyDataDto LauncherHeaderData()
        {
            var httpContext = HttpContext;
            var httpRequest = httpContext.Request;
            var guestUniqueIdSet = httpRequest.Headers.TryGetValue("CustomerUniqueId", out var customerUniqueId);
            var guestGuidIdSet = httpRequest.Headers.TryGetValue("GuestGuid", out var guestGuid);
            var companyIdSet = httpRequest.Headers.TryGetValue("CompanyId", out var companyIdString);
            var companyHashSet = httpRequest.Headers.TryGetValue("CompanyHash", out var companyHash);

            if (companyIdSet && companyHashSet)
            {
                var companyIdIsValid = int.TryParse(companyIdString, out int companyId);
                if (companyIdIsValid && companyId > 0)
                {
                    var company = Db.Company_GetDetails(companyId: companyId);
                    if (company != null)
                    {
                        if (string.Equals(companyHash, company.Hash, StringComparison.CurrentCultureIgnoreCase))
                        {
                            //todo change to start chat->
                            var companyGuest = Db.CompanyGuestCustomer_CreateAndOrGetDetails(companyId: company.Id,
                                uniqueId: customerUniqueId,
                                guid: guestGuid);

                            if (companyGuest != null)
                            {
                                return new StoredCompanyDataDto()
                                {
                                    CompanyGuestCompany = companyGuest,
                                    Company = company,
                                };  
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
