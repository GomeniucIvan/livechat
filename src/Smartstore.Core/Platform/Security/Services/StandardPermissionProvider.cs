using Smartstore.Core.Identity;

namespace Smartstore.Core.Security
{
    /// <summary>
    /// Provider to get permissions of the core module.
    /// </summary>
    public partial class StandardPermissionProvider : IPermissionProvider
    {
        public virtual IEnumerable<PermissionRecord> GetPermissions()
        {
            var permissionSystemNames = PermissionHelper.GetPermissions(typeof(Permissions));
            var permissions = permissionSystemNames.Select(x => new PermissionRecord { SystemName = x });

            return permissions;
        }

        public virtual IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Administrators,
                    PermissionRecords = new[]
                    {
                        new PermissionRecord { SystemName = Permissions.Customer.Self },
                        new PermissionRecord { SystemName = Permissions.Cms.Self },
                        new PermissionRecord { SystemName = Permissions.Configuration.Self },
                        new PermissionRecord { SystemName = Permissions.System.Self },
                        new PermissionRecord { SystemName = Permissions.Media.Self }
                    }
                },
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Guests,
                },
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = SystemCustomerRoleNames.Registered,
                    PermissionRecords = new[]
                    {
                        new PermissionRecord { SystemName = Permissions.System.AccessWebsite }
                    }
                }
            };
        }
    }
}
