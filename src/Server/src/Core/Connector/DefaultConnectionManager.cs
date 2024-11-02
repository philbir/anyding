using Anyding.Connectors;
using Anyding.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Anyding;

public class DefaultConnectorManager(IServiceProvider serviceProvider) : IConnectorManager
{
    public string[] ManagedTypes => [ConnectorTypes.LocalFileSystem, ConnectorTypes.PostgresDatabase];

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
            case ConnectorTypes.PostgresDatabase:
                connector = new PostgresDatabaseConnector(serviceProvider.GetRequiredService<IAnydingDbContext>());
                break;
            default:
                throw new NotSupportedException(
                    $"Not supported Type: {definition.Type}");
        }

        await connector.ConnectAsync(definition, cancellationToken);

        return connector;
    }
}

public static class ConnectorTypes
{
    public const string LocalFileSystem = "LFS";
    public const string PostgresDatabase = "PGDB";
}
