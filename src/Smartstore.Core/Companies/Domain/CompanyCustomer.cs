using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Smartstore.Core.Identity;

namespace Smartstore.Core.Companies.Domain
{
    internal class CompanyCustomerMap : IEntityTypeConfiguration<CompanyCustomer>
    {
        public void Configure(EntityTypeBuilder<CompanyCustomer> builder)
        {
            // Globally exclude soft-deleted entities from all queries.
            builder.HasQueryFilter(c => !c.Deleted);

            builder.HasOne(c => c.Company)
                .WithMany(c => c.CompanyCustomers)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.NoAction);
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
        [Required]
        public int CustomerId { get; set; }

        //todo
        //private Customer _customerId;
        ///// <summary>
        ///// Gets or sets the customer record.
        ///// </summary>
        //[ForeignKey(nameof(CustomerId))]
        //[JsonIgnore]
        //public Customer Customer
        //{
        //    get => _customerId ?? LazyLoader.Load(this, ref _customerId);
        //    set => _customerId = value;
        //}

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        private Company _company;
        /// <summary>
        /// Gets or sets the company record.
        /// </summary>
        [ForeignKey(nameof(CompanyId))]
        [JsonIgnore]
        public Company Company
        {
            get => _company ?? LazyLoader.Load(this, ref _company);
            set => _company = value;
        }
    }
}
