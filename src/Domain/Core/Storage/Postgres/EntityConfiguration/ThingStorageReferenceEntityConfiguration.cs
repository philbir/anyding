using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ThingStorageReferenceEntityConfiguration : IEntityTypeConfiguration<ThingStorageReference>
{
    public void Configure(EntityTypeBuilder<ThingStorageReference> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing_storage_reference", SchemaNames.Thing);
    }
}
