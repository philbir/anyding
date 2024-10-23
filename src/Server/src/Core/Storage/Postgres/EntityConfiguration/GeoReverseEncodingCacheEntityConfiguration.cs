using Anyding.Geo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class GeoReverseEncodingCacheEntityConfiguration : IEntityTypeConfiguration<GeoReverseEncodingCache>
{
    public void Configure(EntityTypeBuilder<GeoReverseEncodingCache> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("geo_reverse_encoding_cache", SchemaNames.Thing);
        builder.Property(p => p.Created)
            .IsRequired();
        builder.Property(p => p.Source)
            .IsRequired();

        builder.Property(p => p.GeoCoding)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(p => p.Raw)
            .HasColumnType("jsonb")
            .IsRequired();
    }
}
