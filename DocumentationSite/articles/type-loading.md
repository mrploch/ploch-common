# Type Loading and Assembly Scanning

Plug-in hosts, convention-based dependency-injection registration, validator discovery and mapping
profile discovery all need the same thing: *find every type in some set of assemblies that satisfies
some criteria*. `Ploch.Common.Reflection` provides three levels of tool for that job.

```csharp
using Ploch.Common.Reflection;
```

| Level | Type | Use when |
|---|---|---|
| Single assembly | `ImplementationTypes` | You already have the assembly and one base type. |
| Several assemblies | `AssemblyTypes`, `AssemblyListBuilder` | You need to search a set of assemblies, or the whole `AppDomain`. |
| Full control | `TypeLoader` + `ITypeLoaderConfigurator` | Multiple base types, glob filters on assembly or type names, abstract-type control, accumulated results. |

---

## `ImplementationTypes` — one assembly, one base type

```csharp
public static IEnumerable<Type> GetTypesImplementing(this Assembly assembly, Type baseType, bool includeAbstract = false)
public static IEnumerable<Type> GetTypesImplementing<TBaseType>(this Assembly assembly, bool includeAbstract = false) where TBaseType : class
public static IEnumerable<Type> GetTypesImplementing(this Type assemblyType, Type baseType, bool includeAbstract = false)
```

The third overload is the convenient one: it takes a *marker type* and searches the assembly that
declares it, so you never have to name the assembly. Note that this overload takes the base type as a
`Type` argument — the generic form exists only on the `Assembly` overload:

```csharp
var validators = typeof(CreateOrderValidator).GetTypesImplementing(typeof(IValidator));

foreach (var validatorType in validators)
{
    services.AddScoped(typeof(IValidator), validatorType);
}

// Equivalent, using the generic overload on the assembly:
var sameValidators = typeof(CreateOrderValidator).Assembly.GetTypesImplementing<IValidator>();
```

`includeAbstract` defaults to `false`, so abstract bases and interfaces are excluded — which is what
you want when the results are going to be instantiated.

Matching is performed by [`TypeExtensions.IsImplementing`](reflection.md#isimplementing-and-open-generic-types),
so open generic base types work:

```csharp
var handlerTypes = typeof(ApplicationMarker).GetTypesImplementing(typeof(IRequestHandler<,>));
```

---

## `AssemblyTypes` — several assemblies at once

```csharp
public static IEnumerable<Type> GetImplementations(Type baseType, bool concreteOnly, params IEnumerable<Assembly> assemblies)
public static IEnumerable<Type> GetImplementations<TBaseType>(bool concreteOnly = true, params IEnumerable<Assembly> assemblies)
public static IEnumerable<Type> GetImplementations(this IEnumerable<Assembly> assemblies, Type baseType, bool concreteOnly)
public static IEnumerable<Type> GetImplementations<TBaseType>(this IEnumerable<Assembly> assemblies, bool concreteOnly = true)
public static IEnumerable<Type> GetAppDomainImplementations(Type baseType, bool concreteOnly = true)
public static IEnumerable<Type> GetAppDomainImplementations<TBaseType>(bool concreteOnly = true)
```

Note that the polarity of the flag is inverted relative to `ImplementationTypes`: here it is
`concreteOnly`, defaulting to `true`.

```csharp
var assemblies = new[] { typeof(DomainMarker).Assembly, typeof(InfrastructureMarker).Assembly };

foreach (var handlerType in assemblies.GetImplementations<IEventHandler>())
{
    services.AddScoped(typeof(IEventHandler), handlerType);
}
```

`GetAppDomainImplementations` scans everything currently loaded into the process:

```csharp
var migrations = AssemblyTypes.GetAppDomainImplementations<IDataMigration>().ToList();
```

Two caveats apply to the `AppDomain` variants:

- **Only assemblies already loaded are searched.** The CLR loads assemblies lazily, so a plug-in
  assembly that nothing has touched yet will not appear. Load the candidates explicitly first — see
  `AssemblyListBuilder` below — or scan a known assembly set instead.
- **It is broad and therefore slow**, touching every framework and third-party assembly in the
  process. Do it once at start-up, never per request.

`AssemblyTypes` retrieves types **defensively**: it handles `ReflectionTypeLoadException` and returns
the types that did load. Scanning an assembly whose optional dependencies are absent therefore yields
partial results rather than bringing start-up down — a genuine difference from calling
`assembly.GetTypes()` yourself.

---

## `AssemblyListBuilder` — assembling the assembly set

```csharp
public AssemblyListBuilder AddAssembly(Assembly assembly)
public AssemblyListBuilder AddAssemblies(params IEnumerable<Assembly> assemblies)
public AssemblyListBuilder AddFromType<T>()
public AssemblyListBuilder AddFromType(Type type)
public AssemblyListBuilder AddFromTypes(params IEnumerable<Type> types)
public AssemblyListBuilder AddFromObject(object obj)
public AssemblyListBuilder AddFromObjects(params IEnumerable<object> objects)
public IEnumerable<Assembly> Build()
```

A fluent way to gather the assemblies to scan from whatever handle you happen to have — a marker type,
a live object, or the assembly itself — de-duplicating as it goes:

```csharp
var assemblies = new AssemblyListBuilder()
                 .AddFromType<DomainMarker>()
                 .AddFromType<InfrastructureMarker>()
                 .AddFromObject(currentPlugin)
                 .AddAssembly(Assembly.GetExecutingAssembly())
                 .Build();

var handlers = assemblies.GetImplementations<IEventHandler>();
```

`AddFromObject` is the one to use when the handle you have is an instance supplied by a host or a
plug-in loader and its concrete type is not known at compile time.

`AssemblyExtensions.GetAssemblyDirectory(this Assembly)` complements this when assemblies must be
discovered on disk before they can be loaded:

```csharp
var pluginDirectory = Path.Combine(typeof(HostMarker).Assembly.GetAssemblyDirectory()!, "plugins");

var pluginAssemblies = Directory.EnumerateFiles(pluginDirectory, "*.dll")
                                .Select(Assembly.LoadFrom);

var plugins = pluginAssemblies.GetImplementations<IPlugin>();
```

---

## `TypeLoader` — configurable, accumulating discovery

`TypeLoader` is the tool for the cases the simpler helpers cannot express: several base types at once,
glob filters over assembly or type names, and results accumulated across several `LoadTypes` calls.

```csharp
public static TypeLoader Configure(Action<ITypeLoaderConfigurator> configurator)

public TypeLoader LoadTypes<TAssemblyType>()
public TypeLoader LoadTypes(Type assemblyType)
public TypeLoader LoadTypes(params Type[] assemblyTypes)
public TypeLoader LoadTypes(Assembly assembly)

public IEnumerable<Type> LoadedTypes { get; }
```

Configuration happens once, up front; `LoadTypes` is then called as many times as needed and every
matching type is accumulated into a `HashSet<Type>` — so scanning overlapping assemblies produces no
duplicates.

### Configuration options

```csharp
ITypeLoaderConfigurator WithBaseType<TBaseType>();
ITypeLoaderConfigurator WithBaseTypes(params Type[] baseTypes);
ITypeLoaderConfigurator IncludeAbstractTypes(bool include = true);
ITypeLoaderConfigurator WithAssemblyGlob(Action<Matcher> globConfiguration);
ITypeLoaderConfigurator WithTypeNameGlob(Action<Matcher> globConfiguration);
```

| Option | Effect |
|---|---|
| `WithBaseType<T>` / `WithBaseTypes` | A type matches if it implements or inherits **any** of the configured base types. Calls accumulate into a set. With none configured, the base-type filter is skipped entirely. |
| `IncludeAbstractTypes` | When `false` (the default), interfaces and abstract classes are excluded. |
| `WithAssemblyGlob` | Filters *whole assemblies* by their simple name (`assembly.GetName().Name`) before any type is examined. |
| `WithTypeNameGlob` | Filters individual types by `Type.FullName` (falling back to `Type.Name`). |

Both glob options use `Microsoft.Extensions.FileSystemGlobbing.Matcher`, configured with ordinal
(case-sensitive) comparison, so patterns support `AddInclude` and `AddExclude` with `*` wildcards.

### A worked example

Discovering every command handler and event handler declared in the application's own assemblies,
excluding test assemblies:

```csharp
var loader = TypeLoader.Configure(configurator =>
                       {
                           configurator.WithBaseTypes(typeof(ICommandHandler<>), typeof(IEventHandler<>))
                                       .WithAssemblyGlob(glob => glob.AddInclude("MyCompany.*")
                                                                     .AddExclude("*.Tests")
                                                                     .AddExclude("*.TestSupport"))
                                       .IncludeAbstractTypes(false);
                       })
                       .LoadTypes<DomainMarker>()
                       .LoadTypes<ApplicationMarker>()
                       .LoadTypes<InfrastructureMarker>();

foreach (var handlerType in loader.LoadedTypes)
{
    foreach (var handlerInterface in handlerType.GetInterfaces().Where(i => i.IsGenericType))
    {
        services.AddScoped(handlerInterface, handlerType);
    }
}
```

The assembly glob is what makes this safe to point at marker types from several projects: any assembly
whose name does not match `MyCompany.*`, or which matches one of the exclusions, is skipped before its
types are enumerated at all.

Filtering by type name instead is useful for naming-convention-driven discovery:

```csharp
var repositories = TypeLoader.Configure(configurator =>
                             {
                                 configurator.WithBaseType<IRepository>()
                                             .WithTypeNameGlob(glob => glob.AddInclude("*Repository")
                                                                           .AddExclude("*ReadOnly*"));
                             })
                             .LoadTypes<InfrastructureMarker>()
                             .LoadedTypes;
```

Remember the glob is matched against the **full** type name including its namespace, so a pattern such
as `"*Repository"` matches on the suffix while `"Repository*"` almost certainly will not match anything.

### Behaviour to be aware of

- `LoadTypes(Assembly)` calls `assembly.GetTypes()` directly. Unlike `AssemblyTypes`, it does **not**
  soften `ReflectionTypeLoadException`, so an assembly with unresolvable dependencies throws. Filter
  such assemblies out with `WithAssemblyGlob`, or use `AssemblyTypes.GetImplementations` for
  untrusted assembly sets.
- `LoadedTypes` is the live set, not a snapshot — calling `LoadTypes` again adds to it. Materialise
  with `.ToList()` if you need a stable view.
- Configuration cannot be changed after `Configure` returns; build a second `TypeLoader` for a
  different query.

---

## Choosing between them

| Situation | Use |
|---|---|
| One assembly, one base type, results instantiated immediately | `ImplementationTypes.GetTypesImplementing` |
| A known set of assemblies, one base type | `AssemblyTypes.GetImplementations` |
| Everything currently loaded in the process | `AssemblyTypes.GetAppDomainImplementations` |
| Gathering assemblies from marker types and live objects | `AssemblyListBuilder` |
| Several base types, name filters, or accumulated results | `TypeLoader` |
| Assemblies that may fail to load some of their types | `AssemblyTypes` (it handles `ReflectionTypeLoadException`) |

### A note on trimming and AOT

All of these scan types at runtime, which the .NET IL trimmer and Native AOT cannot analyse
statically. In a trimmed or AOT-published application, discovered types can be removed from the
output. Preserve them with `[DynamicallyAccessedMembers]`, a trimmer root descriptor, or move
registration to a source generator.

## See also

- [Reflection utilities](reflection.md)
- [`Ploch.Common.Reflection` API reference](../api/Ploch.Common.Reflection.html)
