using Anyding.Search;

namespace Anyding.Api;

public static class MediaOperations
{
    [Query]
    public static async Task<SearchResult<MediaIndex>> SearchMediaAsync(
        SearchRequest request,
        [Service] MediaSearchClient search,
        CancellationToken ct)
    {
        var result = await search.Search<MediaIndex>(request);
        return result;
    }
}
