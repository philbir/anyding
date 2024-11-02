using Anyding.Connectors;


namespace Anyding.Data.Postgres;


public interface IThingStore
{
    Task<Thing> AddAsync(Thing thing, CancellationToken cancellationToken);
}

public class ThingStore(IAnydingDbContext dbContext) : IThingStore
{
    public async Task<Thing> AddAsync(Thing thing, CancellationToken cancellationToken)
    {
        await dbContext.Things.AddAsync(thing, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return thing;
    }
}
