using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Smartstore.Core.Identity;

namespace Smartstore.Core.Companies.Domain
{
    public class CompanyUserMessage : BaseEntity
    {
        public CompanyUserMessage()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private CompanyUserMessage(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier.
        /// </summary>
        public int CustomerId { get; set; }

        private Customer _customer;
        /// <summary>
        /// Gets or sets the customer record.
        /// </summary>
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer
        {
            get => _customer ?? LazyLoader.Load(this, ref _customer);
            set => _customer = value;
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
