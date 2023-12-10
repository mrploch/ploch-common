using System;
using System.Collections;
using Dawn;
using Ploch.Common.Collections;

namespace Ploch.Common.Reflection;

/// <summary>
///     <see cref="System.Type" /> extension methods.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    ///     Checks if the type provided is implementing the specified interface.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="interfaceType">The type of interface.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="type" /> is implementing <paramref name="interfaceType" />, false
    ///     otherwise.
    /// </returns>
    public static bool IsImplementing(this Type type, Type interfaceType)
    {
        Guard.Argument(interfaceType, nameof(interfaceType)).NotNull();
        Guard.Argument(type, nameof(type)).NotNull();

        return type.GetInterfaces().Exists(i => i == interfaceType || (i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType));
    }

    /// <summary>
    ///     Checks if the type provided is an <see cref="IEnumerable" />.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><c>true</c> if the provided type is <see cref="IEnumerable" />, otherwise false.</returns>
    public static bool IsEnumerable(this Type type)
    {
        Guard.Argument(type, nameof(type)).NotNull();

        return typeof(IEnumerable).IsAssignableFrom(type);
    }
}