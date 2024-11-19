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
        }).WithMetadata(new CacheResponseMetadata((int)TimeSpan.FromDays(1).TotalSeconds));

        return group;
    }
}

class CacheResponseMetadata
{
    public CacheResponseMetadata(int maxAge)
    {
        MaxAge = maxAge;
    }

    public int MaxAge { get; }
}

class AddCacheHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public AddCacheHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (httpContext.GetEndpoint()?.Metadata.GetMetadata<CacheResponseMetadata>() is { } cacheResponseMetadata)
        {
            if (!httpContext.Response.HasStarted)
            {
                httpContext.Response.Headers.CacheControl = new[]
                {
                    "private", $"max-age={cacheResponseMetadata.MaxAge}"
                };
            }
        }

        await _next(httpContext);
    }
}
