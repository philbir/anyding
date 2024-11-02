namespace Anyding;

public record ThingsIngestedEvent(Guid[] ThingIds) : Event;

