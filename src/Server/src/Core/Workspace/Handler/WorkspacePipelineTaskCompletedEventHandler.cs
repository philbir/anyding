using Anyding.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Anyding.Handler;

public class WorkspacePipelineTaskCompletedEventHandler(ILogger<WorkspacePipelineTaskCompletedEventHandler> logger)
    : INotificationHandler<WorkspacePipelineTaskCompletedEvent>
{
    public async Task Handle(
        WorkspacePipelineTaskCompletedEvent notification,
        CancellationToken cancellationToken)
    {
        logger.WorkspacePipelineTaskCompleted(notification.WorkspaceId, notification.TaskName);
    }
}

internal static partial class Logs
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message =
            "Workspace pipeline task completed {workspaceId}, {taskName}")]
    public static partial void WorkspacePipelineTaskCompleted(
        this ILogger logger,
        Guid workspaceId,
        string taskName);
}
