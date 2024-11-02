using Anyding.Connectors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anyding.Connector;

public static class ConnectorServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddConnectors(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddConnectorServices(builder.Configuration);

        return builder;
    }

    private static IServiceCollection AddConnectorServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ConnectorOptions>()
            .Bind(configuration.GetSection("Connector"))
            .ValidateDataAnnotations();

        services.AddScoped<IFileSystemStore, FileSystemStore>();
        services.AddScoped<IConnector, FileSystemConnector>();
        services.AddScoped<IConnector, PostgresDatabaseConnector>();
        services.AddScoped<IConnectorFactory, DefaultConnectorFactory>();
        services.AddScoped<IConnectorManager, DefaultConnectorManager>();
        services.AddScoped<DiscoveryJob>();

        //Queries
        services.AddScoped<IConnectorDefinitionQuery, ConnectorDefinitionQuery>();

        return services;
    }
}

public class ConnectorOptions
{
    public string Root { get; set; }

    public string Default { get; set; }
}
