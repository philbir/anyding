namespace Anyding;

public sealed class TypesenseContextData
{
    public TypesenseContextData(List<ITypesenseCollectionType> collections)
    {
        Collections = collections;
    }

    public List<ITypesenseCollectionType> Collections { get; }
}
