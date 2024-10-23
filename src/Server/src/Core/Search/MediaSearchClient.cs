using TS = Typesense;

namespace Anyding.Search;

public class MediaSearchClient(SearchDbContext searchDb)
{
    public async Task<SearchResult<MediaIndex>> Search<MediaIndex>(SearchRequest request) where MediaIndex : class
    {
        SearchBuilder<MediaIndex> searchBuilder = new SearchBuilder<MediaIndex>()
            .WithText(request.Text)
            .WithPageNr(request.PageNr)
            .WithPageSize(request.PageSize);

        if (request.QueryBy is { Count: > 0 })
        {
            searchBuilder.WithQueryBy(request.QueryBy.ToArray());
        }
        else
        {
            searchBuilder.WithQueryBy("name,faces.person_name,geo_names.value,tags.value");
        }
        if (request.FacetBy is { Count: > 0 })
        {
            searchBuilder.WithFacetBy(request.FacetBy.ToArray());
        }

        if (request.Filters != null)
        {
            foreach (SearchFilter filter in request.Filters)
            {
                searchBuilder.WithFilter(filter);
            }
        }

        if (request.GeoRadius != null)
        {
            searchBuilder.WithFilter("location",
                $"({request.GeoRadius.Latitude},{request.GeoRadius.Longitude}, {request.GeoRadius.Radius} km)");
            searchBuilder.WithSortBy("location({request.GeoRadius.Latitude},{request.GeoRadius.Longitude})", "asc");
        }

        searchBuilder.WithSortBy("date_taken.timestamp", "desc");

        TS.SearchParameters searchParameters = searchBuilder.Build();
        TS.SearchResult<MediaIndex> result = await searchDb.Media.SearchAsync<MediaIndex>(searchParameters);

        return new SearchResult<MediaIndex>()
        {
            PageNr = result.Page,
            TotalCount = result.OutOf,
            TotalFound = result.Found,
            Hits = result.Hits.Select(x => new SearchHit<MediaIndex>
            {
                Document = x.Document,
                VectorDistance = x.VectorDistance,
                GeoDistance =
                    x.GeoDistanceMeters?.Select(x => new GeoDistance(x.Key, x.Value))
                        .ToList()
            }).ToList(),
            Facets = result.FacetCounts.Select(x => new FacetResult
            {
                Field = x.FieldName,
                TotalValues = x.Stats.TotalValues,
                Values = x.Counts.Select(y => new FacetFieldCount { Value = y.Value, Count = y.Count }).ToList()
            }).ToList()
        };

        //searchDb.Client.Search<>()
    }
}

public class SearchRequest
{
    public string Text { get; set; } = "*";

    public int? PageNr { get; set; } = 0;

    public int? PageSize { get; set; } = 100;

    public List<SearchFilter>? Filters { get; set; } = [];

    public List<string>? QueryBy { get; set; } = [];

    public List<string>? FacetBy { get; set; } = [];

    public GeoSearchRadius? GeoRadius { get; set; }
}

public class SearchFilter
{
    public string Field { get; set; }

    public string? Comparer { get; set; } = ":";

    public string Value { get; set; }
}

public class GeoSearchRadius
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Radius { get; set; }
}

public class SearchResult<MediaIndex>
{
    public List<SearchHit<MediaIndex>> Hits { get; set; }
    public int PageNr { get; set; }
    public int TotalCount { get; set; }
    public int? TotalFound { get; set; }
    public List<FacetResult> Facets { get; set; }
}

public class SearchHit<TDocument>
{
    public TDocument Document { get; set; }
    public double? VectorDistance { get; set; }
    public IReadOnlyList<GeoDistance>? GeoDistance { get; set; }
}

public record GeoDistance(string Field, int Distance);

public class FacetResult
{
    public string Field { get; set; }
    public int TotalValues { get; set; }
    public IReadOnlyList<FacetFieldCount> Values { get; set; }
}

public class FacetFieldCount
{
    public string Value { get; set; }
    public int Count { get; set; }
}
