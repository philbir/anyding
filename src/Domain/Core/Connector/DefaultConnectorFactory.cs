using Anyding.Discovery;

namespace Anyding;

public interface IConnectorFactory
{
    Task<IConnector> CreateConnectorAsync(
        ConnectorDefinition definition,
        CancellationToken cancellationToken);
}

public class DefaultConnectorFactory(IEnumerable<IConnectorManager> connectionManagers) : IConnectorFactory
{
    public async Task<IConnector> CreateConnectorAsync(
        ConnectorDefinition definition,
        CancellationToken cancellationToken)
    {
        IConnectorManager? manager = connectionManagers.SingleOrDefault(
            x => x.ManagedTypes.Contains(definition.Type));

        if (manager is null)
        {
            throw new InvalidOperationException(
                $"No manager found for type: {definition.Type}");
        }

        return await manager.CreateAsync(definition, cancellationToken);
    }
}
