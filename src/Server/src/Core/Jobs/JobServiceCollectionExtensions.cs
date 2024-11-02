using Anyding.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Anyding;

public static class JobServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddJobs(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddJobs();

        return builder;
    }

    public static IServiceCollection AddJobs(
        this IServiceCollection services)
    {
        services.AddScoped<JobFactory>();
        services.AddScoped<JobManager>();
        services.AddScoped<IJob, DiscoveryJob>();

        return services;
    }
}
