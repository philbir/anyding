using Anyding.Data;
using Anyding.Events;
using MediatR;

namespace Anyding;

public interface IThingIngestionService
{
    Task IngestAsync(CreateThingsRequest request, CancellationToken ct);
}

public class ThingIngestionService(
    IAnydingDbContext db,
    IThingDataManager dataManager,
    IMediator mediator) : IThingIngestionService
{
    public async Task IngestAsync(CreateThingsRequest request, CancellationToken ct)
    {
        var classes = db.ThingClasses.ToList();
        db.ThingClasses.AttachRange(classes);

        foreach (CreateThingInput thing in request.Things)
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

            thingToAdd.Ingested();

            foreach (ThingInputData data in thing.Data)
            {
                ThingDataReference dataRef = await dataManager.UploadAsync(new UploadThingDataRequest(
                    thing.Id,
                    data.Id,
                    data.Name,
                    data.Type,
                    data.ContentType,
                    data.LoadData()), ct);

                thingToAdd.Data.Add(dataRef);
            }

            await db.Things.AddRangeAsync(thingToAdd);
        }

        db.ThingConnections.AddRange(request.Connections);
        await db.SaveChangesAsync(ct);

        await mediator.Publish(new ThingsIngestedEvent(
            request.Things.Select(x => x.Id).ToArray()), ct);
    }
}
