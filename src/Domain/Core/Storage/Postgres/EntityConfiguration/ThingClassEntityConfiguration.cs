using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ThingClassEntityConfiguration : IEntityTypeConfiguration<ThingClass>
{
    public void Configure(EntityTypeBuilder<ThingClass> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing_class", SchemaNames.Thing);

        builder.Property(p => p.Name)
            .IsRequired();

        builder.HasData(SampleData.Classes.All);
    }
}
