using Anyding.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class JobDefinitionConfiguration : IEntityTypeConfiguration<JobDefintion>
{
    public void Configure(EntityTypeBuilder<JobDefintion> builder)
    {
        builder.ToTable("job_definition", SchemaNames.Job);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name)
            .IsRequired();
        builder.Property(p => p.Type)
            .IsRequired();
        builder.Property(p => p.Details)
            .HasColumnType("jsonb");
        builder.OwnsOne<JobSchedule>(p => p.Schedule, builder => builder.ToJson());
    }
}
