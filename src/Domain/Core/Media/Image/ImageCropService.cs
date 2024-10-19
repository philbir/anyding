using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Anyding.Media;

public class ImageCropService(IImagePreviewService previewService) : IImageCropService
{
    public async Task<IEnumerable<ImageBoxCropResult>> CropBoxAsync(
        Stream stream,
        IEnumerable<ImageBoxCropInput> inputs,
        ImageBoxCropOptions options,
        CancellationToken cancellationToken)
    {
        Image image = await Image.LoadAsync(stream, cancellationToken);

        return await CropBoxAsync(image, inputs, options, cancellationToken);
    }

    public async Task<IEnumerable<ImageBoxCropResult>> CropBoxAsync(
        Image image,
        IEnumerable<ImageBoxCropInput> inputs,
        ImageBoxCropOptions options,
        CancellationToken cancellationToken)
    {
        var results = new List<ImageBoxCropResult>();

        foreach (ImageBoxCropInput input in inputs)
        {
            try
            {
                input.Box = ApplyOptions(input.Box, options, new MediaDimension(image.Height, image.Width));

                ImageBoxCropResult face = await CropBoxAsync(
                    image,
                    input,
                    options.PreviewImageSize,
                    cancellationToken);

                results.Add(face);
            }
            catch (Exception ex)
            {
                // Log error
                Console.WriteLine("Error cropping image" + ex.Message);
            }
        }

        return results;
    }

    private async Task<ImageBoxCropResult> CropBoxAsync(
        Image image,
        ImageBoxCropInput input,
        PreviewImageSizeName previewSize,
        CancellationToken cancellationToken)
    {
        var rect = new Rectangle(
            input.Box.X,
            input.Box.Y,
            input.Box.Width,
            input.Box.Height);

        Image cropped = image.Clone(x => x.Crop(rect));

        CreatePreviewImageResult croppedImage = await previewService.CreatePreviewAsync(
            cropped,
            previewSize,
            cancellationToken);

        return new ImageBoxCropResult
        {
            Id = input.Id, Info = croppedImage.Info, Image = croppedImage.Preview.ToByteArray()
        };
    }

    private ImageRactangle ApplyOptions(
        ImageRactangle box,
        ImageBoxCropOptions options,
        MediaDimension size)
    {
        var bottomOffset = (int)(box.Height * options.BottomOffset);

        var widthAdd = (int)(box.Width * options.AddRatio / 2);
        var heightAdd = (int)(box.Height * options.AddRatio / 2) + bottomOffset;

        var newBox = new ImageRactangle()
        {
            Width = box.Width + widthAdd,
            Height = box.Height + heightAdd,
            X = box.X - widthAdd / 2,
            Y = box.Y - (heightAdd / 2) - bottomOffset
        };

        //Ensure the box is within the image bounds
        if (newBox.X < 0)
        {
            newBox.Width += newBox.X;
            newBox.X = 0;
        }
        if (newBox.Y < 0)
        {
            newBox.Height += newBox.Y;
            newBox.Y = 0;
        }
        if (newBox.X + newBox.Width > size.Width)
        {
            newBox.Width = size.Width - newBox.X;
        }
        if (newBox.Y + newBox.Height > size.Height)
        {
            newBox.Height = size.Height - newBox.Y;
        }

        return newBox;
    }

}
