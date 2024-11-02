namespace Anyding;

public interface IThingsDataDownloader
{
    Task<IReadOnlyList<ThingData>> DownloadBatchAsync(IEnumerable<Guid> thingIds, string name, CancellationToken ct);
    Task<Stream> DownloadAsync(ThingDataReference dataReference, CancellationToken ct);
    Task<ThingData> DownloadAsync(Guid thingId, string name, CancellationToken ct);
}
