namespace Anyding.Api;

public static class ThingsDataBuilderExtensions
{
    public static RouteGroupBuilder MapThingsDataApi(this RouteGroupBuilder group)
    {
        group.MapGet("{thingId}/{name}", async (
            Guid thingId,
            string name,
            IThingsDataDownloader dataDownloader,
            CancellationToken ct) =>
        {
            ThingData data = await dataDownloader.DownloadAsync(thingId, name, ct);
            return Results.Stream(data.Stream, data.Reference.ContentType);
        });

        return group;
    }
}
