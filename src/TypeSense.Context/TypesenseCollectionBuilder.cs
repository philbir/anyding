using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Typesense;

namespace Anyding;

public class TypesenseCollectionBuilder<TDocument> : ITypesenseCollectionBuilder<TDocument>
{
    string _name;
    List<Field> _fields = new();
    private bool _enableNestedFields;

    public ITypesenseCollectionBuilder<TDocument> WithName(string name)
    {
        _name = name;
        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> EnableNestedFields()
    {
        _enableNestedFields = true;
        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> AddField(Field field)
    {
        _fields.Add(field);
        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> AddField<TProperty>(Expression<Func<TDocument, TProperty>> propertyExpression)
    {
        _fields.Add(new Field(propertyExpression.Name, FieldType.Auto));

        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> AddField<TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression,
        FieldType type,
        bool facet = true,
        bool optional = false)
    {
        _fields.Add(new Field(CreateFieldName(propertyExpression), type, facet, optional));

        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> AddField<TProperty>(Expression<Func<TDocument, TProperty>> propertyExpression,
        Field field)
    {
        _fields.Add(field with { Name = CreateFieldName(propertyExpression) });

        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> AddField(string name, FieldType type, bool facet = true,
        bool optional = false)
    {
        _fields.Add(new Field(name, type, facet, optional));
        return this;
    }

    public ITypesenseCollectionBuilder<TDocument> AddField(string name, FieldType type)
    {
        _fields.Add(new Field(name, type));
        return this;
    }

    public ITypesenseCollectionType Build()
    {
        var collection = new TypesenseCollectionType(new Schema(_name, _fields) { EnableNestedFields = _enableNestedFields}, typeof(TDocument).FullName);
        return collection;
    }

    private string CreateFieldName<TProperty>(Expression<Func<TDocument, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return Regex.Replace(memberExpression.Member.Name, "(?<!^)([A-Z])", "_$1").ToLower();
        }

        throw new InvalidOperationException($"Invalid expression: {propertyExpression}");

        //return string.Concat(propertyExpression.Name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + char.ToLower(x) : char.ToLower(x).ToString()));
    }
}

public class TypesenseCollectionType : ITypesenseCollectionType
{
    public TypesenseCollectionType(Schema schema, string typeName)
    {
        Schema = schema;
        TypeName = typeName;
    }

    public Schema Schema { get; private set; }
    public string TypeName { get; }
}

public interface ITypesenseCollectionType
{
    public string TypeName { get; }
    public Schema Schema { get; }
}

public interface ITypesenseCollectionBuilder<TDocument>
{
    ITypesenseCollectionBuilder<TDocument> WithName(string name);

    ITypesenseCollectionBuilder<TDocument> EnableNestedFields();
    ITypesenseCollectionBuilder<TDocument> AddField(Field field);

    ITypesenseCollectionBuilder<TDocument> AddField(string name, FieldType type, bool facet = true,
        bool optional = false);

    public ITypesenseCollectionBuilder<TDocument> AddField<TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression);

    public ITypesenseCollectionBuilder<TDocument> AddField<TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression,
        FieldType type,
        bool facet = true,
        bool optional = false);

    public ITypesenseCollectionBuilder<TDocument> AddField<TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression,
        Field field);
}
