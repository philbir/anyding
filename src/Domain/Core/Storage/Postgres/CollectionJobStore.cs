using Anyding.Discovery;


namespace Anyding.Data.Postgres;

public class CollectionJobStore(AnydingDbContext dbContext) : ICollectionJobStore
{
    public async Task<CollectorJob> AddJobAsync(CollectorJob job, CancellationToken cancellationToken)
    {
        await dbContext.CollectorJobs.AddAsync(job, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return job;
    }
}

public interface IThingStore
{
    Task<Thing> AddAsync(Thing thing, CancellationToken cancellationToken);
}

public class ThingStore(AnydingDbContext dbContext) : IThingStore
{
    public async Task<Thing> AddAsync(Thing thing, CancellationToken cancellationToken)
    {
        await dbContext.Things.AddAsync(thing, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return thing;
    }
}
