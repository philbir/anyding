
namespace Anyding.Geo;
using Microsoft.Extensions.DependencyInjection;


public static class GeoLocationServiceCollectionExtensions
{
    private const string _nominatimUrl = "https://nominatim.openstreetmap.org";

    public static IAnydingServerBuilder AddGeoLocation(this IAnydingServerBuilder builder)
    {
        builder.Services.AddScoped<IGeoCodingSource, NominatimClient>();
        builder.Services.AddScoped<IGeoDecoderService, GeoDecoderService>();
        builder.Services.AddScoped<IGeoReverseEncodingCacheQueries, GeoReverseEncodingCacheQueries>();

        builder.Services.AddHttpClient(NominatimClient.HttpClientName, (client) =>
        {
            client.BaseAddress = new Uri(_nominatimUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Anyding");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en");
        });

        builder.Services.AddAnydingApplication();

        return builder;
    }
}

