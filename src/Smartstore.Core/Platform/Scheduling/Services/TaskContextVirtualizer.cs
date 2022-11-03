using Microsoft.AspNetCore.Http;
using Smartstore.Core;
using Smartstore.Core.Data;
using Smartstore.Core.Identity;

namespace Smartstore.Scheduling
{
    public class TaskContextVirtualizer : ITaskContextVirtualizer
    {
        public const string CurrentCustomerIdParamName = "CurrentCustomerId";

        public async Task VirtualizeAsync(HttpContext httpContext, IDictionary<string, string> taskParameters = null)
        {
            var db = httpContext.RequestServices.GetRequiredService<SmartDbContext>();
            var customerService = httpContext.RequestServices.GetRequiredService<ICustomerService>();
            var workContext = httpContext.RequestServices.GetRequiredService<IWorkContext>();

            // Try virtualize current customer (which is necessary when user manually executes a task).
            Customer customer = null;
            if (taskParameters != null && taskParameters.ContainsKey(CurrentCustomerIdParamName))
            {
                customer = await db.Customers
                    .IncludeCustomerRoles()
                    .FindByIdAsync(taskParameters[CurrentCustomerIdParamName].Convert<int>());
            }

            if (customer == null && !workContext.CurrentCustomer.IsBackgroundTaskAccount())
            {
                // No virtualization: set background task system customer as current customer.
                customer = await customerService.GetCustomerBySystemNameAsync(SystemCustomerNames.BackgroundTask);
            }

            // Set virtual customer.
            if (customer != null)
            {
                workContext.CurrentCustomer = customer;
            }
        }
    }
}