using Anyding.Query;
using Anyding.Search;
using MediatR;

namespace Anyding;

public record ThingIngestedEvent(Guid Id) : Event;

public class ThingsIngestedHandler(ThingLoader thingLoader, IThingsIndexingService indexingService) : INotificationHandler<ThingsIngestedEvent>
{
    public async Task Handle(ThingsIngestedEvent notification, CancellationToken cancellationToken)
    {
        LoadThingOptions options = LoadThingOptions.Default;
        options.Filter2.Where(x => notification.ThingIds.Contains(x.Id));
        options.IncludeData = true;
        options.IncludeConnections = true;
        options.IncludeTags = true;
        options.PageSize = int.MaxValue;

        List<IThing> things = await thingLoader.LoadAsych(notification.ThingIds, options, cancellationToken);
        await indexingService.IndexAsync(new IndexThingsRequest
        {
            Things = things.ToList()
        }, cancellationToken);
    }
}
