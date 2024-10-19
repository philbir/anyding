using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anyding;

public static class AnydingHostBuilder
{
    public static IAnydingServerBuilder AddAnydingServer(
        this IHostApplicationBuilder builder)
    {
        return new AnydingServerBuilder(
            builder.Services,
            builder.Configuration.GetSection("Anyding"));
    }
}

public interface IAnydingServerBuilder
{
    IServiceCollection Services { get; }
    IConfiguration Configuration { get; }
}

public class AnydingServerBuilder(
    IServiceCollection services,
    IConfiguration configuration) : IAnydingServerBuilder
{
    public IServiceCollection Services { get; } = services;
    public IConfiguration Configuration { get; } = configuration;
}
