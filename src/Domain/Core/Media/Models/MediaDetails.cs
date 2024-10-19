using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using Anyding.Geo;

namespace Anyding.Media;

public class MediaDetails : IThingDetail
{
    public string Filename { get; set; }

    public LensInfo Lens { get; set; }

    public string? Orientation { get; set; }

    public DateTimeOffset? DateTaken { get; set; }
    public GeoCoding? GeoCoding { get; set; }
    public GeoLocation? GeoLocation { get; set; }
    public MediaDimension? Dimension { get; set; }
    public string? Path { get; set; }
    public object FileSize { get; set; }
}

public class FaceDetails : IThingDetail
{
    public int? AgeInMonth { get; set; }
    public double[] Encoding { get; set; }
    public ImageRactangle Box { get; set; }
    public string RecognitionType { get; set; }
    public double? DistanceThreshold { get; set; }
    public string State { get; set; }
}

public class LensInfo
{
    public string Model { get; set; }

    public string FNumber { get; set; }

    public int ISOSpeedRatings { get; set; }
}

public class CountryDetails
{
    public string Name { get; set; }

    public string Code { get; set; }
}

public interface IThingDetail{}
public interface ITagDetail{}

public interface IDetailProvider
{
    string Details { get;  }
}

public static class ThingExtension
{
    public static string SerializeToJson<T>(this T detail) where T : IThingDetail
    {
        return ThingDetailSerializer.SerializeToJson(detail);
    }

    public static T GetDetails<T>(this IDetailProvider thing) where T : IThingDetail
    {
        return ThingDetailSerializer.Deserialize<T>(thing.Details);
    }
}



public static class ThingDetailSerializer
{
    static readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    internal static string SerializeToJson<T>(T detail)
    {
        return JsonSerializer.Serialize(detail, _serializerOptions);
    }

    internal static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _serializerOptions);
    }
}

public static class TagDetailExtension
{
    public static string Serialize<T>(this T detail) where T : ITagDetail
    {
        return ThingDetailSerializer.SerializeToJson(detail);
    }
}

public class PlaceDetails : IThingDetail
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Altitude { get; set; }
    public string Name { get; set; }
    public string? City { get; set; }

    public string? Road { get; set; }
    public string Region { get; set; }
    public string Locality { get; set; }
    public string LocalAdmin { get; set; }
    public string CountryCode { get; set; }
    public string Continent { get; set; }
    public string Label { get; set; }
}

public class CameraInfo
{
    public string? Model { get; set; }
    public string? Make { get; set; }
    public string? Software { get; set; }
}

public class GeoLocation
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int? Altitude { get; set; }
    public string Source { get; set; }
}

public class ImageAITagDetails : ITagDetail
{
    public double Confidence { get; set; }
    public string Source { get; set; }
}

public class ImageAICaptionTagDetails : ITagDetail
{
    public double Confidence { get; set; }
    public string Source { get; set; }
}

public class ImageAIColorTagDetails : ITagDetail
{
    public string DominantForeground { get; set; }

    public string DominantBackground { get; set; }

    public string Accent { get; set; }

    public bool IsBlackWhite { get; set; }

    public string Source { get; set; }
}

public class ImageAIObjectTagDetails : ITagDetail
{
    public ImageRactangle Box { get; set; }

    public string Source { get; set; }

    public double Confidence { get; set; }
}
