using Smartstore.Core.Configuration;

namespace Smartstore.Core.Identity
{
    public class CustomerSettings : ISettings
    {
        /// <summary>
        /// Default password format for customers
        /// </summary>
        public PasswordFormat DefaultPasswordFormat { get; set; } = PasswordFormat.Hashed;

        /// <summary>
        /// Gets or sets a customer password format (SHA1, MD5) when passwords are hashed.
        /// </summary>
        public string HashedPasswordFormat { get; set; } = "SHA1";

        /// <summary>
        /// Gets or sets a value indicating whether password requires non alphanumeric chars.
        /// </summary>
        public bool PasswordRequireNonAlphanumeric { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether customers location is shown.
        /// </summary>
        public bool ShowCustomersLocation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show customers join date.
        /// </summary>
        public bool ShowCustomersJoinDate { get; set; }

        /// <summary>
        /// Customer name formatting.
        /// </summary>
        public CustomerNameFormat CustomerNameFormat { get; set; } = CustomerNameFormat.ShowFirstName;

        /// <summary>
        /// Gets or sets a value indicating the number of minutes for 'online customers' module.
        /// </summary>
        public int OnlineCustomerMinutes { get; set; } = 20;

        /// <summary>
        /// Gets or sets a value indicating we should store last visited page URL for each customer.
        /// </summary>
        public bool LastVisitedPage { get; set; } = true;
    }
}
