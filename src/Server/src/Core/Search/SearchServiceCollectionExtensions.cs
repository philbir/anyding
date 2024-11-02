using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Typesense;
using Typesense.Setup;

namespace Anyding.Search;

public static class SearchServiceCollectionExtensions
{
    public static IAnydingServerBuilder AddTypeSense(this IAnydingServerBuilder builder)
    {
        builder.Services.AddOptions<TypesenseOptions>()
            .Bind(builder.Configuration.GetSection("Search:Typesense"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddTypesenseClient(jsonOptions =>
        {
            jsonOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
        });
        builder.Services.AddScoped<Func<ITypesenseClient>>(provider =>
            provider.GetRequiredService<ITypesenseClient>);

        builder.Services.AddSingleton<ISearchDbContext, SearchDbContext>();
        builder.Services.AddScoped<MediaSearchClient>();
        builder.Services.AddScoped<IThingsIndexingService, ThingsIndexingService>();

        return builder;
    }
}
