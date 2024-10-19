namespace Anyding.Media;

public class MediaMetadata
{
    public MediaDimension? Dimension { get; set; }
    public GeoLocation? GeoLocation { get; set; }
    public string? Orientation { get; set; }
    public CameraInfo? Camera { get; set; }

    public string? ImageId { get; set; }
    public DateTime? DateTaken { get; set; }
    public string? Lens { get; set; }
}

public class ImageMetadata : MediaMetadata
{
    public string? ImageId { get; set; }
}

public class VideoMetadata : MediaMetadata
{
    public string? PixelFormat { get; set; }
    public double FrameRate { get; set; }
    public double Bitrate { get; set; }
    public TimeSpan Duration { get; set; }
    public string Codec { get; set; }
}

public enum MediaOrientation
{
    Landscape,
    Portrait
}


