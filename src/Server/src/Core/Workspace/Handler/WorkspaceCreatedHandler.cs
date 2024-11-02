using Anyding.Discovery.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Anyding.Handler;

public class WorkspaceCreatedHandler(WorkspaceRunner workspaceRunner) : INotificationHandler<WorkspaceCreatedEvent>
{
    public async Task Handle(
        WorkspaceCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        await workspaceRunner.RunAsync(notification.WorkspaceId, cancellationToken);
    }
}

