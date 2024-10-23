using Anyding.Geo;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Data.Postgres;

public class GeoReverseEncodingCacheStore(IDbContextFactory<AnydingDbContext> dbContextFactory) : IGeoReverseEncodingCacheStore
{
    public async Task<GeoReverseEncodingCache> AddAsync(
        GeoReverseEncodingCache cache,
        CancellationToken cancellationToken)
    {
        await using AnydingDbContext dbContext = await dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        await dbContext.GeoReverseEncodings.AddAsync(cache, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return cache;
    }

    public async Task<GeoReverseEncodingCache?> GetAsync(
        string id,
        CancellationToken cancellationToken)
    {
        await using AnydingDbContext dbContext = await dbContextFactory
            .CreateDbContextAsync(cancellationToken);

        return await dbContext.GeoReverseEncodings
            .Where(x =>x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
