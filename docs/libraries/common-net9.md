# Ploch.Common.Net9

> .NET 9/10 extensions for application-domain-wide type discovery, complementing the assembly-scoped `TypeLoader` in `Ploch.Common`.

## Overview

`Ploch.Common.Net9` targets modern .NET runtimes (net10.0) and provides utilities that depend on APIs unavailable in .NET Standard. The primary addition is `AppDomainTypesLoader`, which scans all assemblies currently loaded across every `AssemblyLoadContext` in the running application domain — including assemblies loaded after construction.

This is useful in plugin-style architectures, test hosts, or any application that loads assemblies dynamically at runtime. The companion `TypeLoader` in `Ploch.Common` operates on explicitly supplied assemblies; `AppDomainTypesLoader` is the automatic, whole-domain variant.

The library depends on `Ploch.Common` and re-exports its namespace alongside the `Ploch.Common.AssemblyLoading` namespace.

## Installation

```bash
dotnet add package Ploch.Common.Net9
```

## Key Types

| Type | Namespace | Description |
|------|-----------|-------------|
| `AppDomainTypesLoader` | `Ploch.Common.AssemblyLoading` | Scans all `AssemblyLoadContext` instances in the current `AppDomain` and filters types by base types, assembly name globs, and type name globs. Also subscribes to `AppDomain.AssemblyLoad` to capture assemblies loaded after construction. |
| `TypeLoadingConfiguration` | `Ploch.Common.AssemblyLoading` | Immutable record that configures `AppDomainTypesLoader`. Accepts optional `Action<Matcher>` delegates for assembly and type name glob patterns, plus an array of base types. |

## Usage Examples

### Discover all concrete implementations of an interface

```csharp
using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.AssemblyLoading;

// Create configuration: only assemblies whose names start with "MyApp."
var configuration = new TypeLoadingConfiguration(
    AssemblyNameGlobConfiguration: matcher => matcher.AddInclude("MyApp.*"),
    BaseTypes: new[] { typeof(IMyPlugin) });

var loader = new AppDomainTypesLoader(configuration);
loader.ProcessAllAssemblies();

// Retrieve all concrete, non-abstract implementations
IEnumerable<Type> pluginTypes = loader.GetTypesImplementing<IMyPlugin>();
foreach (var type in pluginTypes)
{
    var plugin = (IMyPlugin)Activator.CreateInstance(type)!;
    plugin.Initialise();
}
```

### Filter by type name pattern

```csharp
var configuration = new TypeLoadingConfiguration(
    TypeNameGlobConfiguration: matcher => matcher.AddInclude("**.*Handler"),
    BaseTypes: new[] { typeof(IRequestHandler) });

var loader = new AppDomainTypesLoader(configuration);
loader.ProcessAllAssemblies();

// All types whose full names end in "Handler" and implement IRequestHandler
var handlers = loader.GetTypesImplementing<IRequestHandler>(includeAbstract: false);
```

### Include abstract types

```csharp
// includeAbstract: true includes abstract classes and interfaces in results
var allImplementors = loader.GetTypesImplementing(typeof(IBaseService), includeAbstract: true);
```

### Access the complete set of loaded types

```csharp
// LoadedTypes contains every type that passed all filters
foreach (var type in loader.LoadedTypes)
{
    Console.WriteLine(type.FullName);
}
```

## Configuration Reference

`TypeLoadingConfiguration` is a positional record:

```csharp
public record TypeLoadingConfiguration(
    Action<Matcher>? AssemblyNameGlobConfiguration = null,
    Action<Matcher>? TypeNameGlobConfiguration = null,
    params IEnumerable<Type>? BaseTypes);
```

| Parameter | Description |
|-----------|-------------|
| `AssemblyNameGlobConfiguration` | Configures a `Matcher` with include/exclude glob patterns applied to `Assembly.GetName().Name`. When `null`, all assemblies are considered. |
| `TypeNameGlobConfiguration` | Configures a `Matcher` applied to `Type.FullName`. When `null`, all type names pass. |
| `BaseTypes` | Types or interfaces that matched types must be assignable to. When `null` or empty, no base-type filtering is applied. |

Glob patterns follow `Microsoft.Extensions.FileSystemGlobbing` syntax (e.g. `MyApp.*`, `**.*Service`).

## Comparison with TypeLoader

| Capability | `TypeLoader` (Ploch.Common) | `AppDomainTypesLoader` (Ploch.Common.Net9) |
|---|---|---|
| Target framework | netstandard2.0 + net8.0 | net10.0 |
| Assembly source | Explicitly supplied per call | All `AssemblyLoadContext` instances in `AppDomain` |
| Dynamic loading | No | Yes — subscribes to `AppDomain.AssemblyLoad` |
| Fluent API | Yes — method chaining | No — configure via constructor, call `ProcessAllAssemblies()` |
| Thread safety | No | Yes — uses `lock` around type set mutations |

## Related Libraries

- [Ploch.Common](common.md) — Core library containing `TypeLoader`, `TypeExtensions`, and other reflection utilities that `Ploch.Common.Net9` builds on.
