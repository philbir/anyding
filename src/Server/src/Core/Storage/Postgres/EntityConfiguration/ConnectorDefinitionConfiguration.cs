using System.Text.Json;
using Anyding.Connectors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anyding.Store.EntityConfiguration;

internal class ConnectorDefinitionConfiguration : IEntityTypeConfiguration<ConnectorDefinition>
{
    public void Configure(EntityTypeBuilder<ConnectorDefinition> builder)
    {
        builder.ToTable("connector_definition", SchemaNames.Configuration);
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Type)
            .IsRequired();
        builder.Property(p => p.Name);
        builder.Property(p => p.Priority)
            .IsRequired();

        builder.Property(p => p.Properties)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v,JsonSerializerOptions.Default));

        builder.Property(p => p.Mapping)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<ConnectorMapping>>(v,JsonSerializerOptions.Default));

        builder.HasData(SampleData.ConnectorDefinitions.All);
    }
}
