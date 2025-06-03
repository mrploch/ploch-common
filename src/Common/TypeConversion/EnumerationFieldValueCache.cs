using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <summary>
///     Provides a caching mechanism for mapped enum field values.
/// </summary>
public static class EnumerationFieldValueCache
{
    private static readonly ConcurrentDictionary<Type, IDictionary<string, object>> EnumsFieldValues = new();

    public static object GetFieldValue(Type enumType, string name)
    {
        var fieldMap = GetFieldsMapping(enumType);

        if (fieldMap.TryGetValue(name, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Enum field mapped to {name} was not found in {enumType}");
    }

    public static IDictionary<string, object> GetFieldsMapping(Type enumType) =>
        EnumsFieldValues.GetOrAdd(enumType, EnumerationMapExtractor.GetEnumFieldValueMap);
}
