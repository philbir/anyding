using System.Security.Cryptography;
using Typesense;
using Type = System.Type;

namespace Anyding;

internal sealed class TypesenseDatabaseBuilder(Func<ITypesenseClient> clientFactory) : ITypesenseDatabaseBuilder
{
    private readonly List<Func<ITypesenseClient, ITypesenseCollectionType>> _collectionActions = new();

    public ITypesenseDatabaseBuilder ConfigureCollection<TDocument>(
        ITypesenseCollectionConfiguration<TDocument> configuration) where TDocument : class
    {
        Func<ITypesenseClient, ITypesenseCollectionType> func = client =>
        {
            var collectionBuilder = new TypesenseCollectionBuilder<TDocument>();
            configuration.OnConfiguring(collectionBuilder);

            var collection = collectionBuilder.Build();
            return collection;
        };

        _collectionActions.Add(func);

        return this;
    }

    public TypesenseContextData Build()
    {
        List<ITypesenseCollectionType> collections = new();

        foreach (Func<ITypesenseClient, ITypesenseCollectionType> action in _collectionActions)
        {
            ITypesenseCollectionType collection = action(clientFactory());
            collections.Add(collection);
        }

        return new TypesenseContextData(collections);
    }
}

public interface ITypesenseDatabaseBuilder
{
    ITypesenseDatabaseBuilder ConfigureCollection<TDocument>(
        ITypesenseCollectionConfiguration<TDocument> configuration) where TDocument : class;

    TypesenseContextData Build();
}
