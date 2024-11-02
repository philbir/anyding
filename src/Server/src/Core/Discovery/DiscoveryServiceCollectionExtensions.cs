using Anyding.Connectors;
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
        // File System


        return services;
    }
}

