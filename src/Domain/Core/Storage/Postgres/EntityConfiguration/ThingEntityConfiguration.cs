using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class SchemaNames
{
    public const string Thing = "thing";
    public const string Job = "job";
}

internal class ThingEntityConfiguration : IEntityTypeConfiguration<Thing>
{
    public void Configure(EntityTypeBuilder<Thing> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("thing", SchemaNames.Thing);
        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.Detail)
            .HasColumnType("jsonb");

        builder.Property(p => p.Created)
            .HasColumnType("date");

        builder.Property(p => p.Status)
            .IsRequired();
        /*
        builder.OwnsOne(c => c.Source, s =>
        {
            s.Property(p => p.SourceId).HasColumnName("Source_Id");
            s.Property(p => p.ConnectorId).HasColumnName("Source_Connector_Id");
            s.Property(p => p.FullLocation).HasColumnName("Source_FullLocation");
        });*/
        builder.OwnsOne(c => c.Source, s =>
        {
            s.ToJson();
        });

        builder.HasMany(x => x.Connections)
            .WithOne(x => x.From)
            .HasForeignKey(x => x.FromId)
            .IsRequired();

        builder.HasMany(x => x.Data)
            .WithOne()
            .HasForeignKey(x => x.ThingId)
            .IsRequired();

        builder.HasMany(x => x.Identifiers)
            .WithOne()
            .HasForeignKey(x => x.ThingId)
            .IsRequired();

        builder
            .HasMany(p => p.Tags)
            .WithOne()
            .HasForeignKey(p => p.ThingId);

        builder.HasOne(p => p.Class);
    }
}
