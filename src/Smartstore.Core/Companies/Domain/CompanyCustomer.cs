using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Identity;

namespace Smartstore.Core.Companies.Domain
{
    internal class CompanyCustomerMap : IEntityTypeConfiguration<CompanyCustomer>
    {
        public void Configure(EntityTypeBuilder<CompanyCustomer> builder)
        {
            // Globally exclude soft-deleted entities from all queries.
            builder.HasQueryFilter(c => !c.Deleted);
        }
    }

    public partial class CompanyCustomer : BaseEntity, ISoftDeletable
    {
        public CompanyCustomer()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private CompanyCustomer(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the company user has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public int CustomerId { get; set; }

        private Customer _customerId;
        /// <summary>
        /// Gets or sets the customer record.
        /// </summary>
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer
        {
            get => _customerId ?? LazyLoader.Load(this, ref _customerId);
            set => _customerId = value;
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
    }
}
