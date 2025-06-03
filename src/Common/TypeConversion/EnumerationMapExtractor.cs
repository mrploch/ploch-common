using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <summary>
///     Provides functionality to extract a map of names to enum values.
/// </summary>
/// <summary>
///     Enumeration fields can be mapped to string names using <see cref="WindowsManagementObjectEnumMappingAttribute" /> attribute.
///     Also the <see cref="WindowsManagementEnumAttribute" /> allows to specify if mapped string names should be case-sensitive.
/// </summary>
public static class EnumerationMapExtractor
{
    /// <summary>
    ///     Retrieves a mapping of enumeration field names to their corresponding values for the specified enumeration type.
    /// </summary>
    /// <remarks>
    ///     The <see cref="WindowsManagementEnumAttribute" /> and <see cref="WindowsManagementObjectEnumMappingAttribute" /> allow to control how
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
    public static IDictionary<string, object> GetEnumFieldValueMap(Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new InvalidOperationException($"Type {enumType} is not an enumeration");
        }

        var enumAttribute = enumType.GetCustomAttribute<WindowsManagementEnumAttribute>();
        var isCaseSensitive = false;
        if (enumAttribute != null)
        {
            isCaseSensitive = enumAttribute.CaseSensitive;
        }

        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

        var fieldMap = new Dictionary<string, object>(isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

        foreach (var fieldInfo in fields)
        {
            AddValueMappings(fieldMap, fieldInfo);
        }

        return fieldMap;
    }

    private static void AddValueMappings(Dictionary<string, object> fieldMap, FieldInfo fieldInfo)
    {
        var enumValue = fieldInfo.GetValue(null) ?? throw new InvalidOperationException($"Value for field {fieldInfo.Name} is null");
        foreach (var name in GetFieldValueNames(fieldInfo))
        {
            var nameToUse = name ?? string.Empty;
            if (fieldMap.ContainsKey(nameToUse))
            {
                var fieldMessage = nameToUse.IsNullOrEmpty() ? "default (empty string)" : nameToUse;

                throw new InvalidOperationException($"Enum field {fieldMessage} is already mapped to {fieldMap[nameToUse]}");
            }

            fieldMap.Add(nameToUse, enumValue);
        }
    }

    private static IEnumerable<string?> GetFieldValueNames(FieldInfo fieldInfo)
    {
        var enumMappingAttribute = fieldInfo.GetCustomAttribute<WindowsManagementObjectEnumMappingAttribute>();
        if (enumMappingAttribute is null)
        {
            return [fieldInfo.Name];
        }

        return enumMappingAttribute.IncludeActualEnumName ? [fieldInfo.Name, ..enumMappingAttribute.Names] : enumMappingAttribute.Names;
    }
}
