using Smartstore.Core.Identity;
using Smartstore.Core.Localization;

namespace Smartstore.Core
{
    /// <summary>
    /// Work context
    /// </summary>
    public interface IWorkContext
    {
        /// <summary>
        /// Gets or sets the current customer
        /// </summary>
        Customer CurrentCustomer { get; set; }

        /// <summary>
        /// Gets the original customer (in case the current in <see cref="CurrentCustomer"/> is impersonated)
        /// </summary>
        Customer CurrentImpersonator { get; }

        /// <summary>
        /// Get or set current user working language
        /// </summary>
        Language WorkingLanguage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether we're in admin area
        /// </summary>
		bool IsAdminArea { get; set; }
    }
}