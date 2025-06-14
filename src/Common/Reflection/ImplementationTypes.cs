using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Collections;

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

/// <summary>
///     Defines a contract for configuring type-loading operations with various criteria, such as base types,
///     assembly patterns, and type name patterns.
/// </summary>
public interface ITypeLoaderConfigurator
{
    /// <summary>
    ///     Configures the type loader to include types that inherit or implement the specified generic base type.
    /// </summary>
    /// <typeparam name="TBaseType">The generic base type or interface to find implementations of.</typeparam>
    /// <returns>The current type loader configurator instance, enabling method chaining.</returns>
    ITypeLoaderConfigurator WithBaseType<TBaseType>();

    /// <summary>
    ///     Configures the type loader to include the specified base types when loading types.
    /// </summary>
    /// <param name="baseTypes">
    ///     An array of base types or interfaces to be used as filters for type-loading operations.
    ///     At least one base type must be provided.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="ITypeLoaderConfigurator" /> to allow for method chaining of type-loading configurations.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="baseTypes" /> is null or empty.
    /// </exception>
    ITypeLoaderConfigurator WithBaseTypes(params Type[] baseTypes);

    /// <summary>
    ///     Configures the type loader to include or exclude abstract types during type-loading operations.
    /// </summary>
    /// <param name="include">
    ///     A boolean value indicating whether abstract types should be included in the results.
    ///     When set to <c>true</c>, abstract types will be included; otherwise, they will be excluded.
    ///     Default is <c>true</c>.
    /// </param>
    /// <returns>The current <see cref="ITypeLoaderConfigurator" /> instance for method chaining.</returns>
    ITypeLoaderConfigurator IncludeAbstractTypes(bool include = true);

    /// <summary>
    ///     Configures a type loader to include or exclude assemblies based on patterns defined by a globbing matcher.
    /// </summary>
    /// <param name="globConfiguration">
    ///     An action to configure the glob matcher, which specifies include and exclude patterns
    ///     for assemblies in the type-loading operation.
    /// </param>
    /// <returns>An instance of <see cref="ITypeLoaderConfigurator" /> for further configuration of the type loader.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="globConfiguration" /> is null.</exception>
    ITypeLoaderConfigurator WithAssemblyGlob(Action<Matcher> globConfiguration);

    /// <summary>
    ///     Configures the type loader to include types whose names match the specified globbing patterns.
    /// </summary>
    /// <param name="globConfiguration">
    ///     An action that configures a <see cref="Microsoft.Extensions.FileSystemGlobbing.Matcher" /> instance
    ///     with the globbing patterns used to filter type names.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="ITypeLoaderConfigurator" />, allowing method chaining
    ///     for additional type-loading configurations.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="globConfiguration" /> is null.
    /// </exception>
    ITypeLoaderConfigurator WithTypeNameGlob(Action<Matcher> globConfiguration);
}

public class TypeLoaderConfigurator : ITypeLoaderConfigurator
{
    public HashSet<Type> BaseTypes { get; } = [];

    public bool IncludeAbstract { get; private set; }

    public Matcher? AssemblyMatcher { get; private set; }

    public Matcher? TypeNameMatcher { get; private set; }

    public ITypeLoaderConfigurator WithBaseTypes(params Type[] baseTypes)
    {
        baseTypes.NotNullOrEmpty(nameof(baseTypes));

        BaseTypes.AddMany(baseTypes);

        // Implementation for configuring base types
        return this;
    }

    public ITypeLoaderConfigurator IncludeAbstractTypes(bool include = true)
    {
        IncludeAbstract = include;

        return this;
    }

    public ITypeLoaderConfigurator WithAssemblyGlob(Action<Matcher> globConfiguration)
    {
        globConfiguration.NotNull(nameof(globConfiguration));

        AssemblyMatcher = new Matcher(StringComparison.Ordinal);
        globConfiguration(AssemblyMatcher);

        return this;
    }

    public ITypeLoaderConfigurator WithTypeNameGlob(Action<Matcher> globConfiguration)
    {
        globConfiguration.NotNull(nameof(globConfiguration));

        TypeNameMatcher = new Matcher(StringComparison.Ordinal);
        globConfiguration(TypeNameMatcher);

        // Implementation for configuring type name glob
        return this;
    }

    public ITypeLoaderConfigurator WithBaseType<TBaseType>() => WithBaseTypes(typeof(TBaseType));
}
