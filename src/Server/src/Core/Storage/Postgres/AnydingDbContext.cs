using Anyding.Connectors;
using Anyding.Geo;
using Anyding.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Anyding.Data;

public interface IAnydingDbContext
{
    DbSet<Thing> Things { get; }
    DbSet<ThingConnection> ThingConnections { get; }
    DbSet<ThingClass> ThingClasses { get; }
    DbSet<TagDefinition> TagDefinitions { get; }
    DbSet<ThingIdentifier> ThingIdentifiers { get; }
    DbSet<ThingDataReference> ThingData { get; }
    DbSet<DataFile> Files { get; }
    DbSet<ThingTag> ThingTags { get; }
    DbSet<JobRun> JobRuns { get; }
    DbSet<JobRunStep> JobRunSteps { get; }
    DbSet<JobDefintion> JobDefinitions { get; }
    DbSet<GeoReverseEncodingCache> GeoReverseEncodings { get; }
    DbSet<ConnectorDefinition> ConnectorDefinitions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class AnydingDbContext(DbContextOptions<AnydingDbContext> options) : DbContext(options), IAnydingDbContext
{
    public DbSet<Thing> Things => Set<Thing>();
    public DbSet<ThingConnection> ThingConnections { get; init; }
    public DbSet<ThingClass> ThingClasses { get; init; }
    public DbSet<TagDefinition> TagDefinitions { get; init; }

    public DbSet<ThingIdentifier> ThingIdentifiers { get; init; }
    public DbSet<ThingDataReference> ThingData => Set<ThingDataReference>();
    public DbSet<DataFile> Files => Set<DataFile>();
    public DbSet<ThingTag> ThingTags { get; init; }
    public DbSet<JobRun> JobRuns { get; init; }
    public DbSet<JobRunStep> JobRunSteps => Set<JobRunStep>();

    public DbSet<JobDefintion> JobDefinitions { get; init; }
    public DbSet<GeoReverseEncodingCache> GeoReverseEncodings => Set<GeoReverseEncodingCache>();
    public DbSet<ConnectorDefinition> ConnectorDefinitions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnydingDbContext).Assembly);
        modelBuilder.IgnoreEvents();
    }

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>()
            .HavePrecision(18, 6);
    }
}

internal static class ModelBuilderExtensions
{
    internal static void IgnoreEvents(this ModelBuilder modelBuilder)
    {
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (entityType.ClrType.BaseType?.BaseType == typeof(Entity))
            {
                modelBuilder.Entity(entityType.ClrType).Ignore(nameof(Entity.Events));
            }
        }
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

public interface IEntity<TId>
{
    TId Id { get; set; }
}

public static class DbContextExtensions
{
    public static async Task<IReadOnlyList<TEntity>> QueryByIdsAsync<TEntity, TId>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TId> ids,
        CancellationToken ct = default) where TEntity : class, IEntity<TId>
    {
        return await dbSet
            .Where(x => ids.Contains(x.Id))
            .ToListAsync(ct);
    }

    public static async Task<TEntity> QueryByIdAsync<TEntity, TId>(
        this DbSet<TEntity> dbSet,
        TId id,
        CancellationToken ct = default) where TEntity : class, IEntity<TId>
    {
        return await dbSet
            .Where(x => x.Id.Equals(id))
            .FirstOrDefaultAsync(ct);
    }
}
