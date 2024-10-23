using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Anyding;

public static class FieldNameResolver
{
    public static string CreateFieldName<TDocument, TProperty>(
        Expression<Func<TDocument, TProperty>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExpression)
        {
            return Regex.Replace(memberExpression.Member.Name, "(?<!^)([A-Z])", "_$1").ToLower();
        }

        throw new InvalidOperationException($"Invalid expression: {propertyExpression}");
    }
}
