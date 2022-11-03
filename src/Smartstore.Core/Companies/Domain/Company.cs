using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Smartstore.Core.Companies.Domain
{
    public partial class Company : BaseEntity
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
    }
}
