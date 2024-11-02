namespace Anyding.Geo;

public class GeoReverseEncodingCache : Entity<string>
{
    public DateTime Created { get; set; }

    public string Source { get; set; }

    public string  GeoCoding { get; set; }

    public string Raw { get; set; }
}
