using System.Collections.Generic;
using System.Reflection;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides utility methods for working with type information and reflection.
/// </summary>
public static class TypeHelper
{
    /// <summary>
    ///     Retrieves the values of all static fields for the specified type.
    /// </summary>
    /// <typeparam name="TType">The type whose static field values are to be retrieved.</typeparam>
    /// <param name="bindingFlags">
    ///     A combination of <see cref="BindingFlags" /> that specifies how the search for fields is conducted.
    ///     By default, public static fields are retrieved.
    /// </param>
    /// <returns>
    ///     A dictionary where the key represents the name of the static field, and the value represents the value of the field.
    /// </returns>
    public static IDictionary<string, object?> GetStaticFieldValues<TType>(BindingFlags bindingFlags = BindingFlags.Public) =>

// Calling extension method as instance
#pragma warning disable PH2073
        ObjectReflectionExtensions.GetFieldValues<TType>(default, bindingFlags | BindingFlags.Static);
#pragma warning restore PH2073
}
