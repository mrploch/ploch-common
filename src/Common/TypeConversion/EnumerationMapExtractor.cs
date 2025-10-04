using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides functionality to extract a map of names to enum values.
/// </summary>
/// <summary>
///     Enumeration fields can be mapped to string names using <see cref="EnumMappingAttribute" /> attribute.
///     Also the <see cref="EnumConversionAttribute" /> allows to specify if mapped string names should be case-sensitive.
/// </summary>
public static class EnumerationMapExtractor
{
    /// <summary>
    ///     Retrieves a mapping of enumeration field names to their corresponding values for the specified enumeration type.
    /// </summary>
    /// <remarks>
    ///     The <see cref="EnumConversionAttribute" /> and <see cref="EnumMappingAttribute" /> allow to control how
    ///     string names are mapped to enum values.
    /// </remarks>
    /// <param name="enumType">
    ///     The enumeration type for which to create the mapping of field names to values. This parameter must be of type <see cref="System.Type" /> and must
    ///     represent
    ///     an enumeration.
    /// </param>
    /// <returns>
    ///     A dictionary where the keys are the string names of the enumeration fields, and the values are the corresponding enumeration values.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <paramref name="enumType" /> is not an enumeration type.
    /// </exception>
    public static IDictionary<EnumName, object> GetEnumFieldValueMap(Type enumType)
    {
        if (!enumType.NotNull(nameof(enumType)).IsEnum)
        {
            throw new InvalidOperationException($"Type {enumType.FullName} is not an enumeration");
        }

        var enumAttribute = enumType.GetCustomAttribute<EnumConversionAttribute>();
        var isCaseSensitive = false;
        if (enumAttribute != null)
        {
            isCaseSensitive = enumAttribute.CaseSensitive;
        }

        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

        var fieldMap = new Dictionary<EnumName, object>(new EnumNameValueComparer());

        foreach (var fieldInfo in fields)
        {
            AddValueMappings(fieldMap, fieldInfo, isCaseSensitive);
        }

        return fieldMap;
    }

    private static void AddValueMappings(Dictionary<EnumName, object> fieldMap, FieldInfo fieldInfo, bool caseSensitiveDefault)
    {
        var enumValue = fieldInfo.GetValue(null) ??
                        throw new InvalidOperationException($"Value for field {fieldInfo.Name} in enum type {fieldInfo.DeclaringType?.Name} is null");

        foreach (var nameToUse in GetFieldValueNames(fieldInfo, caseSensitiveDefault).Select(name => name ?? new EnumName(string.Empty)))
        {
            if (fieldMap.TryGetValue(nameToUse, out var value))
            {
                var fieldMessage = nameToUse.Name.IsNullOrEmpty() ? "default (empty string)" : nameToUse.Name;

                throw new InvalidOperationException($"Enum field {fieldMessage} is already mapped to {value}");
            }

            fieldMap.Add(nameToUse, enumValue);
        }
    }

    private static IEnumerable<EnumName?> GetFieldValueNames(FieldInfo fieldInfo, bool caseSensitiveDefault)
    {
        var enumMappingAttribute = fieldInfo.GetCustomAttribute<EnumMappingAttribute>();
        if (enumMappingAttribute is null)
        {
            return [ new(fieldInfo.Name, caseSensitiveDefault) ];
        }

        return enumMappingAttribute.IncludeActualEnumName ?
            [ new(fieldInfo.Name, enumMappingAttribute.CaseSensitive.IsCaseSensitive(caseSensitiveDefault)),
              ..enumMappingAttribute.Names.Select(n => new EnumName(n, enumMappingAttribute.CaseSensitive.IsCaseSensitive(caseSensitiveDefault))) ] :
            GetFromNames(enumMappingAttribute.Names, enumMappingAttribute.CaseSensitive.IsCaseSensitive(caseSensitiveDefault));
    }

    private static IEnumerable<EnumName> GetFromNames(IEnumerable<string?> names, bool caseSensitiveDefault) =>
        names.Select(n => new EnumName(n, caseSensitiveDefault));
}
