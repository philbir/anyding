using Anyding.Data;

namespace Anyding.Ingest;

public class ThingIngestionContext
{
    public CreateThingsRequest Request { get; set; }

    public List<Thing> Things { get; set; } = new List<Thing>();

    public List<ThingConnection> Connections { get; set; } = new List<ThingConnection>();

    //public List<ThingPreview> Previews { get; set; }
    public AnydingDbContext Database { get; set; }

    public CancellationToken Cancelled { get; set; }

    public void WriteThings(params Thing[] things)
    {
        Things.AddRange(things);
    }

    public void WriteConnections(params ThingConnection[] connections)
    {
        Connections.AddRange(connections);
    }
}


public interface IThingIngestionMiddleware
{
    Task InvokeAsync(ThingIngestionContext context, Func<Task> next);
}
