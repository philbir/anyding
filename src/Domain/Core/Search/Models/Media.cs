using Anyding.Media;
using Anyding.Search;

namespace Anyding.Search;

public class MediaIndex
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Orientation { get; set; }
    public DateIndex DateTaken { get; set; }
    public double[] Location { get; set; }
    public string? City { get; set; }
    public string Country { get; set; }
    public string PlaceName { get; set; }
    public byte[] Image { get; set; }
    public string? Street { get; set; }
    public string? CountryCode { get; set; }
    public int? Altitude { get; set; }
    public MediaDimension? Dimension { get; set; }
    public List<FaceInMediaIndex> Faces { get; set; } = new();
    public List<ThingTagIndex> Tags { get; set; } = new();
}

public enum MediaType
{
    Image,
    Video,
}
