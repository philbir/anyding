using Anyding.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Anyding;

public static class AnydingHostBuilder
{
    public static IAnydingServerBuilder AddAnydingServer(
        this IHostApplicationBuilder builder)
    {
        var server = new AnydingServerBuilder(
            builder.Services,
            builder.Configuration.GetSection("Anyding"));

        server.AddCoreServices();

        return server;
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

public static class AnydingServerBuilderExtensions
{
    public static IAnydingServerBuilder AddMediatR(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<AnydingServerBuilder>();
                config.NotificationPublisherType = typeof(MassTransitNotificationPublisher);
            });

        return builder;
    }

    internal static IAnydingServerBuilder AddCoreServices(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddOptions<FileSystemOptions>()
            .Bind(builder.Configuration.GetSection("Storage:FileSystem"))
            .ValidateDataAnnotations();

        return builder;
    }

    public static IAnydingServerBuilder AddMassTransit(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddScoped<WorkspaceCreatedConsumer>();
        builder.Services.AddMassTransit(ConfigureMassTransit);

        return builder;
    }

    private static void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
    {
        //configurator.AddConsumers(typeof(AnydingHostBuilder).Assembly);
        configurator.AddConsumer<WorkspaceCreatedConsumer>();
        configurator.UsingInMemory((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        });
        /*
        configurator.UsingRabbitMq((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
        });*/
    }
}
