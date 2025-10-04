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
    /// <summary>
    ///     Copies all of the properties of the source object to the target object.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void CopyProperties<T>(this T source, T target)
    {
        source.CopyPropertiesIncludeOnly(target, null);
    }

    /// <summary>
    ///     Copies the properties of the source object to the target object, but only the properties specified in the
    ///     includedProperties array.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <param name="includedProperties">The properties to include or null to copy all.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void CopyPropertiesIncludeOnly<T>(this T source, T target, params string[]? includedProperties)
    {
        source.CopyProperties(target, includedProperties, null);
    }

    /// <summary>
    ///     Copies the properties of the source object to the target object, excluding the properties specified in the
    ///     excludedProperties array.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="target">The target object.</param>
    /// <param name="excludedProperties">The properties to exclude or null to not exclude anything.</param>
    /// <typeparam name="T">The object type.</typeparam>
    public static void CopyPropertiesExcluding<T>(this T source, T target, params string[]? excludedProperties)
    {
        source.CopyProperties(target, null, excludedProperties);
    }

    private static void CopyProperties<T>(this T source, T target, IEnumerable<string>? includedProperties, params string[]? excludedProperties)
    {
        var properties = typeof(T).GetProperties();
        var includedPropertiesSet = includedProperties != null ? new HashSet<string>(includedProperties) : null;

        var excludedPropertiesSet = excludedProperties != null ? new(excludedProperties) : new HashSet<string>();

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
