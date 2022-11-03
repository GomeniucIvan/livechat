using Smartstore.Core.Common;

namespace Smartstore.Core.Identity
{
    public class CustomerAttributeCollection : GenericAttributeCollection<Customer>
    {
        public CustomerAttributeCollection(GenericAttributeCollection collection)
            : base(collection)
        {
        }

        #region Form fields

        // INFO: this address data is for information purposes only (e.g. on the customer profile page).
        // Usually customer addresses are used for it (e.g. in checkout).

        public string Phone
        {
            get => Get<string>(SystemCustomerAttributeNames.Phone);
            set => Set(SystemCustomerAttributeNames.Phone, value);
        }

        #endregion

        #region Other attributes

        public int? AvatarPictureId
        {
            get => Get<int?>(SystemCustomerAttributeNames.AvatarPictureId);
            set => Set(SystemCustomerAttributeNames.AvatarPictureId, value);
        }

        public string PasswordRecoveryToken
        {
            get => Get<string>(SystemCustomerAttributeNames.PasswordRecoveryToken);
            set => Set(SystemCustomerAttributeNames.PasswordRecoveryToken, value);
        }

        public string AccountActivationToken
        {
            get => Get<string>(SystemCustomerAttributeNames.AccountActivationToken);
            set => Set(SystemCustomerAttributeNames.AccountActivationToken, value);
        }

        public string LastVisitedPage
        {
            get => Get<string>(SystemCustomerAttributeNames.LastVisitedPage);
            set => Set(SystemCustomerAttributeNames.LastVisitedPage, value);
        }

        public int? ImpersonatedCustomerId
        {
            get => Get<int?>(SystemCustomerAttributeNames.ImpersonatedCustomerId);
            set => Set(SystemCustomerAttributeNames.ImpersonatedCustomerId, value);
        }

        public int AdminAreaStoreScopeConfiguration
        {
            get => Get<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            set => Set(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration, value);
        }

        public bool HasConsentedToGdpr
        {
            get => Get<bool>(SystemCustomerAttributeNames.HasConsentedToGdpr);
            set => Set(SystemCustomerAttributeNames.HasConsentedToGdpr, value);
        }

        public string ClientIdent
        {
            get => Get<string>(SystemCustomerAttributeNames.ClientIdent);
            set => Set(SystemCustomerAttributeNames.ClientIdent, value);
        }

        #endregion

        #region Depends on store

        public int? LanguageId
        {
            get => Get<int?>(SystemCustomerAttributeNames.LanguageId);
            set => Set(SystemCustomerAttributeNames.LanguageId, value);
        }

        #endregion
    }
}
