using System.Text.Json.Serialization;

namespace Anyding.Geo;

public interface IGeoCodingSource
{
    Task<ReverseGeoCodeResult> ReverseGeoCodeAsync(
        double latitude,
        double longitude,
        CancellationToken ct);
}

public interface IGeoDecoderService
{
    Task<GeoCoding?> ReverseAsync(
        double latitude,
        double longitude,
        CancellationToken ct);
}

public class ReverseGeoCodeResult
{
    public bool Found { get; set; }

    public string Source { get; set; }

    public GeoCoding? GeoCoding { get; set; }

    public string Raw { get; set; }
}

public class GeoCoding
{
    public string Type { get; set; }

    public string Name { get; set; }

    public string Label { get; set; }

    public string? HouseNumber { get; set; }

    public string PostCode { get; set; }

    public string? Street { get; set; }

    public string? Locality { get; set; }

    public string? District { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? State { get; set; }

    public string Country { get; set; }

    public string CountryCode { get; set; }

    public OpenStreetMapReference OpenStreetMap { get; set; }
    public Geometry Geometry { get; set; }
}

public class Geometry
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("coordinates")]
    public List<double> Coordinates { get; set; }
}


public class OpenStreetMapReference
{
    public string Version { get; set; }

    public string Type { get; set; }

    public long Id { get; set; }

    public long PlaceId { get; set; }

    public string OsmKey { get; set; }

    public string OsmValue { get; set; }

    public int Accuracy { get; set; }
}
