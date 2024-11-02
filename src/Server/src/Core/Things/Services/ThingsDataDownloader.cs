using Anyding.Connectors;
using Anyding.Data;
using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace Anyding;

public class ThingsDataDownloader(IAnydingDbContext dbContext, IConnectorFactory connectorFactory)
    : IThingsDataDownloader
{
    public async Task<Stream> DownloadAsync(ThingDataReference dataReference, CancellationToken ct)
    {
        IConnector connector = await connectorFactory.CreateConnectorAsync(dataReference.ConnectorId, ct);
        return await connector.DownloadAsync(dataReference.Identifier, ct);
    }

    public async Task<ThingData> DownloadAsync(Guid thingId, string name, CancellationToken ct)
    {
        ThingDataReference dataRef = await dbContext.ThingData
            .Where(x => x.ThingId == thingId && x.Name == name)
            .FirstOrDefaultAsync(ct);

        IConnector connector = await connectorFactory.CreateConnectorAsync(dataRef.ConnectorId, ct);
        Stream stream = await connector.DownloadAsync(dataRef.Identifier, ct);

        return new ThingData
        {
            Reference = dataRef,
            Stream = stream
        };
    }

    public async Task<IReadOnlyList<ThingData>> DownloadBatchAsync(
        IEnumerable<Guid> thingIds,
        string name,
        CancellationToken ct)
    {
        List<ThingDataReference> dataRefs = await dbContext.ThingData
            .Where(x => thingIds.Contains(x.ThingId) && x.Name == name)
            .ToListAsync(ct);

        var results = new List<ThingData>();

        IEnumerable<IGrouping<string, ThingDataReference>> groupedDataRefs = dataRefs.GroupBy(x => x.ConnectorId);

        foreach (IGrouping<string, ThingDataReference> group in groupedDataRefs)
        {
            IConnector connector = await connectorFactory.CreateConnectorAsync(group.Key, ct);
            IReadOnlyList<(byte[] data, string id)> data = await connector.DownloadBatchAsync(
                group.Select(x => x.Identifier),
                ct);

            foreach (var (d, id) in data)
            {
                ThingDataReference reference = group.First(x => x.Identifier == id);
                results.Add(new ThingData
                {
                    Reference = reference,
                    Stream = new MemoryStream(d)
                });
            }
        }

        return results;
    }
}
