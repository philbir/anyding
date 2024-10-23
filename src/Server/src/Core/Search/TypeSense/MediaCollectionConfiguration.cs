using Typesense;

namespace Anyding.Search;

internal class MediaCollectionConfiguration : ITypesenseCollectionConfiguration<MediaIndex>
{
    public void OnConfiguring(ITypesenseCollectionBuilder<MediaIndex> builder)
    {
        builder
            .WithName("media")
            .EnableNestedFields()
            .AddField(p => p.Id, FieldType.String)
            .AddField(p => p.Name, FieldType.String)
            .AddField(p => p.DateTaken, FieldType.Object, optional: true)
            .AddField(p => p.Type, FieldType.String, facet: true, optional: false)
            .AddField(p => p.Dimension, FieldType.Object, optional: true)
            .AddField(p => p.Orientation, FieldType.String, optional: true)
            .AddField(p => p.Faces, FieldType.ObjectArray, optional: true)
            .AddField(p => p.Altitude, FieldType.Float, optional: true)
            .AddField(p => p.Country, FieldType.String, optional: true)
            .AddField(p => p.City, FieldType.String, optional: true)
            .AddField(p => p.PlaceName, FieldType.String, optional: true)
            .AddField(p => p.Street, FieldType.String, optional: true)
            .AddField(p => p.CountryCode, FieldType.String, optional: true)
            .AddField(p => p.Tags, FieldType.ObjectArray, optional: true)
            .AddField(p => p.GeoNames, FieldType.ObjectArray, optional: true)
            .AddField(new Field("image", FieldType.Image) { Optional = true, Store = false })
            .AddField(new Field("embedding", FieldType.FloatArray)
            {
                Optional = true,
                Embed = new AutoEmbeddingConfig(["image"], new ModelConfig("ts/clip-vit-b-p32"))
            })
            .AddField(p => p.Location, FieldType.GeoPoint, optional: true);
    }
}
