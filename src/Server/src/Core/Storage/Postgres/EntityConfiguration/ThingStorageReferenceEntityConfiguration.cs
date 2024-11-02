using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ThingDataReferenceEntityConfiguration : IEntityTypeConfiguration<ThingDataReference>
{
    public void Configure(EntityTypeBuilder<ThingDataReference> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing_data_ref", SchemaNames.Thing);
    }
}
