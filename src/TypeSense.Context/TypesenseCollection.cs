using Typesense;

namespace Anyding;

internal sealed class TypesenseCollection<TDocument> : ITypesenseCollection<TDocument> where TDocument : class
{
    private readonly ITypesenseClient _client;

    public TypesenseCollection(string name, ITypesenseClient client)
    {
        Name = name;
        _client = client;
    }

    public string Name { get; set; }

    public async Task CreateDocument<TDocument>(TDocument document) where TDocument : class
    {
        await _client.CreateDocument(Name, document);
    }

    public async Task CreateDocuments<TDocument>(IEnumerable<TDocument> documents, int batchSize = 40) where TDocument : class
    {
        var responses = await _client.ImportDocuments(Name, documents, batchSize:batchSize);
    }
}

public interface ITypesenseCollection<TDocument> where TDocument : class
{
    Task CreateDocument<TDocument>(TDocument document) where TDocument : class;
    Task CreateDocuments<TDocument>(IEnumerable<TDocument> documents, int batchSize = 50) where TDocument : class;
    string Name { get; set; }
}
