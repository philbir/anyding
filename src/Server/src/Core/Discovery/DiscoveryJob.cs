using System.Text.Json;
using Anyding.Connector;
using Anyding.Data;
using Anyding.Connectors;
using Anyding.Discovery.Events;
using Anyding.Jobs;
using Anyding.Jobs.Command;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Anyding;

public class DiscoveryJob(
    IConnectorFactory connectorFactory,
    IWorkspaceFactory workspaceFactory,
    IMediator mediator,
    ILogger<DiscoveryJob> logger) : IJob
{
    public async Task RunAsync(JobContext context, CancellationToken ct)
    {
        DiscoveryJobDetails? details = JsonSerializer.Deserialize<DiscoveryJobDetails>(context.Defintion.Details);
        IConnector connector = await connectorFactory.CreateConnectorAsync(details.ConnectorId, ct);
        IReadOnlyList<DiscoveredItem> items = await connector.DiscoverAsync(details.Filter, ct);

        foreach (DiscoveredItem item in items)
        {
            logger.LogInformation("Discovered item {Name} of type {Type}", item.Name, item.ItemType);
            await using Stream fileStream = await connector.DownloadAsync(item.Id, ct);

            var command = new AddJobStepCommand(context.JobId, $"{item.Id}");
            await mediator.Send(command, ct);

            IWorkspace workspace = await workspaceFactory.CreateNewWorkspaceAsync(command.Id, item, fileStream, ct);

            var workspaceCreatedEvent = new WorkspaceCreatedEvent(workspace.Id) { JobId = context.JobId };
            await mediator.Publish(workspaceCreatedEvent, ct);
        }

        var completedCommand = new JobCompletedCommand(context.JobId, JobStatus.Completed);
        await mediator.Send(completedCommand, ct);
    }

    public JobType Type => JobType.Discovery;
}
