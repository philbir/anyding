using Anyding.Discovery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class CollectorJobConfiguration : IEntityTypeConfiguration<CollectorJob>
{
    public void Configure(EntityTypeBuilder<CollectorJob> builder)
    {
        builder.ToTable("collector_jobs", SchemaNames.Job);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ConnectorId)
            .IsRequired();
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.Status)
            .IsRequired();
    }
}
