using Anyding.Discovery;
using Anyding.Storage.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Ingest;

public class LoadThingsMiddleware(IConnectorFactory connectorFactory) : IThingIngestionMiddleware
{
    public async Task InvokeAsync(ThingIngestionContext context, Func<Task> next)
    {
        var connector = await connectorFactory.CreateConnectorAsync(
            new ConnectorDefinition
            {
                Id = Guid.NewGuid(), Name = "Local", Type = "LFS", Properties = new Dictionary<string, string>()
                {
                    ["Root"] = "/Users/p7e/anyding/store_00"
                }
            }, CancellationToken.None);

        var classes = context.Database.ThingClasses.ToList();
        context.Database.ThingClasses.AttachRange(classes);

        foreach (CreateThingInput thing in context.Request.Things)
        {
            var thingToAdd = new Thing
            {
                Id = thing.Id,
                Name = thing.Name,
                Created = thing.Created,
                Type = thing.Type,
                Class = classes.FirstOrDefault(x => x.Name == thing.ClassName),
                Detail = thing.Details,
                Source = thing.Source,
            };

            foreach (ThingInputData data in thing.Data)
            {
                var identifier = Path.Combine(thing.Id.ToString("N"), data.Id);

                await connector.UploadAsync(data.Id, thing.Id.ToString("N"), data.LoadData(), context.Cancelled);

                thingToAdd.Data.Add(new ThingStorageReference
                {
                    Id = Guid.NewGuid(),
                    ThingId = thing.Id,
                    ConnectorId = connector.Id,
                    Identifier = identifier,
                    Type = data.Type,
                    ContentType = data.ContentType
                });
            }

            context.WriteThings(thingToAdd);
        }

        context.WriteConnections(context.Request.Connections.ToArray());

        await next();
    }
}
