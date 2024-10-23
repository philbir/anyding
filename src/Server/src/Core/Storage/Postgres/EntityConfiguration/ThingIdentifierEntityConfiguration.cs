using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ThingIdentifierEntityConfiguration : IEntityTypeConfiguration<ThingIdentifier>
{
    public void Configure(EntityTypeBuilder<ThingIdentifier> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing_identifier", SchemaNames.Thing);
    }
}
