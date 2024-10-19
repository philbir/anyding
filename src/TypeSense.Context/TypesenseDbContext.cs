using Typesense;

namespace Anyding;

public abstract class TypesenseDbContext(Func<ITypesenseClient> clientFactory)
{
    private TypesenseContextData _contextData;
    protected abstract void OnConfiguring(ITypesenseDatabaseBuilder typesenseDatabaseBuilder);

    public async Task EnsureCreatedAsync()
    {
        ITypesenseDatabaseBuilder builder = new TypesenseDatabaseBuilder(clientFactory);
        OnConfiguring(builder);
        _contextData = builder.Build();

        var client = clientFactory();

        var allCollections = await client.RetrieveCollections();

        foreach (var collectionType in _contextData.Collections)
        {
            Schema schema = collectionType.Schema;
            if (allCollections.All(c => c.Name != schema.Name))
            {
                await client.CreateCollection(schema);
            }
        }
    }

    public async Task EnsureDeletedAsync()
    {
        ITypesenseClient client = clientFactory();
        List<CollectionResponse> allCollections = await client.RetrieveCollections();

        foreach (var collection in allCollections)
        {
            await client.DeleteCollection(collection.Name);
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

        return new TypesenseCollection<TDocument>(collectionType.Schema.Name, clientFactory());
    }
}

public interface ITypesenseDbContext
{
}

