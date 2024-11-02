using Anyding.Connectors;
using Anyding.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class JobRunConfiguration : IEntityTypeConfiguration<JobRun>
{
    public void Configure(EntityTypeBuilder<JobRun> builder)
    {
        builder.ToTable("job_run", SchemaNames.Job);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Details)
            .HasColumnType("jsonb");
        builder.Property(p => p.CreatedAt)
            .IsRequired();
        builder.Property(p => p.Status)
            .IsRequired();
    }
}
