using Anyding.Media;
using Snapshooter.Xunit;

namespace Media.Tests;

public class ImageMetadataExtractorTests
{
    [Fact]
    public async Task GetMetadata()
    {
        // Arrange
        var extractor = new ImageMetadataExtractor();

        // Act
        ImageMetadata metadata = await extractor.GetMetadataAsync(
            TestData.Images.JpgIphone15,
            CancellationToken.None);

        // Assert
        metadata.MatchSnapshot();
    }
}

