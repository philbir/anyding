namespace Anyding;

public interface IThingDataByIdQuery
{
    Task<IReadOnlyList<ThingDataReference>> ExecuteAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<ThingDataReference> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken);
}
