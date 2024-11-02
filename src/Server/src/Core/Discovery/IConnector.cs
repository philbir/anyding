namespace Anyding.Connectors;

public interface IConnectorManager
{
    string[] ManagedTypes { get; }

    ValueTask<IConnector> CreateAsync(
        ConnectorDefinition definition,
        CancellationToken cancellationToken);
}

public interface IConnector
{
    string Id { get; set; }
    ValueTask ConnectAsync(ConnectorDefinition definition, CancellationToken ct);
    Task<IReadOnlyList<DiscoveredItem>> DiscoverAsync(DiscoveryFilter filter, CancellationToken ct);
    Task<UploadResult> UploadAsync(string id, string path, Stream data, CancellationToken ct);
    ValueTask<Stream> DownloadAsync(string id, CancellationToken ct);
    ValueTask<IReadOnlyList<(byte[] data, string id)>> DownloadBatchAsync(IEnumerable<string> ids, CancellationToken ct);
    ValueTask DeleteAsync(string id, CancellationToken ct);
    ValueTask MoveAsync(string id, string path, CancellationToken ct);
}

public record UploadResult(string Identifier, long Size);
