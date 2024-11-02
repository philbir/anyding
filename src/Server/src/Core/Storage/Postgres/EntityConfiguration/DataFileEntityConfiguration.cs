using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class DataFileEntityConfiguration : IEntityTypeConfiguration<DataFile>
{
    public void Configure(EntityTypeBuilder<DataFile> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("data_files", SchemaNames.Thing);

        builder.Property(p => p.Name)
            .IsRequired();

        builder.Property(p => p.Path)
            .IsRequired(false);

        builder.Property(p => p.CreateAt)
            .IsRequired();

        builder.Property(p => p.Data)
            .IsRequired();
    }
}
