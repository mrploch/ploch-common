using System.Globalization;
using Microsoft.AspNetCore.Http;
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
        queryInstance = new TQuery();

        foreach (var property in typeof(TQuery).GetProperties())
        {
            var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            // Whether a type is bindable is a property of the type, not of the request, so it is
            // checked before the value is looked up. Deciding this only for properties that happen
            // to appear in the query string would let an unsupported type pass unnoticed whenever
            // the caller omitted it.
            if (!IsSupported(propertyType))
            {
                return false;
            }

            if (!query.TryGetValue(property.Name, out var queryValue) || queryValue.Count == 0)
            {
                continue;
            }

            var firstValue = queryValue[0];

            // An empty value ("?page=") is a supplied-but-blank parameter. It is a legitimate
            // empty string, but every other conversion would throw FormatException on it, so it
            // leaves non-string properties at their default instead.
            if (firstValue is null || (firstValue.Length == 0 && propertyType != typeof(string)))
            {
                continue;
            }

            property.SetValue(queryInstance, Convert(firstValue, propertyType));
        }

        return true;
    }

    private static bool IsSupported(Type propertyType) =>
        propertyType == typeof(string) ||
        propertyType == typeof(int) ||
        propertyType == typeof(bool) ||
        propertyType == typeof(DateTime) ||
        propertyType == typeof(DateTimeOffset) ||
        propertyType == typeof(DateOnly) ||
        propertyType == typeof(TimeOnly) ||
        propertyType.IsEnum;

    // Query strings are a culture-neutral wire format, so values are read with the invariant
    // culture. bool and enum values have no culture-sensitive representation.
    private static object Convert(string value, Type propertyType)
    {
        if (propertyType == typeof(string))
        {
            return value;
        }

        if (propertyType == typeof(int))
        {
            return int.Parse(value, CultureInfo.InvariantCulture);
        }

        if (propertyType == typeof(bool))
        {
            return bool.Parse(value);
        }

        if (propertyType == typeof(DateTime))
        {
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        if (propertyType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(value, CultureInfo.InvariantCulture);
        }

        if (propertyType == typeof(DateOnly))
        {
            return DateOnly.Parse(value, CultureInfo.InvariantCulture);
        }

        if (propertyType == typeof(TimeOnly))
        {
            return TimeOnly.Parse(value, CultureInfo.InvariantCulture);
        }

        return Enum.Parse(propertyType, value);
    }
}
