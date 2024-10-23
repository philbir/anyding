namespace Anyding;

public class ItemTypeMapper {

    private static readonly Dictionary<string, string> _fileTypeMappings = new()
    {
        { "jpg", ItemTypeNames.Image },
        { "heic", ItemTypeNames.Image },
        { "png", ItemTypeNames.Image },
        { "tif", ItemTypeNames.Image },
        { "mov", ItemTypeNames.Video },
        { "mp4", ItemTypeNames.Video },
        { "avi", ItemTypeNames.Video }
    };

    public static string? GetFromFileExtension(string extension)
    {
        if (_fileTypeMappings.TryGetValue(extension, out var type))
        {
            return type;
        }

        return null;
    }
}

public class ItemTypeNames
{
    public const string Image = "IMAGE";
    public const string Video = "VIDEO";
    public const string Document = "DOCUMENT";
    public const string Email = "EMAIL";
}
