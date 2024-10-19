using Typesense;

namespace Anyding.Search;

public class SearchDbContext(Func<ITypesenseClient> clientFactory) : TypesenseDbContext(clientFactory)
{
    public ITypesenseCollection<MediaIndex> Media => GetCollection<MediaIndex>();

    public ITypesenseCollection<FaceIndex> Faces => GetCollection<FaceIndex>();

    public ITypesenseCollection<PersonIndex> Persons => GetCollection<PersonIndex>();

    protected override void OnConfiguring(ITypesenseDatabaseBuilder builder)
    {
        builder.ConfigureCollection(new MediaCollectionConfiguration());
        builder.ConfigureCollection(new FaceCollectionConfiguration());
        builder.ConfigureCollection(new PersonCollectionConfiguration());
    }
}



