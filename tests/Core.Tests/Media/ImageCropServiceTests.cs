using System.Security.Cryptography;
using Anyding;
using Anyding.Media;
using Snapshooter.Xunit;

namespace Media.Tests;

public class ImageCropServiceTests
{
    [Fact]
    public async Task CropImages()
    {
        // Arrange
        var imageCropService = new ImageCropService(new ImagePreviewService());
        Stream testImageWithFaces = TestData.Images.FriendsJpg;

        var inputs = new List<ImageBoxCropInput>
        {
            new()
            {
                Id = Guid.Parse("10ed5e34-5369-4b0a-bd27-f82f20c0ad2c"),
                Box = new ImageRactangle { X = 1150, Y = 511, Width = 186, Height = 185 }
            },
            new()
            {
                Id = Guid.Parse("d2f4f1c4-112a-424e-bcb4-99a54d2eb032"),
                Box = new ImageRactangle { X = 1377, Y = 242, Width = 186, Height = 186 }
            },
            new()
            {
                Id = Guid.Parse("ee1ca487-f278-4e0d-a2a4-e5a19eaa11f8"),
                Box = new ImageRactangle { X = 489, Y = 201, Width = 186, Height = 186 }
            },
            new()
            {
                Id = Guid.Parse("5b466b20-79c9-4712-8c32-6692a52bc53c"),
                Box = new ImageRactangle { X = 1088, Y = 139, Width = 186, Height = 186 }
            },
            new()
            {
                Id = Guid.Parse("d63cb4ee-cd53-4863-b376-34aee846ae26"),
                Box = new ImageRactangle { X = 752, Y = 184, Width = 154, Height = 155 }
            },
            new()
            {
                Id = Guid.Parse("3bca45d1-6063-438e-b725-a81e4e5bcc83"),
                Box = new ImageRactangle { X = 737, Y = 614, Width = 186, Height = 186 }
            }
        };

        // Act
        IEnumerable<ImageBoxCropResult> result = await imageCropService.CropBoxAsync(
            testImageWithFaces,
            inputs,
            ImageBoxCropOptions.Face,
            CancellationToken.None);

        foreach (ImageBoxCropResult image in result)
        {
            var path = Path.Combine(TestData.ResultPath, $"Cropped_{image.Id}.{image.Info.Format.ToLower()}");
            await File.WriteAllBytesAsync(path, image.Image);
        }

        var sha256 = new SHA256Managed();

        result.Select(x => new { meta = x.Info, datahash = sha256.ComputeHash(x.Image) }).MatchSnapshot();
    }
}
