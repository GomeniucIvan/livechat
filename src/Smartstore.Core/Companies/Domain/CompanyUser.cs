using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Smartstore.Core.Companies.Domain
{
    internal class CompanyUserMap : IEntityTypeConfiguration<CompanyUser>
    {
        public void Configure(EntityTypeBuilder<CompanyUser> builder)
        {
            // Globally exclude soft-deleted entities from all queries.
            builder.HasQueryFilter(c => !c.Deleted);
        }
    }

    public partial class CompanyUser : BaseEntity, ISoftDeletable
    {
        public CompanyUser()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private CompanyUser(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the company user has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the company identifier.
        /// </summary>
        public int CompanyId { get; set; }

        private Company _company;
        /// <summary>
        /// Gets or sets the company record.
        /// </summary>
        [ForeignKey("CompanyId")]
        public Company Company
        {
            get => _company ?? LazyLoader.Load(this, ref _company);
            set => _company = value;
        }
    }
}
