using Microsoft.Extensions.Primitives;

namespace Smartstore.Core.Companies.Proc
{
    public class StoredCompanyData
    {
        public string CompanyCustomerId { get; set; }
        public string CompanyHash { get; set; }
        public string EncodeHash { get; set; }
    }

    public class CompanyUserMessageDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string CustomerUserId { get; set; }
    }
}
