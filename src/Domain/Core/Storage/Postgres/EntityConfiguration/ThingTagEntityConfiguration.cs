using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ThingTagEntityConfiguration : IEntityTypeConfiguration<ThingTag>
{
    public void Configure(EntityTypeBuilder<ThingTag> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing_tag", SchemaNames.Thing);

        builder.Property(p => p.Detail)
            .HasColumnType("jsonb")
            .IsRequired(false);

        builder.HasOne(p => p.Definition)
            .WithMany()
            .HasForeignKey(p => p.DefinitionId)
            .IsRequired();
    }
}
