namespace Anyding.Ingest;

public class SaveThingsToDatabaseMiddleware : IThingIngestionMiddleware
{
    public async Task InvokeAsync(ThingIngestionContext context, Func<Task> next)
    {
        await context.Database.Things.AddRangeAsync(context.Things);
        await context.Database.ThingConnections.AddRangeAsync(context.Connections);
        await context.Database.SaveChangesAsync();

        await next();
    }
}
