using Anyding.Discovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anyding;

public static class DiscoveryServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddDiscovery(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddDiscoveryServices(builder.Configuration);
        return builder;
    }

    private static IServiceCollection AddDiscoveryServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //Add Connectors
        services.AddSingleton<FileSystemConnector>();

        services.AddSingleton<IConnectorFactory, DefaultConnectorFactory>();
        services.AddSingleton<IConnectorManager, DefaultConnectorManager>();
        services.AddScoped<ItemCollector>();

        // File System
        services.AddOptions<FileStorageOptions>()
            .Bind(configuration.GetSection("Storage:FileSystem"))
            .ValidateDataAnnotations();

        services.AddSingleton<IFileSystemStore, FileSystemStore>();
        services.AddSingleton<IWorkspaceFactory, WorkspaceFactory>();
        services.AddSingleton<WorkspaceRunner>();

        return services;
    }
}

