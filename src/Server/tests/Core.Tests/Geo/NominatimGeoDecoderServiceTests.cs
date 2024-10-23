using Anyding;
using Anyding.Geo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Snapshooter.Xunit;

namespace Core.Tests.Geo;

public class NominatimClientTests
{
    [Fact]
    public async Task GeoDecode()
    {
        // Arrange
        var builder = new TestAnydingServerBuilder();
        builder.AddGeoLocation();

        builder.Build();

        var service = new NominatimClient(
            builder.GetRequiredService<IHttpClientFactory>(),
            Mock.Of<ILogger<NominatimClient>>());

        // Act
        ReverseGeoCodeResult? result = await service.ReverseGeoCodeAsync(47.0, 8.0, CancellationToken.None);

        // Assert
        result.MatchSnapshot();
    }
}

public class TestAnydingServerBuilder : IAnydingServerBuilder
{
    public IServiceCollection Services { get; set; } = new ServiceCollection();

    public ConfigurationBuilder ConfigurationBuilder { get; set; } = new();

    public IConfiguration Configuration { get; private set; }

    public ServiceProvider Provider { get; set; }

    internal void Configure(IEnumerable<KeyValuePair<string, string?>> configuration)
    {
        ConfigurationBuilder.AddInMemoryCollection(configuration);
    }

    internal void AddPostgresTest(string connectionString)
    {
        Configure(new Dictionary<string, string?>
        {
            { "Storage:Postgres:ConnectionString", connectionString }
        });
    }

    public void BuildConfiguration()
    {
        Configuration = ConfigurationBuilder.Build();
    }

    public void Build()
    {
        Provider = Services.BuildServiceProvider();
    }

    public TService GetRequiredService<TService>()
    {
        return Provider.GetRequiredService<TService>();
    }
}
