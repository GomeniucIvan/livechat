using System.Diagnostics;
using Smartstore.Caching;
using Smartstore.Core.Data;
using Smartstore.Data;
using Smartstore.Data.Hooks;
using Smartstore.Scheduling;

namespace Smartstore.Core.Identity.Rules
{
    public partial class TargetGroupEvaluatorTask : ITask
    {
        protected readonly SmartDbContext _db;
        protected readonly ICacheManager _cache;

        public TargetGroupEvaluatorTask(
            SmartDbContext db,
            ICacheManager cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var count = 0;
            var numDeleted = 0;
            var numAdded = 0;
            var rolesCount = 0;

            using (var scope = new DbContextScope(_db, autoDetectChanges: false, minHookImportance: HookImportance.Important, deferCommit: true))
            {
                // Delete existing system mappings.
                var deleteQuery = _db.CustomerRoleMappings.Where(x => x.IsSystemMapping);
                if (ctx.Parameters.ContainsKey("CustomerRoleIds"))
                {
                    var roleIds = ctx.Parameters["CustomerRoleIds"].ToIntArray();
                    deleteQuery = deleteQuery.Where(x => roleIds.Contains(x.CustomerRoleId));
                }

                numDeleted = await deleteQuery.BatchDeleteAsync(cancelToken);

                // Insert new customer role mappings.
                var roles = await _db.CustomerRoles
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(x => x.Active)
                    .ToListAsync(cancelToken);
                rolesCount = roles.Count;

                foreach (var role in roles)
                {
                    var ruleSetCustomerIds = new HashSet<int>();

                    await ctx.SetProgressAsync(++count, roles.Count, $"Add customer assignments for role \"{role.SystemName.NaIfEmpty()}\".");

                    // Add mappings.
                    if (ruleSetCustomerIds.Any())
                    {
                        foreach (var chunk in ruleSetCustomerIds.Chunk(500))
                        {
                            if (cancelToken.IsCancellationRequested)
                                return;

                            foreach (var customerId in chunk)
                            {
                                _db.CustomerRoleMappings.Add(new CustomerRoleMapping
                                {
                                    CustomerId = customerId,
                                    CustomerRoleId = role.Id,
                                    IsSystemMapping = true
                                });

                                ++numAdded;
                            }

                            await scope.CommitAsync(cancelToken);
                        }

                        try
                        {
                            scope.DbContext.DetachEntities<CustomerRoleMapping>();
                        }
                        catch
                        {
                        }
                    }
                }
            }

            Debug.WriteLineIf(numDeleted > 0 || numAdded > 0, $"Deleted {numDeleted} and added {numAdded} customer assignments for {rolesCount} roles.");
        }
    }
}
