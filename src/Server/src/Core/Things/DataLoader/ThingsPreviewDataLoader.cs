using GreenDonut;

namespace Anyding;

public static class ThingsPreviewDataLoader
{
    [DataLoader]
    public static async Task<IReadOnlyDictionary<Guid, byte[]>> GetPreviewDataByIdAsync(
        IReadOnlyList<Guid> thingIds,
        [DataLoaderState("name")] string name,
        IThingsDataDownloader downloader,
        CancellationToken ct)
    {
        IReadOnlyList<ThingData> previews = await downloader.DownloadBatchAsync(thingIds, name, ct);

        return previews.ToDictionary(x => x.Reference.ThingId, x => x.Stream.ToByteArray());
    }

    public static IPreviewDataByIdDataLoader WithName(this IPreviewDataByIdDataLoader loader, string name)
    {
        loader.SetState(nameof(name), name);

        return loader;
    }
}
