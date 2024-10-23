using Anyding.Data;

namespace Anyding;

public class ThingsDataQuery
{
    private readonly IConnectorFactory _connectorFactory;
    private readonly AnydingDbContext _anydingDbContext;

    public ThingsDataQuery(IConnectorFactory connectorFactory, AnydingDbContext anydingDbContext)
    {
        _connectorFactory = connectorFactory;
        _anydingDbContext = anydingDbContext;
    }

    public async Task<ThingStorageReference> GetDataAsync(Guid id)
    {

    }
}
