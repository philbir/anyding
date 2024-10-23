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
}

[ObjectType<FaceIndex>]
public static partial class FaceType
{
    static partial void Configure(IObjectTypeDescriptor<FaceIndex> descriptor)
    {
        descriptor.Ignore(x => x.Encoding);
    }
}
