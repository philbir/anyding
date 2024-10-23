using Anyding.Discovery;
using Anyding.Geo;
using Microsoft.EntityFrameworkCore;

namespace Anyding.Data;

public class AnydingDbContext(DbContextOptions<AnydingDbContext> options) : DbContext(options)
{
    public DbSet<Thing> Things { get; init; }

    public DbSet<ThingConnection> ThingConnections { get; init; }

    public DbSet<ThingClass> ThingClasses { get; init; }

    public DbSet<TagDefinition> TagDefinitions { get; init; }

    public DbSet<ThingIdentifier> ThingIdentifiers { get; init; }

    public DbSet<ThingStorageReference> ThingData { get; init; }

    public DbSet<ThingTag> ThingTags { get; init; }

    public DbSet<CollectorJob> CollectorJobs { get; init; }

    public DbSet<GeoReverseEncodingCache> GeoReverseEncodings { get; init; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        /*
        modelBuilder.ApplyConfiguration(new ThingEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ThingTypeEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ThingConnectionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ThingClassEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TagDefinitionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ThingTagEntityConfiguration());
        modelBuilder.ApplyConfiguration(new GeoReverseEncodingCacheEntityConfiguration());*/
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnydingDbContext).Assembly);
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 6);
    }
}

public class AnydingDbContextTest(DbContextOptions<AnydingDbContextTest> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tt");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("users");

            entity.OwnsOne(e => e.Details, details =>
            {
                details.ToJson();
            });
        });
    }
}

public class User
{
    public string Id { get; set; }

    public string Name { get; set; }

    public UserDetails Details { get; set; }
}

public class UserDetails
{
    public string Email { get; set; }
    public string Phone { get; set; }
}
