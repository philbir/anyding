using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace Anyding.Media;

public interface IImageMetadataExtractor
{
    Task<ImageMetadata> GetMetadataAsync(
        Stream stream,
        CancellationToken cancellationToken);
}

public class ImageMetadataExtractor : IImageMetadataExtractor
{
    private const string ExifDateFormat = "yyyy:MM:dd HH:mm:ss";

    public async Task<ImageMetadata> GetMetadataAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        Image image = await Image.LoadAsync(stream, cancellationToken);

        return await GetMetadataAsync(image, cancellationToken);
    }

    private async Task<ImageMetadata> GetMetadataAsync(
        Image image,
        CancellationToken cancellationToken)
    {
        var metadata =
            new ImageMetadata { Dimension = new MediaDimension(image.Height, image.Width) };

        ExifProfile? exifProfile = image.Metadata.ExifProfile;

        if (exifProfile != null)
        {
            metadata.GeoLocation = GetGeoLocation(exifProfile);
            metadata.Camera = GetCameraInfo(exifProfile);
            metadata.DateTaken = GetDateTaken(exifProfile);
            metadata.Orientation = exifProfile.GetValue(ExifTag.Orientation)?.ToString();
            metadata.ImageId = exifProfile.GetValue(ExifTag.ImageUniqueID)?.ToString();
            metadata.Lens = exifProfile.GetValue(ExifTag.LensModel)?.ToString();
        }

        return metadata;
    }

    private DateTime? GetDateTaken(ExifProfile exifProfile)
    {
        if (exifProfile.TryGetValue(ExifTag.DateTimeOriginal, out IExifValue<string>? originalDate) &&
            DateTime.TryParseExact(
                originalDate.Value,
                ExifDateFormat,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out DateTime dateTaken))
        {
            return dateTaken;
        }

        return null;
    }

    private CameraInfo? GetCameraInfo(ExifProfile exifProfile)
    {
        exifProfile.TryGetValue(ExifTag.Model, out IExifValue<string>? model);
        exifProfile.TryGetValue(ExifTag.Make, out IExifValue<string>? make);
        exifProfile.TryGetValue(ExifTag.Software, out IExifValue<string>? software);

        if (model?.GetValue() != null)
        {
            return new CameraInfo { Model = model?.ToString(), Make = make?.ToString(), Software = software?.ToString() };
        }

        return null;
    }

    private GeoLocation GetGeoLocation(ExifProfile exifProfile)
    {
        if (!exifProfile.TryGetValue(ExifTag.GPSLatitude, out IExifValue<Rational[]>? lat) ||
            !exifProfile.TryGetValue(ExifTag.GPSLongitude, out IExifValue<Rational[]>? lon) ||
            lat == null || lon == null)
        {
            return null;
        }

        var latValue = ConvertToLocation(lat.Value);
        var lonValue = ConvertToLocation(lon.Value);

        if (exifProfile.TryGetValue(ExifTag.GPSLatitudeRef, out IExifValue<string>? latRef) && latRef?.Value == "S")
        {
            latValue *= -1;
        }

        if (exifProfile.TryGetValue(ExifTag.GPSLongitudeRef, out IExifValue<string>? lonRef) && lonRef?.Value == "W")
        {
            lonValue *= -1;
        }

        var location = new GeoLocation { Latitude = latValue, Longitude = lonValue, };

        if (exifProfile.TryGetValue(ExifTag.GPSAltitude, out IExifValue<Rational>? alt) && alt != null)
        {
            var denominator = (int)alt.Value.Denominator;
            if (denominator == 0)
                denominator = 1;
            location.Altitude = (int)alt.Value.Numerator / denominator;
        }

        return location;
    }

    private double ConvertToLocation(Rational[] rational)
    {
        return Math.Round(
            rational[0].GetValue() + rational[1].GetValue() /
            60.0 + rational[2].GetValue() / 3600.0, 6);
    }
}

public static class ExifProfileExtensions
{
    public static IExifValue<TValueType> GetValue<TValueType>(
        this ExifProfile exifProfile,
        ExifTag<TValueType> tag)
    {
        return exifProfile.TryGetValue(tag, out IExifValue<TValueType>? value) ? value : null;
    }
}
