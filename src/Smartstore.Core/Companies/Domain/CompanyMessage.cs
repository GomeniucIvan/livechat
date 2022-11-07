using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Smartstore.Core.Companies.Domain
{
    public class CompanyMessage : BaseEntity
    {
        public CompanyMessage()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private CompanyMessage(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the company customer identifier.
        /// </summary>
        public int? CompanyCustomerId { get; set; }

        private CompanyCustomer _companyCustomer;
        /// <summary>
        /// Gets or sets the company customer record.
        /// </summary>
        [ForeignKey(nameof(CompanyCustomerId))]
        public CompanyCustomer CompanyCustomer
        {
            get => _companyCustomer ?? LazyLoader.Load(this, ref _companyCustomer);
            set => _companyCustomer = value;
        }

        /// <summary>
        /// Gets or sets the company guest customer identifier.
        /// </summary>
        public int? CompanyGuestCustomerId { get; set; }

        private CompanyGuestCustomer _companyGuestCustomer;
        /// <summary>
        /// Gets or sets the company guest customer record.
        /// </summary>
        [ForeignKey(nameof(CompanyGuestCustomerId))]
        public CompanyGuestCustomer CompanyGuestCustomer
        {
            get => _companyGuestCustomer ?? LazyLoader.Load(this, ref _companyGuestCustomer);
            set => _companyGuestCustomer = value;
        }

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        public int CompanyId { get; set; }

        private Company _company;
        /// <summary>
        /// Gets or sets the company record.
        /// </summary>
        [ForeignKey(nameof(CompanyId))]
        public Company Company
        {
            get => _company ?? LazyLoader.Load(this, ref _company);
            set => _company = value;
        }

        public DateTime CreatedOnUtc { get; set; }
    }
}
