using System;
using System.Collections.Generic;
using Ploch.Common.Collections;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides helper methods for type conversion operations.
/// </summary>
public static class TypeConverterHelper
{
    /// <summary>
    ///     Determines whether a converter can handle a specific type based on type compatibility.
    /// </summary>
    /// <param name="canHandleDerivedValue">If true, derived types are considered compatible with the base type.</param>
    /// <param name="type">The type that the converter is designed to handle.</param>
    /// <param name="actualType">The actual type to check compatibility with.</param>
    /// <returns>
    ///     True if the actual type is exactly the same as the specified type, or if derived types are allowed
    ///     and the actual type is derived from or implements the specified type; otherwise, false.
    /// </returns>
    public static bool CanHandleType(bool canHandleDerivedValue, Type type, Type actualType) =>
        actualType == type || (canHandleDerivedValue && type.IsAssignableFrom(actualType));

    /// <summary>
    ///     Creates a HashSet containing a specified type and optionally combines it with additional types.
    /// </summary>
    /// <typeparam name="TType">The primary type to include in the resulting HashSet.</typeparam>
    /// <param name="types">Additional collections of types to include in the resulting HashSet.</param>
    /// <returns>A HashSet containing the primary type and all additional types.</returns>
    public static HashSet<Type> CombinedTypes<TType>(params IEnumerable<Type>? types) => new HashSet<Type> { typeof(TType) }.AddMany(types ?? []);
}
