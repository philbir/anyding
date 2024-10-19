using System.Text.Json;
using Anyding.Data;
using Microsoft.Extensions.Logging;

namespace Anyding.Geo;

public class GeoDecoderService(
    IGeoReverseEncodingCacheStore store,
    IEnumerable<IGeoCodingSource> sources,
    ILogger<GeoDecoderService> logger) : IGeoDecoderService
{
    public async Task<GeoCoding?> ReverseAsync(
        double latitude,
        double longitude,
        CancellationToken ct)
    {
        var key = $"{latitude}_{longitude}";
        GeoReverseEncodingCache? cache = await store.GetAsync(key, ct);

        if (cache != null)
        {
            return JsonSerializer.Deserialize<GeoCoding>(cache.GeoCoding);
        }

        foreach (IGeoCodingSource source in sources)
        {
            ReverseGeoCodeResult? sourceResult = await source.ReverseGeoCodeAsync(latitude, longitude, ct);

            if (sourceResult.Found)
            {
                var newCache = new GeoReverseEncodingCache
                {
                    Id = key,
                    Created = DateTime.UtcNow,
                    Source = sourceResult.Source,
                    GeoCoding = JsonSerializer.Serialize(sourceResult.GeoCoding),
                    Raw = sourceResult.Raw
                };

                await store.AddAsync(newCache, ct);

                return sourceResult.GeoCoding;
            }
        }

        return null;
    }
}
