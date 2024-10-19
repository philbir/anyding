using Anyding.Data;
using Anyding.Discovery;
using Microsoft.Extensions.Logging;

namespace Anyding;

public class ItemCollector(
    IConnectorFactory connectorFactory,
    ICollectionJobStore jobStore,
    IWorkspaceFactory workspaceFactory,
    ILogger<ItemCollector> logger)
{
    public async Task CollectAsync(
        CancellationToken cancellationToken)
    {
        var definitions = new ConnectorDefinition[]
        {
            new()
            {
                Name = "TestSource",
                Type = ConnectorTypes.LocalFileSystem,
                Id = Guid.Parse("823d3b20-6bc3-46fe-92bb-b5f0d512dbbf"),
                Properties = new Dictionary<string, string> { { "Root", "/Users/p7e/anyding/source_01/" } }
            },
            new()
            {
                Name = "NasSource",
                Type = ConnectorTypes.LocalFileSystem,
                Id = Guid.Parse("823d3b20-6bc3-46fe-92bb-b5f0d512dbbe"),
                Properties = new Dictionary<string, string> { { "Root", "/Users/p7e/nas/home/Photos/Moments/ours" } }
            }
        };

        foreach (ConnectorDefinition definition in definitions.Where(x => x.Name == "NasSource"))
        {
            IConnector connector = await connectorFactory.CreateConnectorAsync(definition, cancellationToken);
            await connector.ConnectAsync(definition, cancellationToken);

            var filter = new DiscoveryFilter { Path = "", Filter = "jpg", IncludeChildren = true, MaxItems = 5000 };

            IReadOnlyList<DiscoveredItem> items = await connector.DiscoverAsync(filter);

            foreach (DiscoveredItem item in items)
            {
                logger.LogInformation("Discovered item {Name} of type {Type}", item.Name, item.ItemType);

                var job = new CollectorJob
                {
                    Id = Guid.NewGuid(),
                    ConnectorId = definition.Id,
                    ItemType =  item.ItemType,
                    CreatedAt = DateTime.UtcNow
                };

                await jobStore.AddJobAsync(job, cancellationToken);
                await using Stream fileStream = await connector.DownloadAsync(item.Id, cancellationToken);

                await workspaceFactory.CreateNewWorkspaceAsync(job.Id, item, fileStream, cancellationToken);
            }
        }
    }
}
