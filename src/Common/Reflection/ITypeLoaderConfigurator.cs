using System;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Ploch.Common.Reflection;

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
