using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Collections;

namespace Ploch.Common.Reflection;

/// <summary>
///     Provides functionality for loading and filtering types from assemblies based on configurable criteria.
/// </summary>
public class TypeLoader
{
    private readonly TypeLoaderConfigurator _configuration;
    private readonly bool _hasBaseTypes;

    private readonly HashSet<Type> _loadedTypes = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeLoader" /> class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration that defines type loading behavior.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration" /> is null.</exception>
    private TypeLoader(TypeLoaderConfigurator configuration)
    {
        configuration.NotNull(nameof(configuration));

        _configuration = configuration;
        _hasBaseTypes = configuration.BaseTypes.Any();
    }

    /// <summary>
    ///     Gets the collection of types that have been loaded and match the configured criteria.
    /// </summary>
    public IEnumerable<Type> LoadedTypes => _loadedTypes;

    /// <summary>
    ///     Creates and configures a new instance of the <see cref="TypeLoader" /> class.
    /// </summary>
    /// <param name="configurator">The action used to configure the type loader.</param>
    /// <returns>A configured instance of <see cref="TypeLoader" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurator" /> is null.</exception>
    public static TypeLoader Configure(Action<ITypeLoaderConfigurator> configurator)
    {
        configurator.NotNull(nameof(configurator));

        var configuration = new TypeLoaderConfigurator();
        configurator(configuration);

        return new TypeLoader(configuration);
    }

    /// <summary>
    ///     Loads types from the assembly containing the specified generic type parameter.
    /// </summary>
    /// <typeparam name="TAssemblyType">The type whose assembly will be used for loading types.</typeparam>
    /// <returns>The current <see cref="TypeLoader" /> instance to enable method chaining.</returns>
    public TypeLoader LoadTypes<TAssemblyType>() => LoadTypes(typeof(TAssemblyType));

    /// <summary>
    ///     Loads types from the assembly containing the specified type.
    /// </summary>
    /// <param name="assemblyType">The type whose assembly will be used for loading types.</param>
    /// <returns>The current <see cref="TypeLoader" /> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyType" /> is null.</exception>
    public TypeLoader LoadTypes(Type assemblyType)
    {
        assemblyType.NotNull(nameof(assemblyType));

        return LoadTypes(assemblyType.Assembly);
    }

    /// <summary>
    ///     Loads types from the assemblies containing the specified types.
    /// </summary>
    /// <param name="assemblyTypes">An array of types whose assemblies will be used for loading types.</param>
    /// <returns>The current <see cref="TypeLoader" /> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyTypes" /> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="assemblyTypes" /> is empty.</exception>
    public TypeLoader LoadTypes(params Type[] assemblyTypes)
    {
        assemblyTypes.NotNullOrEmpty(nameof(assemblyTypes));

        assemblyTypes.ForEach(type => LoadTypes(type.Assembly));

        return this;
    }

    /// <summary>
    ///     Loads types from the specified assembly that match the configured criteria.
    /// </summary>
    /// <param name="assembly">The assembly from which to load types.</param>
    /// <returns>The current <see cref="TypeLoader" /> instance to enable method chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly" /> is null.</exception>
    public TypeLoader LoadTypes(Assembly assembly)
    {
        assembly.NotNull(nameof(assembly));

        if (_configuration.AssemblyMatcher is not null && !_configuration.AssemblyMatcher.Match(assembly.GetName().Name ?? string.Empty).HasMatches)
        {
            return this;
        }

        assembly.GetTypes().Where(IsMatch).ForEach(type => _loadedTypes.Add(type));

        return this;
    }

    /// <summary>
    ///     Determines whether the specified type matches the configured criteria.
    /// </summary>
    /// <param name="type">The type to check against the configured criteria.</param>
    /// <returns>
    ///     <c>true</c> if the type matches all configured criteria; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type" /> is null.</exception>
    private bool IsMatch(Type type)
    {
        type.NotNull(nameof(type));

        if (_hasBaseTypes && !_configuration.BaseTypes.Any(baseType => type.IsImplementing(baseType, !_configuration.IncludeAbstract)))
        {
            return false;
        }

        return _configuration.TypeNameMatcher == null || _configuration.TypeNameMatcher.Match(type.FullName ?? type.Name).HasMatches;
    }
}
