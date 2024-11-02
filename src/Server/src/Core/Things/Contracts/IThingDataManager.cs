namespace Anyding;

public interface IThingDataManager
{
    Task<ThingDataReference> UploadAsync(
        UploadThingDataRequest request,
        CancellationToken ct);
}
