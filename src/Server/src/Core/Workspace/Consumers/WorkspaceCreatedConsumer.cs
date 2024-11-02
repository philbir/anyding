using Anyding.Discovery.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Anyding.Consumers;

public class WorkspaceCreatedConsumer(ILogger<WorkspaceCreatedEvent> logger) :
    IConsumer<WorkspaceCreatedEvent>
{
    public async Task Consume(ConsumeContext<WorkspaceCreatedEvent> context)
    {
        logger.LogInformation("Workspace created: {WorkspaceId}", context.Message.WorkspaceId);

        await Task.CompletedTask;
    }
}
