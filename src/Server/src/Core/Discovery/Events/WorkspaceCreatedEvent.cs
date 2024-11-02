namespace Anyding.Discovery.Events;

public record WorkspaceCreatedEvent(Guid WorkspaceId) : Event
{
    public Guid? JobId { get; init; }
}
