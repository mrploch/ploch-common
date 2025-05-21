using System.Reflection;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides extension methods for working with object reflection.
/// </summary>
public static class ObjectReflectionExtensions
{
    /// <summary>
    ///     Gets the value of a field by name including non-public, instance and static members.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fieldName">The field name.</param>
    /// <returns>The field value if found or null.</returns>
    public static object? GetFieldValue(this object obj, string fieldName)
    {
        var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        return fieldInfo?.GetValue(obj);
    }

    /// <summary>
    ///     Gets the value of a field by name including non-public, instance and static members.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="fieldName">The field name.</param>
    /// <typeparam name="TValue">The object type.</typeparam>
    /// <returns>The field value if found or default.</returns>
    public static TValue? GetFieldValue<TValue>(this object obj, string fieldName)
    {
        return (TValue?)obj.GetFieldValue(fieldName);
    }
}