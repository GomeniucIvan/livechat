using Smartstore.Core.Companies.Proc;

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
        protected StoredCompanyData LauncherHeaderData()
        {
            var httpContext = HttpContext;
            var httpRequest = httpContext.Request;
            var companyCustomerIdSet = httpRequest.Headers.TryGetValue("CompanyCustomerId", out var companyCustomerId);
            var companyHashSet = httpRequest.Headers.TryGetValue("CompanyHash", out var companyHash);
            var encodeHashSet = httpRequest.Headers.TryGetValue("EncodeHash", out var encodeHash);

            return new StoredCompanyData()
            {
                CompanyCustomerId = companyCustomerIdSet ? companyCustomerId : string.Empty,
                CompanyHash = companyHashSet ? companyHash : string.Empty,
                EncodeHash = encodeHashSet ? encodeHash : string.Empty
            };
        }
    }
}
