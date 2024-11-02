using Anyding.Data;
using Anyding.Connectors;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Connector;

public interface IConnectorDefinitionQuery
{
    Task<IReadOnlyList<ConnectorDefinition>> ByIdAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken);

    Task<ConnectorDefinition> ByIdAsync(string id, CancellationToken cancellationToken);

    Task<IReadOnlyList<ConnectorDefinition>> AllAsync(
        CancellationToken cancellationToken);
}

public class ConnectorDefinitionQuery(IAnydingDbContext dbContext) : IConnectorDefinitionQuery
{
    public async Task<IReadOnlyList<ConnectorDefinition>> ByIdAsync(
        IEnumerable<string> ids,
        CancellationToken cancellationToken)
    {
        return await dbContext.ConnectorDefinitions
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<ConnectorDefinition> ByIdAsync(string id, CancellationToken cancellationToken)
    {
        IReadOnlyList<ConnectorDefinition> result = await ByIdAsync([id], cancellationToken);

        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyList<ConnectorDefinition>> AllAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.ConnectorDefinitions
            .ToListAsync(cancellationToken);
    }
}

