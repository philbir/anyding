using Anyding.Data;
using Anyding.Data.Postgres;
using Anyding.Storage.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anyding;

public  static class PostgresStoreServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddPostgresStore(this IAnydingServerBuilder builder)
    {
        builder.Services.AddOptions<PostgresOptions>()
            .Bind(builder.Configuration.GetSection("Storage:Postgres"))
            .ValidateDataAnnotations();

        builder.Services.AddDbContext<AnydingDbContext>((provider, options) =>
        {
            PostgresOptions postgresOptions = provider.GetRequiredService<IOptions<PostgresOptions>>().Value;
            options
                .UseNpgsql(postgresOptions.ConnectionString)
                .UseSnakeCaseNamingConvention();
        });

        builder.Services.AddScoped<IAnydingDbContext, AnydingDbContext>();

        return builder;
    }
}
