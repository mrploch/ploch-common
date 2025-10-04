using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.Reflection;

namespace Ploch.Common.AssemblyLoading;

/// <summary>
///     Provides functionality for loading and managing types from assemblies within an application domain.
///     This class allows for dynamically identifying and filtering types based on specific criteria,
///     such as base types or naming patterns, and processes all assemblies within the current application domain.
/// </summary>
[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:BracesForMultiLineStatementsMustNotShareLine", Justification = "Reviewed.")]
public class AppDomainTypesLoader
{
    private readonly Matcher? _assemblyMatcher;
    private readonly IEnumerable<Type>? _baseTypes;

    private readonly object _lock = new();
    private readonly Matcher? _typeMatcher;
    private readonly HashSet<Type> _types = new();

    /// <summary>
    ///     A utility class responsible for loading and filtering types from assemblies in the current AppDomain
    ///     based on specific configuration rules.
    /// </summary>
    /// <remarks>
    ///     The <see cref="AppDomainTypesLoader" /> class provides functionality to load types from assemblies based
    ///     on various configurations such as filtering by assembly names, type names, or base type inheritance.
    ///     The configuration is defined through the <see cref="TypeLoadingConfiguration" /> record and is applied
    ///     during the construction of this class.
    /// </remarks>
    public AppDomainTypesLoader(TypeLoadingConfiguration configuration)
    {
        if (configuration.AssemblyNameGlobConfiguration != null)
        {
            _assemblyMatcher = new(StringComparison.Ordinal);
            configuration.AssemblyNameGlobConfiguration(_assemblyMatcher);
        }

        if (configuration.TypeNameGlobConfiguration != null)
        {
            _typeMatcher = new(StringComparison.Ordinal);
            configuration.TypeNameGlobConfiguration(_typeMatcher);
        }

        var any = configuration.BaseTypes?.Any();
        _baseTypes = any == true ? configuration.BaseTypes!.ToArray() : null;
    }

    /// <summary>
    ///     Gets the collection of loaded <see cref="System.Type" /> objects identified
    ///     and processed by the <see cref="AppDomainTypesLoader" /> instance.
    /// </summary>
    /// <remarks>
    ///     The <see cref="LoadedTypes" /> property contains the set of types loaded from the assemblies
    ///     available in the current application domain and filtered based on the criteria specified
    ///     in the <see cref="TypeLoadingConfiguration" /> during the initialization of the loader.
    ///     These types are determined after invoking the <see cref="ProcessAllAssemblies" /> method.
    ///     If no assemblies or types meet the criteria specified in the configuration,
    ///     the collection will remain empty.
    /// </remarks>
    /// <value>
    ///     An <see cref="IEnumerable{T}" /> of <see cref="System.Type" /> objects representing the
    ///     types loaded from assemblies.
    /// </value>
    /// <example>
    ///     To use the <see cref="LoadedTypes" /> property, you should first create an instance
    ///     of <see cref="AppDomainTypesLoader" /> with a configuration indicating your filtering
    ///     preferences. After that, invoke <see cref="ProcessAllAssemblies" /> to populate the
    ///     <see cref="LoadedTypes" /> property:
    ///     <code>
    /// var config = new TypeLoadingConfiguration(baseTypes: new[] { typeof(MyBaseClass) });
    /// var loader = new AppDomainTypesLoader(config);
    /// loader.ProcessAllAssemblies();
    /// var loadedTypes = loader.LoadedTypes; // Enumerates all types derived from MyBaseClass.
    /// </code>
    /// </example>
    public IEnumerable<Type> LoadedTypes => _types;

    /// <summary>
    ///     Scans all assemblies available in the current application domain,
    ///     processes them based on the specified configuration, and loads qualified types.
    /// </summary>
    /// <remarks>
    ///     The <see cref="ProcessAllAssemblies" /> method triggers the loading process by scanning all assemblies
    ///     that are currently loaded into the AppDomain's <see cref="AssemblyLoadContext" /> instances. This includes
    ///     any assemblies already loaded and any subsequently loaded assemblies via subscription to the
    ///     <see cref="AppDomain.AssemblyLoad" /> event.
    ///     This method determines which types to load based on the filtering rules defined in the
    ///     <see cref="TypeLoadingConfiguration" /> provided during the initialization of the <see cref="AppDomainTypesLoader" /> instance.
    ///     This method populates the <see cref="LoadedTypes" /> collection with the identified and loaded types.
    /// </remarks>
    /// <example>
    ///     Here's how you can invoke <see cref="ProcessAllAssemblies" /> to load types based on specific criteria:
    ///     <code>
    /// var configuration = new TypeLoadingConfiguration(
    /// baseTypes: new[] { typeof(MyBaseClass) },
    /// includeAssemblies: new[] { "MyAssemblyName" });
    /// var typeLoader = new AppDomainTypesLoader(configuration);
    /// // Process and load matching types from all assemblies in the current AppDomain.
    /// typeLoader.ProcessAllAssemblies();
    /// // Access the results: types loaded and filtered by the configuration.
    /// foreach (var type in typeLoader.LoadedTypes)
    /// {
    /// Console.WriteLine(type.FullName);
    /// }
    /// </code>
    /// </example>
    public void ProcessAllAssemblies()
    {
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;

        foreach (var assemblyLoadContext in AssemblyLoadContext.All)
        {
            foreach (var assembly in assemblyLoadContext.Assemblies)
            {
                LoadAssembly(assembly);
            }
        }
    }

    /// <summary>
    ///     Retrieves a collection of types from the currently loaded assemblies that implement or inherit from the specified base type.
    /// </summary>
    /// <param name="baseType">The base type or interface for which to find implementing or inheriting types.</param>
    /// <param name="includeAbstract">
    ///     A boolean value indicating whether abstract types should be included in the result.
    ///     If set to <c>false</c>, only concrete implementations will be returned.
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{Type}" /> containing all types that implement or inherit from the specified <paramref name="baseType" />.
    ///     The result respects the value of <paramref name="includeAbstract" /> to determine whether abstract types are included.
    /// </returns>
    public IEnumerable<Type> GetTypesImplementing(Type baseType, bool includeAbstract = false)
    {
        lock (_lock)
        {
            return _types.Where(t => t.IsImplementing(baseType, !includeAbstract));
        }
    }

    /// <summary>
    ///     Retrieves types from the loaded assemblies that implement or derive from the specified base type.
    /// </summary>
    /// <typeparam name="TBaseType">The base type or interface that the returned types should implement or inherit from.</typeparam>
    /// <param name="includeAbstract">
    ///     A boolean value indicating whether abstract types should be included in the result.
    ///     If <c>true</c>, abstract types are included; otherwise, they are excluded.
    /// </param>
    /// <returns>
    ///     An enumerable of <see cref="Type" /> objects representing types that are assignable to <typeparamref name="TBaseType" />
    ///     and meet the specified filtering criteria.
    /// </returns>
    /// <remarks>
    ///     This method examines all types loaded into the current application domain by the <see cref="AppDomainTypesLoader" />.
    ///     It filters the types based on their assignability to the provided base type and optionally their abstract status.
    /// </remarks>
    public IEnumerable<Type> GetTypesImplementing<TBaseType>(bool includeAbstract = false) => GetTypesImplementing(typeof(TBaseType), includeAbstract);

    private void LoadAssembly(Assembly? assembly)
    {
        if (assembly == null)
        {
            return;
        }

        if (_assemblyMatcher != null)
        {
            var assemblyName = assembly.GetName().Name;
            if (assemblyName == null || !_assemblyMatcher.Match(assemblyName).HasMatches)
            {
                return; // Skip assemblies that do not match the glob pattern
            }
        }

        // This method can be used to load types from a specific assembly.
        // You can use reflection to get types, methods, etc. from the assembly.
        foreach (var type in assembly.GetTypes())
        {
            lock (_lock)
            {
                if (_types.Contains(type))
                {
                    continue; // Skip types that are already loaded
                }
            }

            if (_typeMatcher != null)
            {
                var typeName = type.FullName;
                if (typeName == null || !_typeMatcher.Match(typeName).HasMatches)
                {
                    continue; // Skip types that do not match the glob pattern
                }
            }

            if (_baseTypes?.Any(baseType => type.IsSubclassOf(baseType)) == false)
            {
                var sc = true; // Skip types that do not match the base types
            }

            if (_baseTypes?.Any(baseType => baseType.IsAssignableFrom(type)) == false)
            {
                continue; // Skip types that do not match the base types
            }

            lock (_lock)
            {
                var add = _types.Add(type);
                var b = _types.Add(type);
            }
        }
    }

    private void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs args)
    {
        try
        {
            LoadAssembly(args.LoadedAssembly);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
