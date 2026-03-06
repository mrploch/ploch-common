using Microsoft.Extensions.Primitives;

namespace Ploch.Common.WebApi;

/// <summary>
/// A static helper class for binding HTTP query string parameters to an instance of a specified type.
/// </summary>
public static class QueryStringBinder
{
    /// <summary>
    /// Binds HTTP query string parameters from the given HttpContext to a new instance of the specified type.
    /// </summary>
    /// <typeparam name="TQuery">The type to which the query string parameters will be bound. Must have a parameterless constructor.</typeparam>
    /// <param name="httpContext">The HTTP context containing the query string parameters to bind.</param>
    /// <returns>An instance of type <typeparamref name="TQuery"/> populated with values from the query string.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when a property of type <typeparamref name="TQuery"/> has a type that is not supported for query string binding.
    /// </exception>
    public static TQuery Bind<TQuery>(HttpContext httpContext)
        where TQuery : new()
    {
        var query = httpContext.Request.Query;

        if (TryParse<TQuery>(query.ToDictionary(q => q.Key, q => q.Value), out var queryInstance))
        {
            return queryInstance;
        }

        throw new NotSupportedException($"The type {typeof(TQuery).Name} contains a property with an unsupported type for query string binding.");
    }

    /// <summary>
    /// Attempts to parse the specified query string parameters into an instance of the given type.
    /// </summary>
    /// <typeparam name="TQuery">The type into which the query string parameters will be parsed. Must have a parameterless constructor.</typeparam>
    /// <param name="query">A dictionary containing the query string parameters, using the property names as keys.</param>
    /// <param name="queryInstance">
    /// When this method returns, contains an instance of type <typeparamref name="TQuery"/>
    /// populated with the values from the query string if parsing was successful, or the default value if parsing failed.
    /// </param>
    /// <returns>
    /// True if the query string parameters were successfully parsed into an instance of <typeparamref name="TQuery"/>; otherwise, false.
    /// </returns>
    public static bool TryParse<TQuery>(IDictionary<string, StringValues> query, out TQuery queryInstance)
        where TQuery : new()
    {
        var queryType = typeof(TQuery);
        var queryProperties = queryType.GetProperties();
        queryInstance = new TQuery();
        foreach (var property in queryProperties)
        {
            var queryValue = query[property.Name];
            if (queryValue.Count == 0)
            {
                continue;
            }

            var firstValue = queryValue[0];
            var propertyType = property.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyType.GetGenericArguments()[0];
            }

            if (propertyType == typeof(string))
            {
                property.SetValue(queryInstance, firstValue);
            }
            else if (propertyType == typeof(int))
            {
                property.SetValue(queryInstance, int.Parse(firstValue));
            }
            else if (propertyType == typeof(bool))
            {
                property.SetValue(queryInstance, bool.Parse(firstValue));
            }
            else if (propertyType == typeof(DateTime))
            {
                property.SetValue(queryInstance, DateTime.Parse(firstValue));
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                property.SetValue(queryInstance, DateTimeOffset.Parse(firstValue));
            }
            else if (propertyType == typeof(DateOnly))
            {
                property.SetValue(queryInstance, DateOnly.Parse(firstValue));
            }
            else if (propertyType == typeof(TimeOnly))
            {
                property.SetValue(queryInstance, TimeOnly.Parse(firstValue));
            }
            else if (propertyType.IsEnum)
            {
                property.SetValue(queryInstance, Enum.Parse(propertyType, firstValue));
            }
            else
            {
                return false;
            }
        }

        return  true;
    }
}
