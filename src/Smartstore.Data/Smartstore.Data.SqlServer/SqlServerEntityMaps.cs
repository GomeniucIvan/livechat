using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smartstore.Core.Localization;

namespace Smartstore.Data.SqlServer
{
    internal class LocalizedPropertyMap : IEntityTypeConfiguration<LocalizedProperty>
    {
        public void Configure(EntityTypeBuilder<LocalizedProperty> builder)
        {
            builder
                .HasIndex(x => x.Id)
                .HasDatabaseName("IX_LocalizedProperty_Key")
                .IncludeProperties(x => new { x.EntityId, x.LocaleKeyGroup, x.LocaleKey });
        }
    }
}
