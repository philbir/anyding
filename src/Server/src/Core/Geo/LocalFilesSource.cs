using System.Text.Json;

namespace Anyding.Geo;

public class LocalFilesSource(string directory) : IGeoCodingSource
{
    public async Task<ReverseGeoCodeResult> ReverseGeoCodeAsync(
        double latitude,
        double longitude,
        CancellationToken ct)
    {
        var path = Path.Combine(directory, $"{latitude}_{longitude}.json");
        if (File.Exists(path))
        {
            var json = await File.ReadAllTextAsync(path, ct);
            JsonElement jsonObject = JsonDocument.Parse(json).RootElement;

            var geoCodingJson = jsonObject.GetProperty("GeoCoding").GetString();
            var geoCoding = JsonSerializer.Deserialize<GeoCoding>(geoCodingJson);

            return new ReverseGeoCodeResult
            {
                Found = true,
                Source = "LocalFile",
                GeoCoding = geoCoding,
                Raw = jsonObject.GetProperty("Raw").GetString()
            };
        }

        return new ReverseGeoCodeResult { Found = false, Source = "LocalFile" };
    }
}
