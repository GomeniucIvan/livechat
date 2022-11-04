using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;

namespace Smartstore.Core.Companies.Domain
{
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
        [StringLength(500)]
        public string HashSalt { get; set; }

        /// <summary>
        /// Gets or sets the hash
        /// </summary>
        [StringLength(500)]
        [JsonIgnore]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }
    }
}
