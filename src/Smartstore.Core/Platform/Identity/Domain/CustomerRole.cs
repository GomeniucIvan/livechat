using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Security;

namespace Smartstore.Core.Identity
{
    internal class CustomerRoleMap : IEntityTypeConfiguration<CustomerRole>
    {
        public void Configure(EntityTypeBuilder<CustomerRole> builder)
        {
 
        }
    }

    /// <summary>
    /// Represents a customer role.
    /// </summary>
    [Index(nameof(Active), Name = "IX_Active")]
    [Index(nameof(IsSystemRole), Name = "IX_IsSystemRole")]
    [Index(nameof(SystemName), Name = "IX_SystemName")]
    [Index(nameof(SystemName), nameof(IsSystemRole), Name = "IX_CustomerRole_SystemName_IsSystemRole")]
    public partial class CustomerRole : EntityWithAttributes
    {
        public CustomerRole()
        {
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private member.", Justification = "Required for EF lazy loading")]
        private CustomerRole(ILazyLoader lazyLoader)
            : base(lazyLoader)
        {
        }

        /// <summary>
        /// Gets or sets the customer role name.
        /// </summary>
        [Required, StringLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer role is system.
        /// </summary>
        public bool IsSystemRole { get; set; }

        /// <summary>
        /// Gets or sets the customer role system name.
        /// </summary>
        [StringLength(255)]
        public string SystemName { get; set; }

        private ICollection<PermissionRoleMapping> _permissionRoleMappings;
        /// <summary>
        /// Gets or sets permission role mappings.
        /// </summary>
        public ICollection<PermissionRoleMapping> PermissionRoleMappings
        {
            get => _permissionRoleMappings ?? LazyLoader.Load(this, ref _permissionRoleMappings) ?? (_permissionRoleMappings ??= new HashSet<PermissionRoleMapping>());
            protected set => _permissionRoleMappings = value;
        }
    }
}
