using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;
using Anyding.Data;
using Anyding.Media;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Query;

public record LoadThingOptions
{
    public bool IncludeTags { get; set; } = false;

    public bool IncludeData { get; set; } = false;

    public bool IncludeConnections { get; set; } = false;

    public bool IncludeIntifiers { get; set; } = false;

    public Expression<Func<Thing, bool>> Filter { get; set; }

    public IQueryable<Thing> Filter2 { get; set; }

    public static LoadThingOptions Default => new();
}

public class ThingLoader(IDbContextFactory<AnydingDbContext> contextFactory)
{
    public async Task<List<IThing>> LoadAsych(
        IEnumerable<Guid> ids,
        LoadThingOptions options,
        CancellationToken ct)
    {
        options.Filter2.Where(x => ids.Contains(x.Id));

        return await LoadAsych(options, ct);
    }

    public async Task<List<IThing>> LoadAsych(
        LoadThingOptions options,
        CancellationToken ct)
    {
        AnydingDbContext db = await contextFactory.CreateDbContextAsync(ct);
        IQueryable<Thing> query = db.Things; //.AsSplitQuery();
        if ( options.Filter2 != null)
        {
            query = query.Where(options.Filter);
        }

        if (options.IncludeTags)
        {
            query = query.Include(x => x.Tags).ThenInclude(tag => tag.Definition);
        }

        if (options.IncludeData)
        {
            query = query.Include(x => x.Data);
        }

        if (options.IncludeConnections)
        {
            query = query.Include(x => x.Connections);
        }

        if (options.IncludeIntifiers)
        {
            query = query.Include(x => x.Identifiers);
        }

        List<Thing> things = await query.ToListAsync(ct);

        return things.Select(ToTypedThing).ToList();
    }

    /*
    public async Task<List<IThing>> LoadAsync(IEnumerable<Thing> things, CancellationToken ct)
    {
        return things.Select(ToTypedThing).ToList();
    }*/

    private IThing ToTypedThing(Thing thing)
    {
        switch (thing.Type)
        {
            case ThingType.Media:
                var mediaThing = (MediaThing)thing;
                return mediaThing;
            case ThingType.Face:
                var faceThing = (FaceThing)thing;
                return faceThing;
            case ThingType.Person:
                var personThing = (PersonThing)thing;
                return personThing;
            case ThingType.Device:
                var deviceThing = (DeviceThing)thing;
                return deviceThing;
            default:
                throw new InvalidOperationException();
        }
    }
}
