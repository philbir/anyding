using System.Collections;
using Anyding.Data;
using Anyding.Media;

namespace Anyding;





public class Thing : Entity<Guid>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Detail { get; set; }
    public ThingType Type { get; set; }
    public ThingClass? Class { get; set; }
    public ThingSource? Source { get; set; }
    public ThingState Status { get; set; }
    public DateTimeOffset? Date { get; set; }
    public DateTime? Created { get; set; }
    public List<ThingTag> Tags { get; set; } = new List<ThingTag>();

    //public IReadOnlyList<ThingPreview> Thumbnails { get; set; } = new List<ThingPreview>();
    public List<ThingConnection> Connections { get; set; } = new List<ThingConnection>();
    public List<ThingDataReference> Data { get; set; } = new List<ThingDataReference>();

    public List<ThingIdentifier> Identifiers { get; set; } = new List<ThingIdentifier>();

    //public IReadOnlyList<ThingProperty> Properties { get; set; } = new List<ThingProperty>();
    public void Ingested()
    {
        Events.Add(new ThingIngestedEvent(Id));
    }
}

public enum ThingType
{
    Media = 1,
    Face = 2,
    Person = 3,
    Document = 4,
    Device = 5
}

public static class ThingConnectionTypes
{
    public static class Media
    {
        public const string TakenWith = nameof(TakenWith);
    }

    public static class Face
    {
        public const string IsPerson = nameof(IsPerson);

        public const string InMedia = nameof(InMedia);
    }
}

public class ZZZThingPreview : ImageData
{
    public string Id { get; set; }

    public int PageNumber { get; set; }
}

public class ImageInfo
{
    public ImageFormat Format { get; set; }

    public ImageSize Size { get; set; }
}

public class ImageData : ImageInfo
{
    public byte[] Data { get; set; }
}

public enum ImageFormat
{
    WebP,
    Png
}

public class ImageSize
{
    public int Height { get; set; }

    public int Width { get; set; }
}

public enum ThingState
{
    Draft,
    Processing,
    New,
    Active,
    Archived,
    Deleted
}

public class ThingData
{
    public ThingDataReference Reference { get; set; }

    public Stream Stream { get; set; }
}

public class ThingClass : Entity<Guid>
{
    public ThingType Type { get; set; }

    public string Name { get; set; }
}

public static class ThingClassNames
{
    public const string Image = "Image";
    public const string Video = "Video";
    public const string Camera = "Camera";
}

public class ThingSource
{
    public string SourceId { get; set; }

    public string ConnectorId { get; set; }

    public string? FullLocation { get; set; }
}

public class PersonDetails : IThingDetail
{
    public string Firstname { get; set; }

    public string Lastname { get; set; }

    public string? Nickname { get; set; }

    public DateOnly? DateOfBirth { get; set; }
}

public class CameraDetails : IThingDetail
{
    public string Make { get; set; }

    public string Model { get; set; }

    public string Title { get; set; }
}
