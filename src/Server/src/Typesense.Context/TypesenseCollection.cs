using Microsoft.Extensions.Logging;
using Typesense;

namespace Anyding;

internal sealed class TypesenseCollection<TDocument> : ITypesenseCollection<TDocument> where TDocument : class
{
    private readonly ITypesenseClient _client;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TypesenseCollection<TDocument>> _logger;

    public TypesenseCollection(string name, ITypesenseClient client, ILoggerFactory loggerFactory)
    {
        Name = name;
        _client = client;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<TypesenseCollection<TDocument>>();
    }

    public string Name { get; set; }

    public async Task CreateDocument<TDocument>(TDocument document) where TDocument : class
    {
        await _client.CreateDocument(Name, document);
    }

    public async Task CreateDocuments<TDocument>(IEnumerable<TDocument> documents, int batchSize = 40) where TDocument : class
    {
        _logger.LogInformation("Importing documents to collection {CollectionName}. Count: {Count}", Name, documents.Count());
        List<ImportResponse> responses = await _client.ImportDocuments(Name, documents, batchSize:batchSize, ImportType.Upsert);

        IEnumerable<ImportResponse> errors = responses.Where(x => !x.Success);
        if (errors.Any())
        {
            _logger.LogWarning("Failed to import documents to collection {CollectionName}. Errors count: {Count}", Name, errors.Count());
        }
    }

    public async Task<SearchResult<TDocument>> SearchAsync<TDocument>(SearchParameters search) where TDocument : class
    {
        return  await _client.Search<TDocument>(Name, search);
    }
}

public interface ITypesenseCollection<TDocument> where TDocument : class
{
    Task<SearchResult<TDocument>> SearchAsync<TDocument>(SearchParameters search) where TDocument : class;
    Task CreateDocument<TDocument>(TDocument document) where TDocument : class;
    Task CreateDocuments<TDocument>(IEnumerable<TDocument> documents, int batchSize = 50) where TDocument : class;
    string Name { get; set; }
}
