using System.Collections.Concurrent;
using System.Reflection;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public static class EnumFieldValueCache
{
    private static readonly ConcurrentDictionary<Type, IDictionary<string, object>> EnumsFieldValues = new();

    public static object GetFieldValue(Type enumType, string fieldNameOrAlias)
    {
        var fieldMap = GetFieldsMapping(enumType);

        if (fieldMap.TryGetValue(fieldNameOrAlias, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Enum field mapped to {fieldNameOrAlias} was not found in {enumType}");
    }

    public static IDictionary<string, object> GetFieldsMapping(Type enumType) => EnumsFieldValues.GetOrAdd(enumType, GetEnumFieldValueMap);

    private static IDictionary<string, object> GetEnumFieldValueMap(Type enumType)
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

    private static void AddValueMappings(IDictionary<string, object> fieldMap, FieldInfo fieldInfo)
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

        return enumMappingAttribute.Names;
    }
}
