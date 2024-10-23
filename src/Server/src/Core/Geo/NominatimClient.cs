using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Anyding.Geo;

/// <summary>
/// https://nominatim.org/release-docs/latest/api/Overview/
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="logger"></param>
public class NominatimClient(
    IHttpClientFactory httpClientFactory,
    ILogger<NominatimClient> logger) : IGeoCodingSource
{
    internal const string HttpClientName = "Nominatim";
    private const string _sourceName = "OSM_Nominatim";

    public async Task<ReverseGeoCodeResult> ReverseGeoCodeAsync(
        double latitude,
        double longitude,
        CancellationToken ct)
    {
        logger.LogInformation("Reverse GeoCode location for {Latitude}, {Longitude}", latitude, longitude);
        HttpClient client = httpClientFactory.CreateClient(HttpClientName);

        NominatimReverseGeoCodeResponse? response = await client.GetFromJsonAsync<NominatimReverseGeoCodeResponse>(
            $"/reverse?format=geocodejson&lat={latitude}&lon={longitude}&zoom=18",
            cancellationToken: ct);

        if (response != null)
        {
            return ToGeoReverseDecodeResult(response);
        }
        return new ReverseGeoCodeResult { Found = false, Source = "Nominatim" };
    }

    private ReverseGeoCodeResult ToGeoReverseDecodeResult(NominatimReverseGeoCodeResponse response)
    {
        FeatureGeocoding? geoCoding = response.Features.FirstOrDefault()?.Properties.Geocoding;

        if ( geoCoding == null)
        {
            return new ReverseGeoCodeResult { Found = false, Source = _sourceName };
        }

        var result = new ReverseGeoCodeResult
        {
            Found = true,
            Source = _sourceName,
            Raw = JsonSerializer.Serialize(response),
            GeoCoding = new GeoCoding
            {
                Type = geoCoding.Type,
                Name = geoCoding.Name,
                Label = geoCoding.Label,
                HouseNumber = geoCoding.HouseNumber,
                PostCode = geoCoding.PostCode,
                Street = geoCoding.Street,
                Locality = geoCoding.Locality,
                District = geoCoding.District,
                City = geoCoding.City,
                County = geoCoding.County,
                State = geoCoding.State,
                Country = geoCoding.Country,
                CountryCode = geoCoding.CountryCode,
                Geometry = new Geometry
                {
                    Type = response.Features.FirstOrDefault()?.Geometry.Type,
                    Coordinates = response.Features.FirstOrDefault()?.Geometry.Coordinates
                },
                OpenStreetMap = new OpenStreetMapReference
                {
                    Version = response.Geocoding.Version,
                    Type = geoCoding.OsmType,
                    Id = geoCoding.OsmId,
                    PlaceId = geoCoding.PlaceId,
                    OsmKey = geoCoding.OsmKey,
                    OsmValue = geoCoding.OsmValue,
                    Accuracy = geoCoding.Accuracy
                }
            }
        };

        return result;
    }
}


