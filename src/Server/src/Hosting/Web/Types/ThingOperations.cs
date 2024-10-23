using Anyding.Query;

namespace Anyding.Api;

public static class ThingOperations
{
    [Query]
    public static async Task<List<IThing>> GetThingsAsync(
        GetThingsRequest request,
        [Service ]ThingLoader loader,
        CancellationToken ct)
    {
        LoadThingOptions options = LoadThingOptions.Default;
        options.IncludeConnections = true;
        options.PageSize = 10;
        if (request.Type is {} type)
        {
            options.Types.Add(type);
        }

        List<IThing> things = await loader.LoadAsych(options, ct);

        return things;
    }
}

public class GetThingsRequest
{
    public ThingType? Type { get; set; }
}

/*
[ObjectType<MediaThing>]
public static partial class MediaThingType
{
    static partial void Configure(IObjectTypeDescriptor<MediaThing> descriptor)
    {

    }
}

[ObjectType<PersonThing>]
public static partial class PersonThingType
{
    static partial void Configure(IObjectTypeDescriptor<PersonThing> descriptor)
    {

    }
}
*/

