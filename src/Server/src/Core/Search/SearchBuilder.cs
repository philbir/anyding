using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Anyding.Search;

public class SearchBuilder<TDocument> where TDocument : class
{
    private readonly List<SearchFilter> _filters = [];
    private readonly List<string> _queryBy = [];
    private readonly List<string> _facetBy = [];
    private readonly List<string> _sortBy = [];
    private string _text = "*";
    private int? _pageNr = 0;
    private int? _pageSize = 100;

    public SearchBuilder<TDocument> WithText(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            _text = text;
        }

        return this;
    }

    public SearchBuilder<TDocument> WithPageNr(int? pageNr)
    {
        _pageNr = pageNr.Value;
        return this;
    }

    public SearchBuilder<TDocument> WithPageSize(int? pageSize)
    {
        _pageSize = pageSize.Value;
        return this;
    }

    public SearchBuilder<TDocument> WithFilter<TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression,
        string value,
        string comparer = ":")
    {
        return WithFilter(CreateFieldName(propertyExpression), value, comparer);
    }

    public SearchBuilder<TDocument> WithFilter(SearchFilter filter)
    {
        _filters.Add(filter);
        return this;
    }

    public SearchBuilder<TDocument> WithFilter(
        string fieldName,
        string value,
        string comparer = ":")
    {
        _filters.Add(new SearchFilter { Field = fieldName, Value = value, Comparer = comparer });

        return this;
    }

    public SearchBuilder<TDocument> WithQueryBy(params string[] fieldNames)
    {
        _queryBy.AddRange(fieldNames);
        return this;
    }

    public SearchBuilder<TDocument> WithQueryBy<TProperty>(Expression<Func<TDocument, TProperty>> propertyExpression)
    {
        _queryBy.Add(FieldNameResolver.CreateFieldName(propertyExpression));
        return this;
    }

    public SearchBuilder<TDocument> WithSortBy(string fieldName, string direction = "asc")
    {
        _sortBy.Add($"{fieldName}:{direction}");
        return this;
    }

    public SearchBuilder<TDocument> WithSortBy<TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression,
        string direction = "asc")
    {
        _sortBy.Add($"{FieldNameResolver.CreateFieldName(propertyExpression)}:{direction}");
        return this;
    }

    public SearchBuilder<TDocument> WithFacetBy(params string[] fieldNames)
    {
        _facetBy.AddRange(fieldNames);
        return this;
    }

    public SearchBuilder<TDocument> WithFacetBy<TProperty>(Expression<Func<TDocument, TProperty>> propertyExpression)
    {
        _facetBy.Add(FieldNameResolver.CreateFieldName(propertyExpression));
        return this;
    }

    private string CreateFieldName<TProperty>(Expression<Func<TDocument, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return Regex.Replace(memberExpression.Member.Name, "(?<!^)([A-Z])", "_$1").ToLower();
        }

        throw new InvalidOperationException($"Invalid expression: {propertyExpression}");
    }

    public Typesense.SearchParameters Build()
    {
        var searchPars = new Typesense.SearchParameters();

        if (_pageNr is { } nr)
        {
            searchPars.Page = nr;
        }

        if (_pageSize is { } size)
        {
            searchPars.PerPage = size;
        }

        searchPars.Text = _text;
        searchPars.FilterBy = string.Join("&&", _filters.Select(x => $"{x.Field}{x.Comparer}{x.Value}"));
        searchPars.QueryBy = string.Join(",", _queryBy);
        searchPars.FacetBy = string.Join(",", _facetBy);

        return searchPars;
    }
}
