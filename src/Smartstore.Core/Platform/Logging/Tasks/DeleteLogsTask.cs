using Smartstore.Core.Common.Settings;
using Smartstore.Scheduling;

namespace Smartstore.Core.Logging.Tasks
{
    /// <summary>
    /// A task that periodically deletes log entries.
    /// </summary>
    public partial class DeleteLogsTask : ITask
    {
        private readonly IDbLogService _dbLogService;

        public DeleteLogsTask(IDbLogService dbLogService)
        {
            _dbLogService = dbLogService;
        }

        public Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var maxAge = DateTime.UtcNow.AddDays(-30);

            return _dbLogService.ClearLogsAsync(maxAge, LogLevel.Error, cancelToken);
        }
    }
}
