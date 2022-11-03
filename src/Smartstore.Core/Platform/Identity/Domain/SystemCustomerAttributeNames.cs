namespace Smartstore.Core.Identity
{
    public static partial class SystemCustomerAttributeNames
    {
        // Form fields
        public static string Phone => "Phone";

        // Other attributes
        public static string AvatarPictureId => "AvatarPictureId";
        public static string PasswordRecoveryToken => "PasswordRecoveryToken";
        public static string AccountActivationToken => "AccountActivationToken";
        public static string LastVisitedPage => "LastVisitedPage";
        public static string ImpersonatedCustomerId => "ImpersonatedCustomerId";
        public static string AdminAreaStoreScopeConfiguration => "AdminAreaStoreScopeConfiguration";
        public static string HasConsentedToGdpr => "HasConsentedToGdpr";
        public static string ClientIdent => "ClientIdent";

        // Depends on store
        public static string LanguageId => "LanguageId";
    }
}