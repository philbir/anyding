using Anyding;
using Anyding.Data;
using Anyding.Geo;
using Microsoft.Extensions.Logging;
using Moq;
using Snapshooter.Xunit;
using Squadron;

namespace Core.Tests.Geo;

public class GeoCodeServiceServiceTests(PostgreSqlResource postgreSqlResource) : IClassFixture<PostgreSqlResource>
{
    [Fact]
    public async Task Reverse_WithCache()
    {
        // Arrange
        var builder = new TestAnydingServerBuilder();
        builder.AddPostgresTest(postgreSqlResource.ConnectionString);
        builder.BuildConfiguration();

        builder.AddPostgresStore();
        builder.AddGeoLocation();

        builder.Build();

        var dbContext = builder.GetRequiredService<AnydingDbContext>();

        await dbContext.Database.EnsureCreatedAsync();

        var service = new GeoDecoderService(
            builder.GetRequiredService<IGeoReverseEncodingCacheStore>(),
            builder.GetRequiredService<IEnumerable<IGeoCodingSource>>(),
            Mock.Of<ILogger<GeoDecoderService>>());

        double latitude = 9.775833;
        double longitude = 99.966389;

        // Act
        var result = await service.ReverseAsync(latitude, longitude, CancellationToken.None);

        var resul2 = await service.ReverseAsync(latitude, longitude, CancellationToken.None);

        // Assert
        result.MatchSnapshot();
    }
}
