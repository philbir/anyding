using Anyding.Discovery;

namespace Anyding;

public class DefaultConnectorManager(IServiceProvider serviceProvider) : IConnectorManager
{
    public string[] ManagedTypes => [ConnectorTypes.LocalFileSystem];

    public async ValueTask<IConnector> CreateAsync(
        ConnectorDefinition definition,
        CancellationToken cancellationToken)
    {
        IConnector? connector;

        switch (definition.Type)
        {
            case ConnectorTypes.LocalFileSystem:
                connector = new FileSystemConnector();
                break;
            default:
                throw new NotSupportedException(
                    $"Not supported Type: {definition.Type}");
        }

        await connector.ConnectAsync(definition, cancellationToken);

        return connector;
    }
}
