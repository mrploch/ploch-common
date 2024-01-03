using System.Collections.Generic;

namespace Ploch.Common;

/// <summary>
///     Class ObjectCloningHelpers.
/// </summary>
/// <remarks>
///     Contains various static methods useful when copying properties / cloning objects.
/// </remarks>
public static class ObjectCloningHelpers
{
    public static void CopyProperties<T>(this T source, T target)
    {
        CopyPropertiesIncludeOnly(source, target, null);
    }

    public static void CopyPropertiesIncludeOnly<T>(this T source, T target, params string[]? includedProperties)
    {
        CopyProperties(source, target, includedProperties, null);
    }

    public static void CopyPropertiesExcluding<T>(this T source, T target, params string[]? excludedProperties)
    {
        CopyProperties(source, target, null, excludedProperties);
    }

    private static void CopyProperties<T>(this T source, T target, IEnumerable<string>? includedProperties, params string[]? excludedProperties)
    {
        var properties = typeof(T).GetProperties();
        var includedPropertiesSet = includedProperties != null ? new HashSet<string>(includedProperties) : null;

        var excludedPropertiesSet = excludedProperties != null ? new HashSet<string>(excludedProperties) : new HashSet<string>();

        foreach (var property in properties)
        {
            if (includedPropertiesSet != null && !includedPropertiesSet.Contains(property.Name))
            {
                continue;
            }

            if (excludedPropertiesSet.Contains(property.Name))
            {
                continue;
            }

            if (!property.CanRead)
            {
                continue;
            }

            if (!property.CanWrite)
            {
                continue;
            }

            var value = property.GetValue(source);
            property.SetValue(target, value);
        }
    }
}