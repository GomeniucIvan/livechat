using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Smartstore.Core.Common;
using Smartstore.Core.Companies.Domain;

namespace Smartstore.Core.Identity
{
    internal class CustomerMap : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            // Globally exclude soft-deleted entities from all queries.
            builder.HasQueryFilter(c => !c.Deleted);
        }
    }

    /// <summary>
    /// Represents a customer
    /// </summary>
    [Index(nameof(Deleted), Name = "IX_Deleted")]
    [Index(nameof(Email), Name = "IX_Customer_Email")]
    [Index(nameof(CustomerGuid), Name = "IX_Customer_CustomerGuid")]
    [Index(nameof(IsSystemAccount), Name = "IX_IsSystemAccount")]
    [Index(nameof(SystemName), Name = "IX_SystemName")]
    [Index(nameof(LastIpAddress), Name = "IX_Customer_LastIpAddress")]
    [Index(nameof(CreatedOnUtc), Name = "IX_Customer_CreatedOn")]
    [Index(nameof(LastActivityDateUtc), Name = "IX_Customer_LastActivity")]
    [Index(nameof(Deleted), nameof(IsSystemAccount), Name = "IX_Customer_Deleted_IsSystemAccount")]
    public partial class Customer : EntityWithAttributes, ISoftDeletable
    {
        public Customer()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private Customer(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        /// <summary>
        /// Gets or sets the customer Guid
        /// </summary>
        public Guid CustomerGuid { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        [StringLength(500)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        [StringLength(500)]
        [JsonIgnore]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        [JsonIgnore]
        public int PasswordFormatId { get; set; }

        /// <summary>
        /// Gets or sets the password format
        /// </summary>
        [NotMapped]
        [JsonIgnore]
        public PasswordFormat PasswordFormat
        {
            get => (PasswordFormat)PasswordFormatId;
            set => PasswordFormatId = (int)value;
        }

        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        [StringLength(500)]
        [JsonIgnore]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        [StringLength(4000)]
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        bool ISoftDeletable.ForceDeletion
        {
            // We don't want to soft-delete ordinary guest customer accounts.
            get => !IsSystemAccount && Email == null;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the customer account is system
        /// </summary>
        public bool IsSystemAccount { get; set; }

        /// <summary>
        /// Gets or sets the customer system name
        /// </summary>
        [StringLength(500)]
        public string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        [StringLength(100)]
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }

        [StringLength(225)]
        public string FirstName { get; set; }

        [StringLength(225)]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public string TimeZoneId { get; set; }

        public string LastUserAgent { get; set; }

        public string LastUserDeviceType { get; set; }

        [NotMapped, JsonIgnore]
        public override CustomerAttributeCollection GenericAttributes
            => new(base.GenericAttributes);

        #region Utils

        private ICollection<CustomerContent> _customerContent;
        /// <summary>
        /// Gets or sets customer generated content.
        /// </summary>
        [JsonIgnore]
        public ICollection<CustomerContent> CustomerContent
        {
            get => _customerContent ?? LazyLoader.Load(this, ref _customerContent) ?? (_customerContent ??= new HashSet<CustomerContent>());
            protected set => _customerContent = value;
        }

        private ICollection<CustomerRoleMapping> _customerRoleMappings;
        /// <summary>
        /// Gets or sets the customer role mappings.
        /// </summary>
        public ICollection<CustomerRoleMapping> CustomerRoleMappings
        {
            get => _customerRoleMappings ?? LazyLoader.Load(this, ref _customerRoleMappings) ?? (_customerRoleMappings ??= new HashSet<CustomerRoleMapping>());
            protected set => _customerRoleMappings = value;
        }

        /// <summary>
        /// Gets a string identifier for the customer's roles by joining all role ids
        /// </summary>
        /// <param name="onlyActiveCustomerRoles"><c>true</c> ignores all inactive roles</param>
        /// <returns>The identifier</returns>
        public string GetRolesIdent(bool onlyActiveCustomerRoles = true)
        {
            return string.Join(',', GetRoleIds(onlyActiveCustomerRoles));
        }

        /// <summary>
        /// Get identifiers of assigned customer roles.
        /// </summary>
        /// <param name="onlyActiveCustomerRoles"><c>true</c> ignores all inactive roles</param>
        /// <returns>Customer role identifiers.</returns>
        public int[] GetRoleIds(bool onlyActiveCustomerRoles = true)
        {
            return CustomerRoleMappings
                .Select(x => x.CustomerRole)
                .Where(x => !onlyActiveCustomerRoles || x.Active)
                .Select(x => x.Id)
                .ToArray();
        }

        #endregion
    }
}