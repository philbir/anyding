using Anyding.Connectors;
using Anyding.Geo;

namespace Anyding.Data;

public interface IGeoReverseEncodingCacheStoreXX
{
    Task<GeoReverseEncodingCache> AddAsync(
        GeoReverseEncodingCache cache,
        CancellationToken cancellationToken);

    Task<GeoReverseEncodingCache?> GetAsync(
        string id,
        CancellationToken cancellationToken);
}
