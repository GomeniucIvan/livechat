using Smartstore.Scheduling;

namespace Smartstore.Core.Identity.Tasks
{
    /// <summary>
    /// A task that periodically deletes guest customers.
    /// </summary>
    public partial class DeleteGuestsTask : ITask
    {
        private readonly ICustomerService _customerService;

        public DeleteGuestsTask(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var registrationTo = DateTime.UtcNow.AddDays(-7);

            await _customerService.DeleteGuestCustomersAsync(null, registrationTo, true, cancelToken);
        }
    }
}
