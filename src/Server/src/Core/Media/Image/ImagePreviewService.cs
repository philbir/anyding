using Anyding.Media;
using ImageMagick;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Anyding;

public class ImagePreviewService : IImagePreviewService
{
    public async Task<CreatePreviewImageResult> CreatePreviewAsync(
        Image image,
        PreviewImageSizeName size,
        CancellationToken cancellationToken)
    {
        if (image == null) throw new ArgumentNullException(nameof(image));

        image.Metadata.ExifProfile = null;
        image.Metadata.IptcProfile = null;

        PreviewImageSizeDefinition def = PreviewImageSizeDefinition.Defaults.Single(x => x.Name == size);
        Image resized = null;

        if (def.Width == 0)
        {
            resized = image.Clone(_ => { });
        }
        else
        {
            if (def.IsSquare)
            {
                image = CropSquare(image);
            }

            var originalWidth = image.Width;
            var targetWidth = Math.Min(def.Width, originalWidth);
            var ratio = (double)originalWidth / targetWidth;
            var targetHeight = (int)(image.Height / ratio);

            resized = image.Clone(ctx => ctx.Resize(targetWidth, targetHeight));
        }

        using var preview = new MemoryStream();

        /*
        ImageSharp slow a hell for WebP on ARM
        https://github.com/SixLabors/ImageSharp/issues/2125
        await resized.SaveAsync(preview, new WebpEncoder
        {
            Quality = def.Quality,
            SkipMetadata = true,
            FileFormat = WebpFileFormatType.Lossy
        }, cancellationToken);*/

        await resized.SaveAsync(preview, new JpegEncoder(), cancellationToken);
        preview.Position = 0;
        Stream webPStream = ConvertToWebP(preview, def.Quality);

        var info = new PreviewImageInfo
        {
            Size = size, Format = "WebP", Dimension = new MediaDimension(resized.Height, resized.Width)
        };

        preview.Dispose();

        return new CreatePreviewImageResult(info, webPStream);
    }

    private Image CropSquare(Image image)
    {
        Rectangle rect;
        if (image.GetOrientation() == MediaOrientation.Landscape)
        {
            var toRem = (image.Width - image.Height);
            rect = new Rectangle(toRem / 2, 0, image.Width - toRem, image.Height);
        }
        else
        {
            var toRem = (image.Height - image.Width);
            rect = new Rectangle(0, toRem / 2, image.Width, image.Height - toRem);
        }

        return image.Clone(x => x.Crop(rect));
    }

    private Stream ConvertToWebP(Stream stream, int quality)
    {
        using var image = new MagickImage(stream);
        image.Quality = (uint)quality;
        image.Format = MagickFormat.WebP;
        var outputStream = new MemoryStream();

        image.Write(outputStream);

        outputStream.Position = 0;
        return outputStream;
    }
}

public static class ImageExtensions
{
    public static MediaOrientation GetOrientation(this Image image)
    {
        return (image.Width > image.Height) ? MediaOrientation.Landscape : MediaOrientation.Portrait;
    }
}
