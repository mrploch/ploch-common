using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides a caching mechanism for mapped enum field values.
/// </summary>
public static class EnumerationFieldValueCache
{
    private static readonly ConcurrentDictionary<Type, IDictionary<EnumName, object>> EnumsFieldValues = new();

    /// <summary>
    ///     Retrieves the value of an enum field based on its mapped name.
    /// </summary>
    /// <param name="enumType">The Type of the enumeration to search.</param>
    /// <param name="name">The mapped name of the enum field to retrieve.</param>
    /// <returns>The value of the enum field corresponding to the specified name.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no enum field is mapped to the specified name.</exception>
    public static object GetFieldValue(Type enumType, string name)
    {
        var fieldMap = GetFieldsMapping(enumType);

        var x = fieldMap.Keys.FirstOrDefault(n => n == name);
        if (x is not null)
        {
            return fieldMap[x];
        }

        if (fieldMap.TryGetValue(name, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Enum field mapped to {name} was not found in {enumType}");
    }

    /// <summary>
    ///     Gets a dictionary mapping field names to their corresponding values for the specified enum type.
    /// </summary>
    /// <param name="enumType">The Type of the enumeration to map.</param>
    /// <returns>A dictionary containing mappings between field names and their values for the specified enum type.</returns>
    public static IDictionary<EnumName, object> GetFieldsMapping(Type enumType) =>
        EnumsFieldValues.GetOrAdd(enumType, EnumerationMapExtractor.GetEnumFieldValueMap);
}
