using Smartstore.Core.Logging;

namespace Smartstore.Core.Data
{
    public partial class SmartDbContext
    {
        public DbSet<Log> Logs { get; set; }
    }
}
