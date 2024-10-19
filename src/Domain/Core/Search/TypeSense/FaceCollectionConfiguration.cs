using Typesense;

namespace Anyding.Search;

internal class FaceCollectionConfiguration : ITypesenseCollectionConfiguration<FaceIndex>
{
    public void OnConfiguring(ITypesenseCollectionBuilder<FaceIndex> builder)
    {
        builder.WithName("face")
            .AddField(p => p.Id, FieldType.String)
            .AddField(p => p.MediaId, new Field(FieldType.String) { Reference = "media.id" })
            .AddField(p => p.PersonId, new Field(FieldType.String) { Reference = "person.id", Optional = true})
            .AddField(p => p.AgeInMonth, FieldType.Int32, optional: true)
            .AddField(p => p.State, FieldType.String, optional: true)
            .AddField(p => p.Encoding, new Field(FieldType.FloatArray) { NumberOfDimensions = 128 });
    }
}
