using Anyding.Data;
using GreenDonut;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Geo;

public static class GeoReverseEncodingCacheDataLoader
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<string, GeoReverseEncodingCache>> GetGeoReverseEncodingCacheByIdAsync(
        IReadOnlyList<string> ids,
        AnydingDbContext db,
        CancellationToken ct) => await db.GeoReverseEncodings
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id.ToString(), ct);
}
