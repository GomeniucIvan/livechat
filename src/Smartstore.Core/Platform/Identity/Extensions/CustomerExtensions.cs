using System.Runtime.CompilerServices;
using Smartstore.Core.Identity;
using Smartstore.Core.Localization;

namespace Smartstore
{
    public static class CustomerExtensions
    {
        /// <summary>
        /// Gets a value indicating whether customer is in a certain customer role.
        /// </summary>
        /// <param name="roleSystemName">Customer role system name.</param>
        /// <param name="onlyActiveRoles">A value indicating whether we should look only in active customer roles.</param>
        public static bool IsInRole(this Customer customer, string roleSystemName, bool onlyActiveRoles = true)
        {
            Guard.NotNull(customer, nameof(customer));
            Guard.NotEmpty(roleSystemName, nameof(roleSystemName));

            foreach (var mapping in customer.CustomerRoleMappings)
            {
                var role = mapping.CustomerRole;

                if (role.SystemName.EqualsNoCase(roleSystemName))
                {
                    return !onlyActiveRoles || role.Active;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether the customer is a built-in record for background tasks.
        /// </summary>
        public static bool IsBackgroundTaskAccount(this Customer customer)
        {
            Guard.NotNull(customer, nameof(customer));

            if (!customer.IsSystemAccount || customer.SystemName.IsEmpty())
                return false;

            return customer.SystemName.Equals(SystemCustomerNames.BackgroundTask, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets a value indicating whether customer is a search engine.
        /// </summary>
        public static bool IsSearchEngineAccount(this Customer customer)
        {
            Guard.NotNull(customer, nameof(customer));

            if (!customer.IsSystemAccount || customer.SystemName.IsEmpty())
                return false;

            return customer.SystemName.Equals(SystemCustomerNames.SearchEngine, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets a value indicating whether customer is the pdf converter.
        /// </summary>
        public static bool IsPdfConverter(this Customer customer)
        {
            Guard.NotNull(customer, nameof(customer));

            if (!customer.IsSystemAccount || customer.SystemName.IsEmpty())
                return false;

            return customer.SystemName.Equals(SystemCustomerNames.PdfConverter, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Gets a value indicating whether customer is administrator (navigation properties CustomerRoleMappings then CustomerRole are required).
        /// </summary>
        /// <param name="onlyActiveRoles">A value indicating whether we should look only in active customer roles.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAdmin(this Customer customer, bool onlyActiveRoles = true)
        {
            return IsInRole(customer, SystemCustomerRoleNames.Administrators, onlyActiveRoles);
        }

        /// <summary>
        /// Gets a value indicating whether customer is registered (navigation properties CustomerRoleMappings then CustomerRole are required).
        /// </summary>
        /// <param name="onlyActiveRoles">A value indicating whether we should look only in active customer roles.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsRegistered(this Customer customer, bool onlyActiveRoles = true)
        {
            return IsInRole(customer, SystemCustomerRoleNames.Registered, onlyActiveRoles);
        }

        /// <summary>
        /// Gets a value indicating whether customer is guest (navigation properties CustomerRoleMappings then CustomerRole are required).
        /// </summary>
        /// <param name="onlyActiveRoles">A value indicating whether we should look only in active customer roles.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGuest(this Customer customer, bool onlyActiveRoles = true)
        {
            return IsInRole(customer, SystemCustomerRoleNames.Guests, onlyActiveRoles);
        }

        /// <summary>
        /// Gets the customer's full name.
        /// </summary>
        public static string GetFullName(this Customer customer)
        {
            if (customer == null)
                return string.Empty;

            if (customer.FullName.HasValue())
            {
                return customer.FullName;
            }

            return customer.Email;
        }

        /// <summary>
        /// Gets the display name of a customer (full name, user name or email).
        /// </summary>
        /// <returns>Display name of a customer.</returns>
        public static string GetDisplayName(this Customer customer, Localizer T)
        {
            if (customer != null)
            {
                return customer.IsGuest()
                    ? T("Customer.Guest").Value
                    : customer.GetFullName().NullEmpty() ?? customer.FindEmail();
            }

            return null;
        }

        /// <summary>
        /// Formats the customer name.
        /// </summary>
        /// <returns>Formatted customer name.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatUserName(this Customer customer)
        {
            return FormatUserName(customer, false);
        }

        /// <summary>
        /// Formats the customer name.
        /// </summary>
        /// <param name="customer">Customer entity.</param>
        /// <param name="stripTooLong">Whether to strip too long customer name.</param>
        /// <returns>Formatted customer name.</returns>
        public static string FormatUserName(this Customer customer, bool stripTooLong)
        {
            var engine = EngineContext.Current.Scope;

            var userName = FormatUserName(
                customer,
                engine.Resolve<CustomerSettings>(),
                engine.Resolve<Localizer>(),
                stripTooLong);

            return userName;
        }

        /// <summary>
        /// Formats the customer name.
        /// </summary>
        /// <param name="customer">Customer entity.</param>
        /// <param name="customerSettings">Customer settings.</param>
        /// <param name="T">Localizer.</param>
        /// <param name="stripTooLong">Whether to strip too long customer name.</param>
        /// <returns>Formatted customer name.</returns>
        public static string FormatUserName(
            this Customer customer,
            CustomerSettings customerSettings,
            Localizer T,
            bool stripTooLong)
        {
            Guard.NotNull(customerSettings, nameof(customerSettings));
            Guard.NotNull(T, nameof(T));

            if (customer == null)
            {
                return string.Empty;
            }
            if (customer.IsGuest())
            {
                return T("Customer.Guest");
            }

            var result = customer.GetFullName();

            return result;
        }

        /// <summary>
        /// Find any email address of customer.
        /// </summary>
        public static string FindEmail(this Customer customer)
        {
            if (customer != null)
            {
                return customer.Email.NullEmpty() ;
            }

            return null;
        }
    }
}
