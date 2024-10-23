using Typesense;

namespace Anyding.Search;

internal class PersonCollectionConfiguration : ITypesenseCollectionConfiguration<PersonIndex>
{
    public void OnConfiguring(ITypesenseCollectionBuilder<PersonIndex> builder)
    {
        builder.WithName("person")
            .AddField(p => p.Id, FieldType.String)
            .AddField(p => p.Name, FieldType.String)
            .AddField(p => p.DateOfBirth, new Field(FieldType.Int64) { Optional = true });
    }
}
