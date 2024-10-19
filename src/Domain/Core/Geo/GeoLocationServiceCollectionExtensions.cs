
namespace Anyding.Geo;
using Microsoft.Extensions.DependencyInjection;


public static class GeoLocationServiceCollectionExtensions
{
    private const string _nominatimUrl = "https://nominatim.openstreetmap.org";

    public static IAnydingServerBuilder AddGeoLocation(
        this IAnydingServerBuilder builder)
    {
        builder.Services.AddHttpClient(NominatimClient.HttpClientName, (client) =>
        {
            client.BaseAddress = new Uri(_nominatimUrl);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Anyding");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en");
        });

        builder.Services.AddSingleton<IGeoCodingSource, NominatimClient>();
        builder.Services.AddSingleton<IGeoDecoderService, GeoDecoderService>();

        return builder;
    }
}

