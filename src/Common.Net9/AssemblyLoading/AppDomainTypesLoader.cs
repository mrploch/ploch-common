using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.Reflection;

namespace Ploch.Common.AssemblyLoading;

public record TypeLoadingConfiguration(Action<Matcher>? AssemblyNameGlobConfiguration = null,
                                       Action<Matcher>? TypeNameGlobConfiguration = null,
                                       params IEnumerable<Type>? BaseTypes);

[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1500:BracesForMultiLineStatementsMustNotShareLine", Justification = "Reviewed.")]
public class AppDomainTypesLoader
{
    private readonly Matcher? _assemblyMatcher;
    private readonly IEnumerable<Type>? _baseTypes;

    private readonly object _lock = new();
    private readonly Matcher? _typeMatcher;
    private readonly HashSet<Type> _types = new();

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

    public IEnumerable<Type> LoadedTypes => _types;

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

    public IEnumerable<Type> GetTypesImplementing(Type baseType, bool includeAbstract = false)
    {
        lock (_lock)
        {
            return _types.Where(t => t.IsImplementing(baseType, !includeAbstract));
        }
    }

    public void Test()
    { }

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
