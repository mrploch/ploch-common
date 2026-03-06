using System.Reflection;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides extension methods for working with MemberInfo instances, such as fields and properties.
/// </summary>
public static class MemberInfoExtensions
{
    /// <summary>
    ///     Retrieves the value of the field or property represented by the specified MemberInfo from a given object.
    /// </summary>
    /// <param name="memberInfo">The MemberInfo object representing the field or property.</param>
    /// <param name="obj">The object from which to retrieve the value.</param>
    /// <param name="index">Optional indexe value for indexer property.</param>
    /// <returns>The value of the field or property if it exists, or null if the MemberInfo is not a field or property.</returns>
    public static object? GetValue(this MemberInfo memberInfo, object? obj, params object?[]? index)
    {
        if (memberInfo is FieldInfo fieldInfo)
        {
            if (obj == null && !fieldInfo.IsStatic)
            {
                return null; // Non-static field cannot be accessed on a null object
            }

            return fieldInfo.GetValue(obj);
        }

        if (memberInfo is not PropertyInfo propertyInfo)
        {
            return null;
        }

        if (obj == null && !propertyInfo.IsStatic())
        {
            return null; // Non-static property cannot be accessed on a null object
        }

        return index?.Length == 0 ? propertyInfo.GetValue(obj) : propertyInfo.GetValue(obj, index);
    }

    /// <summary>
    ///     Determines whether the specified MemberInfo represents an indexer property.
    /// </summary>
    /// <param name="memberInfo">The MemberInfo instance to check.</param>
    /// <returns>True if the MemberInfo represents an indexer property; otherwise, false.</returns>
    public static bool IsIndexer(this MemberInfo memberInfo)
    {
        if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }

        return false;
    }

    /// <summary>
    ///     Determines if the specified MemberInfo represents a non-indexer readable property.
    /// </summary>
    /// <param name="memberInfo">The MemberInfo object to inspect.</param>
    /// <returns>True if the MemberInfo is a readable property and not an indexer; otherwise, false.</returns>
    public static bool IsNonIndexerReadProperty(this MemberInfo memberInfo)
    {
        if (memberInfo is PropertyInfo propertyInfo)
        {
            return propertyInfo.CanRead && !propertyInfo.IsIndexer();
        }

        return false;
    }

    /// <summary>
    ///     Determines whether the specified <see cref="MemberInfo" /> represents a static member.
    /// </summary>
    /// <param name="memberInfo">
    ///     The <see cref="MemberInfo" /> instance to evaluate.
    /// </param>
    /// <returns>
    ///     <see langword="true" /> if the member is static; otherwise, <see langword="false" />.
    /// </returns>
    public static bool IsStatic(this MemberInfo memberInfo) => memberInfo is FieldInfo { IsStatic: true } ||
                                                               (memberInfo is PropertyInfo propertyInfo && propertyInfo.IsStatic()) ||
                                                               memberInfo is MethodInfo { IsStatic: true };
}
