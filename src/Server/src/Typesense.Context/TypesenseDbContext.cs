using Microsoft.Extensions.Logging;
using Typesense;

namespace Anyding;

public abstract class TypesenseDbContext(ITypesenseClient typesenseClient, ILoggerFactory loggerFactory) : ITypesenseDbContext
{
    private TypesenseContextData _contextData;
    private ILogger<TypesenseDbContext> _logger = loggerFactory.CreateLogger<TypesenseDbContext>();

    protected abstract void OnConfiguring(ITypesenseDatabaseBuilder typesenseDatabaseBuilder);

    public async Task EnsureCreatedAsync()
    {
        ITypesenseDatabaseBuilder builder = new TypesenseDatabaseBuilder(typesenseClient);
        OnConfiguring(builder);
        _contextData = builder.Build();

        List<CollectionResponse> allCollections = await typesenseClient.RetrieveCollections();

        foreach (var collectionType in _contextData.Collections)
        {
            Schema schema = collectionType.Schema;
            if (allCollections.All(c => c.Name != schema.Name))
            {
                await typesenseClient.CreateCollection(schema);
            }
        }
    }

    public async Task EnsureDeletedAsync()
    {
        List<CollectionResponse> allCollections = await typesenseClient.RetrieveCollections();

        foreach (CollectionResponse collection in allCollections)
        {
            await typesenseClient.DeleteCollection(collection.Name);
        }
    }

    protected ITypesenseCollection<TDocument> GetCollection<TDocument>() where TDocument : class
    {
        var typeName = typeof(TDocument).FullName;

        ITypesenseCollectionType? collectionType = _contextData.Collections.FirstOrDefault(c => c.TypeName == typeName);

        if (collectionType is null)
        {
            throw new InvalidOperationException($"Collection for type {typeName} not found.");
        }

        return new TypesenseCollection<TDocument>(collectionType.Schema.Name, typesenseClient, loggerFactory);
    }
}

public interface ITypesenseDbContext
{
}

