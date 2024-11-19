using Anyding.Data;
using Anyding.Search;

namespace Anyding.Api;

[ObjectType<MediaIndex>]
public static partial class MediaType
{
    static partial void Configure(IObjectTypeDescriptor<MediaIndex> descriptor)
    {
        descriptor.Ignore(x => x.Image);
        descriptor.Ignore(x => x.Tags);
    }

    public static async Task<string> GetPreviewAsync(
        [Parent] MediaIndex media,
        IPreviewDataByIdDataLoader proviewDataByIdDataLoader,
        string name,
        CancellationToken ct)
    {
        byte[]? data = await proviewDataByIdDataLoader.WithName(name).LoadAsync(Guid.Parse(media.Id), ct);

        return data.ToDataUrl("webp");
    }
}

[ObjectType<FaceIndex>]
public static partial class FaceType
{
    static partial void Configure(IObjectTypeDescriptor<FaceIndex> descriptor)
    {
        descriptor.Ignore(x => x.Encoding);
    }
}

public static class DataUrlExtensions
{
    public static string ToDataUrl(this byte[] image, string type)
    {
        return $"data:image/{type};base64,{Convert.ToBase64String(image)}";
    }
}
