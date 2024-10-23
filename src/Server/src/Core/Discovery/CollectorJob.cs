namespace Anyding.Discovery;

public class CollectorJob
{
    public Guid Id { get; set; }

    public Guid ConnectorId { get; set; }

    public string ItemType { get; set; }

    public DateTime CreatedAt { get; set; }

    public JobStatus Status { get; set; }
}

public enum JobStatus
{
    New,
    Running,
    Completed,
    Failed
}

public interface IConnectorManager
{
    string[] ManagedTypes { get; }

    ValueTask<IConnector> CreateAsync(
        ConnectorDefinition definition,
        CancellationToken cancellationToken);
}

public interface IConnector
{
    Guid Id { get; set; }
    ValueTask ConnectAsync(ConnectorDefinition definition, CancellationToken ct);
    Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(DiscoveryFilter filter);
    Task UploadAsync(string id, string path, Stream data, CancellationToken ct);
    ValueTask<Stream> DownloadAsync(string id, CancellationToken ct);
    ValueTask DeleteAsync(string id, CancellationToken ct);
    ValueTask MoveAsync(string id, string path, CancellationToken ct);
}
