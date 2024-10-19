namespace Anyding;

public interface ITypesenseCollectionConfiguration<TDocument> where TDocument : class
{
    void OnConfiguring(ITypesenseCollectionBuilder<TDocument> builder);
}
