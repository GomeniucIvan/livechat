namespace Smartstore.Core.Identity
{
    public enum PasswordFormat
    {
        Clear = 0,
        Hashed = 1,
        Encrypted = 2
    }

    /// <summary>
    /// Represents the customer name fortatting enumeration.
    /// </summary>
    /// <remarks>
    /// Backward compat: don't singularize enum values.
    /// </remarks>
    public enum CustomerNameFormat
    {
        /// <summary>
        /// Show emails
        /// </summary>
        ShowEmails = 1,

        /// <summary>
        /// Show full names
        /// </summary>
        ShowFullNames = 2,

        /// <summary>
        /// Show first name
        /// </summary>
        ShowFirstName = 3
    }
}
