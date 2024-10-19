using System.Text.Json.Serialization;
namespace Anyding.Geo;


public class NominatimReverseGeoCodeResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("geocoding")]
    public NominatimGeocoding Geocoding { get; set; }

    [JsonPropertyName("features")]
    public List<GeoFeature> Features { get; set; }
}

public class NominatimGeocoding
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("attribution")]
    public string Attribution { get; set; }

    [JsonPropertyName("licence")]
    public string Licence { get; set; }

    [JsonPropertyName("query")]
    public string Query { get; set; }
}

public class GeoFeature
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("properties")]
    public GeoProperties Properties { get; set; }

    [JsonPropertyName("geometry")]
    public Geometry? Geometry { get; set; }
}

public class GeoProperties
{
    [JsonPropertyName("geocoding")]
    public FeatureGeocoding? Geocoding { get; set; }
}

public class FeatureGeocoding
{
    [JsonPropertyName("place_id")]
    public long PlaceId { get; set; }

    [JsonPropertyName("osm_type")]
    public string OsmType { get; set; }

    [JsonPropertyName("osm_id")]
    public long OsmId { get; set; }

    [JsonPropertyName("osm_key")]
    public string OsmKey { get; set; }

    [JsonPropertyName("osm_value")]
    public string OsmValue { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("accuracy")]
    public int Accuracy { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("housenumber")]
    public string? HouseNumber { get; set; }

    [JsonPropertyName("postcode")]
    public string? PostCode { get; set; }

    [JsonPropertyName("street")]
    public string? Street { get; set; }

    [JsonPropertyName("locality")]
    public string? Locality { get; set; }

    [JsonPropertyName("district")]
    public string? District { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("county")]
    public string? County { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; }

    [JsonPropertyName("admin")]
    public GeoAdminHierarchy GeoAdminHierarchy { get; set; }
}

public class GeoAdminHierarchy
{
    [JsonPropertyName("level8")]
    public string Level8 { get; set; }

    [JsonPropertyName("level6")]
    public string Level6 { get; set; }

    [JsonPropertyName("level4")]
    public string Level4 { get; set; }
}
