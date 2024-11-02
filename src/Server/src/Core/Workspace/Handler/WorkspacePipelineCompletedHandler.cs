using Anyding.Events;
using MediatR;

namespace Anyding;

public class WorkspacePipelineCompletedHandler(ThingsWorkspaceHarvester harvester)
    : INotificationHandler<WorkspacePipelineCompletedEvent>
{
    public async Task Handle(
        WorkspacePipelineCompletedEvent notification,
        CancellationToken cancellationToken)
    {
        await harvester.RunAsync(notification.WorkspaceId, cancellationToken);
    }
}
