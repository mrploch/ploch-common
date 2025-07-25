using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Collections;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides a concrete implementation of the <see cref="ITypeLoaderConfigurator" /> interface,
///     enabling the configuration of type-loading operations based on various criteria such as base types,
///     inclusion of abstract types, assembly patterns, and type name patterns.
/// </summary>
public class TypeLoaderConfigurator : ITypeLoaderConfigurator
{
    /// <summary>
    ///     Gets the set of base types against which types are matched during type loading.
    ///     Used to filter or constrain the types to those that derive from or implement
    ///     the specified base types.
    /// </summary>
    /// <remarks>
    ///     This property is a collection containing types that are used as base type constraints
    ///     when loading or matching types. Types added to this collection will limit the resulting
    ///     types to those that are assignable to the base types listed.
    /// </remarks>
    public HashSet<Type> BaseTypes { get; } = [];

    /// <summary>
    ///     Indicates whether abstract types should be included during type loading or matching operations.
    /// </summary>
    /// <remarks>
    ///     When set to <c>true</c>, abstract types will be considered in operations that involve determining
    ///     or filtering types. When set to <c>false</c>, only non-abstract (concrete) types will be included.
    ///     This property is typically used in conjunction with type configuration or selection mechanisms.
    /// </remarks>
    public bool IncludeAbstract { get; private set; }

    /// <summary>
    ///     Gets the <see cref="Matcher" /> instance used for configuring and matching assembly names
    ///     based on specified glob patterns.
    /// </summary>
    /// <remarks>
    ///     This property is typically utilized to filter assemblies during type loading operations,
    ///     ensuring only assemblies matching the specified criteria are considered.
    ///     The glob configuration is applied using the <see cref="TypeLoaderConfigurator.WithAssemblyGlob" /> method.
    /// </remarks>
    public Matcher? AssemblyMatcher { get; private set; }

    /// <summary>
    ///     Represents an internal matcher used to configure pattern-based filtering for type names
    ///     in assembly scanning or other reflection-based operations.
    /// </summary>
    /// <remarks>
    ///     This property allows the specification of a globbing pattern to match type names, enabling
    ///     fine-grained control over which types are included in operations that rely on pattern matching.
    ///     It utilizes the <see cref="Microsoft.Extensions.FileSystemGlobbing.Matcher" /> for defining
    ///     and applying globbing patterns.
    /// </remarks>
    /// <seealso cref="Microsoft.Extensions.FileSystemGlobbing.Matcher" />
    public Matcher? TypeNameMatcher { get; private set; }

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
    public ITypeLoaderConfigurator WithBaseTypes(params Type[] baseTypes)
    {
        baseTypes.NotNullOrEmpty(nameof(baseTypes));

        BaseTypes.AddMany(baseTypes);

        // Implementation for configuring base types
        return this;
    }

    /// <summary>
    ///     Configures the type loader to include or exclude abstract types during type-loading operations.
    /// </summary>
    /// <param name="include">
    ///     A boolean value indicating whether abstract types should be included in the results.
    ///     When set to true, abstract types will be included; otherwise, they will be excluded.
    ///     Default is true.
    /// </param>
    /// <returns>
    ///     The current instance of <see cref="ITypeLoaderConfigurator" /> to enable method chaining of type-loading configurations.
    /// </returns>
    public ITypeLoaderConfigurator IncludeAbstractTypes(bool include = true)
    {
        IncludeAbstract = include;

        return this;
    }

    /// <summary>
    ///     Configures the type loader to use a custom assembly matching pattern based on the specified glob configuration.
    /// </summary>
    /// <param name="globConfiguration">
    ///     An action that configures a <see cref="Microsoft.Extensions.FileSystemGlobbing.Matcher" /> instance to define
    ///     the glob patterns for selecting assemblies to include during type-loading operations.
    /// </param>
    /// <returns>
    ///     The current <see cref="ITypeLoaderConfigurator" /> instance to allow for method chaining of type-loading configurations.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="globConfiguration" /> is null.
    /// </exception>
    public ITypeLoaderConfigurator WithAssemblyGlob(Action<Matcher> globConfiguration)
    {
        globConfiguration.NotNull(nameof(globConfiguration));

        AssemblyMatcher = new Matcher(StringComparison.Ordinal);
        globConfiguration(AssemblyMatcher);

        return this;
    }

    /// <summary>
    ///     Configures the type loader to use the specified type name glob pattern for filtering types during type-loading operations.
    /// </summary>
    /// <param name="globConfiguration">
    ///     An action to configure a <see cref="Microsoft.Extensions.FileSystemGlobbing.Matcher" /> instance with the desired glob patterns for type names.
    ///     The provided <see cref="Matcher" /> will be used to match type names against the configured glob pattern.
    /// </param>
    /// <returns>The current <see cref="ITypeLoaderConfigurator" /> instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="globConfiguration" /> is null.</exception>
    public ITypeLoaderConfigurator WithTypeNameGlob(Action<Matcher> globConfiguration)
    {
        globConfiguration.NotNull(nameof(globConfiguration));

        TypeNameMatcher = new Matcher(StringComparison.Ordinal);
        globConfiguration(TypeNameMatcher);

        // Implementation for configuring type name glob
        return this;
    }

    /// <summary>
    ///     Configures the type loader to include the specified base type when loading types.
    /// </summary>
    /// <typeparam name="TBaseType">
    ///     The base type or interface to be used as a filter for type-loading operations.
    /// </typeparam>
    /// <returns>
    ///     The current instance of <see cref="ITypeLoaderConfigurator" /> to allow for method chaining of type-loading configurations.
    /// </returns>
    public ITypeLoaderConfigurator WithBaseType<TBaseType>() => WithBaseTypes(typeof(TBaseType));
}
