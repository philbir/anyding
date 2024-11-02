using System.Text.Json;
using Anyding.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class JobRunStepConfiguration : IEntityTypeConfiguration<JobRunStep>
{
    public void Configure(EntityTypeBuilder<JobRunStep> builder)
    {
        builder.ToTable("job_run_step", SchemaNames.Job);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Description)
            .HasMaxLength(1000);
        builder.Property(p => p.StartedAt)
            .IsRequired();
        builder.Property(p => p.Status)
            .IsRequired();
        builder.Property(p => p.Properties)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default));

        builder.HasOne<JobRun>()
            .WithMany(x => x.Steps)
            .HasForeignKey(x => x.JobRunId);
    }
}
