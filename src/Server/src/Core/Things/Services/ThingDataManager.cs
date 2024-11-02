using Anyding.Connectors;

namespace Anyding;

public class ThingDataManager(IConnectorFactory connectorFactory) : IThingDataManager
{
    public async Task<ThingDataReference> UploadAsync(
        UploadThingDataRequest request,
        CancellationToken ct)
    {
        IConnector connector = request.ConnectorId switch
        {
            null => await connectorFactory.CreateConnectorAsync(
                new Dictionary<string, string>()
                {
                    {"Name", request.Name},
                    {"Type", request.Type},
                }, ct),
            _ => await connectorFactory.CreateConnectorAsync(request.ConnectorId, ct)
        };
        var path = CreatePath(request.ThingId);

        UploadResult uploadResult = await connector.UploadAsync(
            request.Id,
            path,
            request.Data,
            ct);

        return new ThingDataReference
        {
            Id = Guid.NewGuid(),
            ThingId = request.ThingId,
            ConnectorId = connector.Id,
            Name = request.Name,
            Identifier = uploadResult.Identifier,
            Size = uploadResult.Size,
            Type = request.Type,
            ContentType = request.ContentType
        };
    }

    private string CreatePath(Guid id)
    {
        var name = id.ToString("N");
        return Path.Combine(name.Substring(0, 4), name);
    }
}

public record UploadThingDataRequest(Guid ThingId, string Id, string Name, string Type, string ContentType, Stream Data)
{
    public string? ConnectorId { get; set; }
};
