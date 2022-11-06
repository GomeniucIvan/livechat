using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Smartstore.Core.Companies.Domain
{
    internal class CustomerRoleMap : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
 
        }
    }

    public partial class Company : BaseEntity, ISoftDeletable
    {
        public Company()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private Company(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [Required, StringLength(400)]
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the company salt
        /// </summary>
        [Required, StringLength(250)]
        public string HashSalt { get; set; }

        /// <summary>
        /// Gets or sets the hash
        /// </summary>
        [StringLength(500)]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        private ICollection<CompanyGuestCustomer> _companyGuestCustomers;
        /// <summary>
        /// Gets or sets the entries of company guests.
        /// </summary>
        public ICollection<CompanyGuestCustomer> CompanyGuestCustomers
        {
            get => _companyGuestCustomers ?? LazyLoader.Load(this, ref _companyGuestCustomers) ?? (_companyGuestCustomers ??= new HashSet<CompanyGuestCustomer>());
            protected set => _companyGuestCustomers = value;
        }

        private ICollection<CompanyCustomer> _companyCustomers;
        /// <summary>
        /// Gets or sets the entries of company customers.
        /// </summary>
        public ICollection<CompanyCustomer> CompanyCustomers
        {
            get => _companyCustomers ?? LazyLoader.Load(this, ref _companyCustomers) ?? (_companyCustomers ??= new HashSet<CompanyCustomer>());
            protected set => _companyCustomers = value;
        }
    }
}
