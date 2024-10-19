using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Typesense;
using Typesense.Setup;

namespace Anyding.Search;

public class JsonSnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return Regex.Replace(name, "(?<!^)([A-Z])", "_$1").ToLower();
    }
}

public static class TypeSenseServiceCollectionExtensions
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
        builder.Services.AddSingleton<Func<ITypesenseClient>>(provider =>
            () => provider.GetRequiredService<ITypesenseClient>());

        builder.Services.AddSingleton<SearchDbContext>();
        return builder;
    }
}

public class TypeSenseOptions
{
    public string? Server { get; set; }
    public string? ApiKey { get; set; }
}



