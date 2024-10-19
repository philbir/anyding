using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ThingConnectionEntityConfiguration : IEntityTypeConfiguration<ThingConnection>
{
    public void Configure(EntityTypeBuilder<ThingConnection> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing_connection", SchemaNames.Thing);
        builder.Property(p => p.Type)
            .IsRequired();

        builder.HasOne(p => p.From)
            .WithMany(p => p.Connections)
            .HasForeignKey(p => p.FromId)
            .IsRequired();

        builder.HasOne(p => p.To)
            .WithMany()
            .HasForeignKey(p => p.ToId)
            .IsRequired();

        builder.HasIndex(p => new { p.FromId, p.Type });
    }
}
