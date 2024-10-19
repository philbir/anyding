using Anyding;
using Anyding.Data;
using Anyding.Data.Postgres;
using Microsoft.EntityFrameworkCore;
using Snapshooter.Xunit;

namespace Store.Tests;

public class ThingStoreTests /*: IClassFixture<PostgreSqlResource>*/
{
    [Fact]
    public async Task AddThing()
    {
        var connectionString = "Host=localhost;Port=5432;Database=anyding;Username=postgres;Password=postgres";
        // Arrange
        DbContextOptions<AnydingDbContext> options = new DbContextOptionsBuilder<AnydingDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new AnydingDbContext(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var store = new ThingStore(context);

        var imageClass = context.ThingClasses.FirstOrDefault(x => x.Name == "Image");

        var tag1 = context.TagDefinitions.FirstOrDefault(x => x.Name == "Tag1");

        /*
        var placeDetail = new PlaceDetails
        {
            Name = "Quaibrücke",
            Address = "Quaibrücke",
            City = "Test City",
            Region = "Zurich",
            Locality = "Zurich",
            LocalAdmin = "Zurich",
            CountryCode = "CH",
            Continent = "Europe",
            Label = "Quaibrücke, Zurich, Switzerland",
            Latitude = 47.366667,
            Longitude = 8.543056,
            Altitude = 460
        };


        var placeThing = new Thing()
        {
            Id = Guid.NewGuid(),
            Name = placeDetail.Label,
            Type = placeType,
            Detail = placeDetail.ToJson2(),
            Created = DateTime.Now,
        };*/

        var imageThing = new Thing()
        {
            Id = Guid.NewGuid(),
            Name = "Foobar.jpg",
            Type = ThingType.Media,
            Class = imageClass,
            Created = DateTime.Now,
            Source = new ThingSource() { ConnectorId = Guid.NewGuid(), SourceId = "Foobar.jpg", FullLocation = "user/foo/FooBar.jpg"},
            Tags =
            [
                new() { Id = Guid.NewGuid(), DefinitionId = tag1.Id, Value = "Test Tag" }
            ]
        };

        var phoneThing = new Thing
        {
            Id = Guid.NewGuid(),
            Name = "iPhone 15 Pro Max",
            Type = ThingType.Device,
            Created = DateTime.Now,
        };

        var personThing = new Thing
        {
            Id = Guid.NewGuid(),
            Name = "Bart Simpson",
            Created = DateTime.Now,
            Type = ThingType.Person,
        };

        /*
        var placeTakenConnection = new ThingConnection
        {
            Id = Guid.NewGuid(),
            Type = "PlaceTaken",
            From = imageThing,
            To = placeThing,
        };*/

        var deviceConnection = new ThingConnection
        {
            Id = Guid.NewGuid(),
            Type = "TakenBy",
            From = imageThing,
            To = phoneThing,
        };

        var personConnection = new ThingConnection
        {
            Id = Guid.NewGuid(),
            Type = "InMedia",
            From = imageThing,
            To = personThing,
        };

        // Act
        context.Things.Add(personThing);
        //context.Things.Add(placeThing);
        context.Things.Add(imageThing);
        context.Things.Add(phoneThing);
        //context.ThingConnections.Add(placeTakenConnection);
        context.ThingConnections.Add(deviceConnection);
        context.ThingConnections.Add(personConnection);

        await context.SaveChangesAsync();

        var createdImageThing = context.Things
            .Include(x => x.Connections)
            .Include(x => x.Type)
            .Include(x => x.Class)
            .SingleOrDefault(x => x.Id == imageThing.Id);

        createdImageThing.MatchSnapshot();
/*
        var resultPerson = await store.AddAsync(personThing, CancellationToken.None);
        var resultPlace = await store.AddAsync(placeThing, CancellationToken.None);*/
    }

    [Fact]
    public async Task Test2()
    {
        var connectionString = "Host=localhost;Port=5432;Database=anyding2;Username=postgres;Password=postgres";
        // Arrange
        DbContextOptions<AnydingDbContextTest> options = new DbContextOptionsBuilder<AnydingDbContextTest>()
        .UseNpgsql(connectionString)
        .Options;

        await using var context = new AnydingDbContextTest(options);
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test User",
            Details = new UserDetails { Email = "ff", Phone = "ddd" }
        };

        context.Add(user);
        await context.SaveChangesAsync();
    }
}
