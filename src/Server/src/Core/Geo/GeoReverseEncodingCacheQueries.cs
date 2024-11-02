using Anyding.Data;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Geo;

public interface IGeoReverseEncodingCacheQueries
{
    Task<IReadOnlyList<GeoReverseEncodingCache>> GetByIdAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken);

    Task<GeoReverseEncodingCache?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken);
}

public class GeoReverseEncodingCacheQueries(
    IGeoReverseEncodingCacheByIdDataLoader geoCacheById)
    : IGeoReverseEncodingCacheQueries
{
    public Task<IReadOnlyList<GeoReverseEncodingCache>> GetByIdAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken) =>
            geoCacheById.LoadAsync(ids.ToList(), cancellationToken);

    /*
    public async Task<IReadOnlyList<GeoReverseEncodingCache>> GetByIdAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken)
    {
        return await db.GeoReverseEncodings
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }*/

    public async Task<GeoReverseEncodingCache?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<GeoReverseEncodingCache> items = await GetByIdAsync([id], cancellationToken);
        return items.FirstOrDefault();
    }
}
