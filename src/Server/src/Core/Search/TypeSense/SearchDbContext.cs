using Microsoft.Extensions.Logging;
using Typesense;

namespace Anyding.Search;

public class SearchDbContext(ITypesenseClient typesenseClient, ILoggerFactory loggerFactory) : TypesenseDbContext(typesenseClient, loggerFactory)
{
    public ITypesenseCollection<MediaIndex> Media => GetCollection<MediaIndex>();

    public ITypesenseCollection<FaceIndex> Faces => GetCollection<FaceIndex>();

    public ITypesenseCollection<PersonIndex> Persons => GetCollection<PersonIndex>();

    public ITypesenseClient Client => typesenseClient;

    protected override void OnConfiguring(ITypesenseDatabaseBuilder builder)
    {
        builder.ConfigureCollection(new MediaCollectionConfiguration());
        builder.ConfigureCollection(new FaceCollectionConfiguration());
        builder.ConfigureCollection(new PersonCollectionConfiguration());
    }
}



