using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides extension methods for discovering types that implement or inherit from specific base types.
/// </summary>
public static class ImplementationTypes
{
    /// <summary>
    ///     Gets all types from the assembly containing the specified type that implement or inherit from the specified base type.
    /// </summary>
    /// <param name="assemblyType">The type whose assembly will be searched for implementations.</param>
    /// <param name="baseType">The base type or interface to find implementations of.</param>
    /// <param name="includeAbstract">When true, abstract types are included in the results; otherwise, only concrete types are returned. Default is false.</param>
    /// <returns>An enumerable collection of types that implement or inherit from the specified base type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyType" /> or <paramref name="baseType" /> is null.</exception>
    public static IEnumerable<Type> GetTypesImplementing(this Type assemblyType, Type baseType, bool includeAbstract = false)
    {
        assemblyType.NotNull(nameof(assemblyType));
        baseType.NotNull(nameof(baseType));

        return assemblyType.Assembly.GetTypesImplementing(baseType, includeAbstract);
    }

    /// <summary>
    ///     Gets all types from the specified assembly that implement or inherit from the specified base type.
    /// </summary>
    /// <param name="assembly">The assembly to search for implementations.</param>
    /// <param name="baseType">The base type or interface to find implementations of.</param>
    /// <param name="includeAbstract">When true, abstract types are included in the results; otherwise, only concrete types are returned. Default is false.</param>
    /// <returns>An enumerable collection of types that implement or inherit from the specified base type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly" /> is null.</exception>
    public static IEnumerable<Type> GetTypesImplementing(this Assembly assembly, Type baseType, bool includeAbstract = false)
    {
        assembly.NotNull(nameof(assembly));

        return assembly.GetTypes().Where(type => type.IsImplementing(baseType, !includeAbstract));
    }

    /// <summary>
    ///     Gets all types from the specified assembly that implement or inherit from the specified generic base type.
    /// </summary>
    /// <typeparam name="TBaseType">The base type or interface to find implementations of.</typeparam>
    /// <param name="assembly">The assembly to search for implementations.</param>
    /// <param name="includeAbstract">When true, abstract types are included in the results; otherwise, only concrete types are returned. Default is false.</param>
    /// <returns>An enumerable collection of types that implement or inherit from the specified base type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly" /> is null.</exception>
    public static IEnumerable<Type> GetTypesImplementing<TBaseType>(this Assembly assembly, bool includeAbstract = false) where TBaseType : class
    {
        assembly.NotNull(nameof(assembly));

        return assembly.GetTypesImplementing(typeof(TBaseType), includeAbstract);
    }
}
