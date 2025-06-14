using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides utility methods for retrieving types that implement specific base types or interfaces
///     from assemblies within a given scope, including the AppDomain or custom assembly lists.
/// </summary>
public static class AssemblyTypes
{
    /// <summary>
    ///     Retrieves all types that implement or inherit from the specified base type within the provided assemblies.
    /// </summary>
    /// <param name="baseType">The base type to search for implementations of.</param>
    /// <param name="concreteOnly">Return only concrete implementations</param>
    /// <param name="assemblies">The assemblies to search within.</param>
    /// <returns>A collection of types that implement or inherit from the specified base type.</returns>
    public static IEnumerable<Type> GetImplementations(Type baseType, bool concreteOnly, params IEnumerable<Assembly> assemblies)
    {
        var result = new List<Type>();
        foreach (var implementations in assemblies.Select(assembly => assembly.GetTypes().Where(t => t.IsImplementing(baseType, concreteOnly))))
        {
            result.AddRange(implementations);
        }

        return result;
    }

    /// <summary>
    ///     Retrieves all types that implement or inherit from the specified generic base type within the provided assemblies.
    /// </summary>
    /// <typeparam name="TBaseType">The generic base type to search for implementations of.</typeparam>
    /// <param name="concreteOnly">Return only concrete types.</param>
    /// <param name="assemblies">The assemblies to search within.</param>
    /// <returns>A collection of types that implement or inherit from the specified generic base type.</returns>
    public static IEnumerable<Type> GetImplementations<TBaseType>(bool concreteOnly = true, params IEnumerable<Assembly> assemblies) =>
        GetImplementations(typeof(TBaseType), concreteOnly, assemblies);

    /// <summary>
    ///     Retrieves all types that implement or inherit from the specified base type within the provided assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to search within.</param>
    /// <param name="baseType">The base type to search for implementations of.</param>
    /// <param name="concreteOnly">Return only concrete implementations.</param>
    /// <returns>A collection of types that implement or inherit from the specified base type.</returns>
    public static IEnumerable<Type> GetImplementations(this IEnumerable<Assembly> assemblies, Type baseType, bool concreteOnly) =>
        GetImplementations(baseType, concreteOnly, assemblies.ToArray());

    /// <summary>
    ///     Retrieves all types that implement or inherit from the specified base type within the provided assemblies.
    /// </summary>
    /// <param name="baseType">The base type to search for implementations of.</param>
    /// <param name="concreteOnly">Return only concrete implementations.</param>
    /// <param name="assemblies">The assemblies to search within.</param>
    /// <returns>A collection of types that implement or inherit from the specified base type.</returns>
    public static IEnumerable<Type> GetImplementations<TBaseType>(this IEnumerable<Assembly> assemblies, bool concreteOnly = true) =>
        GetImplementations<TBaseType>(concreteOnly, assemblies.ToArray());

    /// <summary>
    ///     Retrieves all types that implement or inherit from the specified base type within all assemblies in the current
    ///     <see cref="AppDomain" />.
    /// </summary>
    /// <param name="baseType">The base type to search for implementations of.</param>
    /// <param name="concreteOnly">Return only concrete types.</param>
    /// <returns>A collection of types that implement or inherit from the specified base type.</returns>
    public static IEnumerable<Type> GetAppDomainImplementations(Type baseType, bool concreteOnly = true) =>
        GetImplementations(baseType, concreteOnly, AppDomain.CurrentDomain.GetAssemblies());

    /// <summary>
    ///     Retrieves all types that implement or inherit from the specified generic base type within all assemblies in the
    ///     current <see cref="AppDomain" />.
    /// </summary>
    /// <typeparam name="TBaseType">The generic base type to search for implementations of.</typeparam>
    /// <param name="concreteOnly">Return only concrete types.</param>
    /// <returns>A collection of types that implement or inherit from the specified generic base type.</returns>
    public static IEnumerable<Type> GetAppDomainImplementations<TBaseType>(bool concreteOnly = true) =>
        GetImplementations<TBaseType>(concreteOnly, AppDomain.CurrentDomain.GetAssemblies());
}
