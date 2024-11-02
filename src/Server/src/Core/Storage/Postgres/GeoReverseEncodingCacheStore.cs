using Anyding.Geo;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Data.Postgres;

public class GeoReverseEncodingCacheStoreXX(
    IAnydingDbContext dbContext) : IGeoReverseEncodingCacheStoreXX
{
    public async Task<GeoReverseEncodingCache> AddAsync(
        GeoReverseEncodingCache cache,
        CancellationToken cancellationToken)
    {
        await dbContext.GeoReverseEncodings.AddAsync(cache, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return cache;
    }

    public async Task<GeoReverseEncodingCache?> GetAsync(
        string id,
        CancellationToken cancellationToken)
    {
        return await dbContext.GeoReverseEncodings
            .Where(x =>x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

