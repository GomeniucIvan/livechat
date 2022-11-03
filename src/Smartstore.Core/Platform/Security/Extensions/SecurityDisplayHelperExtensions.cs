using Smartstore.Core;
using Smartstore.Core.Common.Settings;
using Smartstore.Core.Security;

namespace Smartstore
{
    public static class SecurityDisplayHelperExtensions
    {
        public static bool HoneypotProtectionEnabled(this IDisplayHelper displayHelper)
        {
            return displayHelper.HttpContext.GetItem(nameof(HoneypotProtectionEnabled), () =>
            {
                return displayHelper.Resolve<SecuritySettings>().EnableHoneypotProtection;
            });
        }

        public static bool IsCustomerRegistered(this IDisplayHelper displayHelper)
        {
            return displayHelper.HttpContext.GetItem(nameof(IsCustomerRegistered), () =>
            {
                var customer = displayHelper.Resolve<IWorkContext>().CurrentCustomer;

                return !customer.IsRegistered();
            });
        }
    }
}
