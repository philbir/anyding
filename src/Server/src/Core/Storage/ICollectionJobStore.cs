using Anyding.Discovery;
using Anyding.Geo;

namespace Anyding.Data;

public interface ICollectionJobStore
{
    Task<CollectorJob> AddJobAsync(CollectorJob job, CancellationToken cancellationToken);
}

public interface IGeoReverseEncodingCacheStore
{
    Task<GeoReverseEncodingCache> AddAsync(
        GeoReverseEncodingCache cache,
        CancellationToken cancellationToken);

    Task<GeoReverseEncodingCache?> GetAsync(
        string id,
        CancellationToken cancellationToken);
}
