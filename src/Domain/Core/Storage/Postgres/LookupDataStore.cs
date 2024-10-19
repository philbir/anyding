using Anyding.Data;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Storage.Postgres;

public interface ILookupDataStore
{
    ThingClass? GetThingClass(string name);
    Task Initialize(AnydingDbContext dbContext, CancellationToken ct);
}

public class LookupDataStore : ILookupDataStore
{
    private List<ThingClass> _classes;

    public async Task Initialize(AnydingDbContext dbContext, CancellationToken ct)
    {
        if (_classes != null)
        {
            return;
        }

        _classes = await dbContext.ThingClasses.ToListAsync(ct);
    }

    public ThingClass? GetThingClass(string name)
    {
        return _classes.FirstOrDefault(c => c.Name == name);
    }
}
