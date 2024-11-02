using Microsoft.Extensions.DependencyInjection;

namespace Anyding;

public static class WorkspaceServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddWorkspaces(this IAnydingServerBuilder builder)
    {
        builder.Services.AddWorkspaces();

        return builder;
    }

    public static IServiceCollection AddWorkspaces(this IServiceCollection services)
    {
        services.AddScoped<WorkspaceRunner>();
        services.AddScoped<IWorkspaceFactory, WorkspaceFactory>();
        services.AddScoped<IThingsWorkspaceHarvester, ImageWorkspaceHarvester>();
        services.AddScoped<ThingsWorkspaceHarvester>();

        return services;
    }
}
