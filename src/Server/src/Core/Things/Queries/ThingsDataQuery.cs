using System.Collections;
using Anyding.Connector;
using Anyding.Data;
using Anyding.Connectors;

namespace Anyding;

public class ThingsDataQuery(
    IConnectorFactory connectorFactory,
    ThingDataByIdQuery thingDataByIdQuery)
{
    public async Task<ThingData> DownloadAsStreamAsync(Guid id, CancellationToken ct)
    {
        ThingDataReference dataReference = await thingDataByIdQuery
            .ExecuteAsync(id, ct);

        IConnector connector = await connectorFactory.CreateConnectorAsync(
            dataReference.ConnectorId,
            ct);

        var data = new ThingData
        {
            Reference = dataReference,
            Stream = await connector.DownloadAsync(dataReference.Identifier, ct)
        };

        return data;
    }
}

public class ThingDataByIdQuery(IAnydingDbContext dbContext) : IThingDataByIdQuery
{
    public async Task<IReadOnlyList<ThingDataReference>> ExecuteAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        return await dbContext.ThingData.QueryByIdsAsync(ids, cancellationToken);
    }

    public async Task<ThingDataReference> ExecuteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<ThingDataReference> result = await ExecuteAsync([id], cancellationToken);
        return result.FirstOrDefault();
    }
}
