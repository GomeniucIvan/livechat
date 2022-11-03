using Smartstore.Core.Common;

namespace Smartstore.Core.Data
{
    public partial class SmartDbContext
    {
        public DbSet<GenericAttribute> GenericAttributes { get; set; }
    }
}
