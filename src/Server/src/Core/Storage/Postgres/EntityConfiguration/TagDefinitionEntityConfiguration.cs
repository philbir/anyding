using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class TagDefinitionEntityConfiguration : IEntityTypeConfiguration<TagDefinition>
{
    public void Configure(EntityTypeBuilder<TagDefinition> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("tag_definition", SchemaNames.Thing);
        builder.Property(p => p.Name)
            .IsRequired();

        builder.HasData(SampleData.Tags.All);
    }
}
