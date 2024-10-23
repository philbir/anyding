using System.Text.Json.Serialization;

namespace Anyding.Search;

public class GeoNameIndex
{
    public string Value { get; set; }

    public string Type { get; set; }

    [JsonConstructor]
    public GeoNameIndex(string type, string value)
    {
        Value = value;
        Type = type;
    }
}
