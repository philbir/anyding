
using SixLabors.ImageSharp;

namespace Anyding.Media;

public interface IFaceDetectionService
{
    Task<IReadOnlyList<FaceDetectionResult>> DetectFacesAsync(
        Stream stream,
        CancellationToken cancellationToken);
}

public class FaceDetectionResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public ImageRactangle Box { get; set; }
    public double[]? Encoding { get; set; }
}

public class ImageBoundingBox
{
    public int Bottom { get; set; }
    public int Left { get; set; }
    public int Right { get; set; }
    public int Top { get; set; }
}

public class ImageRactangle
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public ImageBoundingBox ToImageBoundingBox()
    {
        return new ImageBoundingBox
        {
            Bottom = Y + Height,
            Left = X,
            Right = X + Width,
            Top = Y
        };
    }

    public static ImageRactangle FromBoundingBox(ImageBoundingBox box)
    {
        return new ImageRactangle
        {
            X = box.Left,
            Y = box.Top,
            Width = box.Right - box.Left,
            Height = box.Bottom - box.Top
        };
    }
}


public class ImageBoxCropInput
{
    public Guid Id { get; set; }

    public ImageRactangle? Box { get; set; }
}

public class ImageBoxCropResult
{
    public Guid Id { get; set; }

    public PreviewImageInfo Info { get; set; }

    public byte[]? Image { get; set; }
}

public interface IImageCropService
{
    Task<IEnumerable<ImageBoxCropResult>> CropBoxAsync(
        Stream stream,
        IEnumerable<ImageBoxCropInput> inputs,
        ImageBoxCropOptions options,
        CancellationToken cancellationToken);

    Task<IEnumerable<ImageBoxCropResult>> CropBoxAsync(
        Image image,
        IEnumerable<ImageBoxCropInput> inputs,
        ImageBoxCropOptions options,
        CancellationToken cancellationToken);
}


public class ImageBoxCropOptions
{
    public double AddRatio { get; set; } = 1;
    public double BottomOffset { get; set; } = 0;

    public PreviewImageSizeName PreviewImageSize { get; set; } = PreviewImageSizeName.Original;

    public static ImageBoxCropOptions Face => new ImageBoxCropOptions
    {
        AddRatio = 0.98, BottomOffset = 0.3, PreviewImageSize = PreviewImageSizeName.M
    };
}

public interface IImagePreviewService
{
    Task<CreatePreviewImageResult> CreatePreviewAsync(
        Image image,
        PreviewImageSizeName size,
        CancellationToken cancellationToken);
}

public class PreviewImageInfo
{
    public PreviewImageSizeName Size { get; set; }

    public MediaDimension Dimension { get; set; }

    public string Format { get; set; }
}

public class PreviewImageSizeDefinition
{
    public PreviewImageSizeName Name { get; set; }
    public int Width { get; set; }

    public string Format { get; set; } = "WebP";
    public bool IsSquare { get; set; }
    public int Quality { get; set; } = 75;

    public static IEnumerable<PreviewImageSizeDefinition> Defaults =>
        new List<PreviewImageSizeDefinition>
        {
            new() { Name = PreviewImageSizeName.Xxxs, Width = 10, IsSquare = false},
            new() { Name = PreviewImageSizeName.Xxs, Width = 20, IsSquare = false },
            new() { Name = PreviewImageSizeName.Xs, Width = 40, IsSquare = false },
            new() { Name = PreviewImageSizeName.S, Width = 120, IsSquare = false },
            new() { Name = PreviewImageSizeName.M, Width = 240, IsSquare = false },
            new() { Name = PreviewImageSizeName.L, Width = 320, IsSquare = false },
            new() { Name = PreviewImageSizeName.Xl, Width = 1024, IsSquare = false },
            new() { Name = PreviewImageSizeName.Xxl, Width = 2000, IsSquare = false },
            new() { Name = PreviewImageSizeName.Original, IsSquare = false, Quality = 50},
            new() { Name = PreviewImageSizeName.SqS, Width = 120, IsSquare = true },
            new() { Name = PreviewImageSizeName.SqXs, Width = 40, IsSquare = true },
        };
}

public enum PreviewImageSizeName
{
    Original,
    Xxxs,
    Xxs,
    Xs,
    S,
    M,
    L,
    Xl,
    Xxl,
    SqXs,
    SqS
}

public record CreatePreviewImageResult(PreviewImageInfo Info, Stream Preview);

public record MediaDimension(int Height, int Width);
