## `Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection`: dropped the FluentAssertions dependencies

**Removed `FluentAssertions` and `FluentAssertions.Json` from the shipped package's dependency graph.**
The project declared both as `PackageReference`s, but its only source file
(`NewtonsoftJsonSerializerRegistration.cs`) never used either — they were leftovers, most
likely copied from a test project. Because the project is packable, every consumer of
`Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection` transitively
pulled two test-assertion libraries into their own dependency graph.

The generated nuspec's `<dependencies>` group changes by exactly those two entries and
nothing else:

```diff
       <group targetFramework=".NETStandard2.0">
         <dependency id="Ploch.Common.Serialization.NewtonsoftJson" version="…" exclude="Build,Analyzers" />
-        <dependency id="FluentAssertions" version="8.10.0" exclude="Build,Analyzers" />
-        <dependency id="FluentAssertions.Json" version="8.0.0" exclude="Build,Analyzers" />
         <dependency id="Microsoft.Extensions.DependencyInjection.Abstractions" version="10.0.5" exclude="Build,Analyzers" />
       </group>
```

`Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection.Tests` did use
FluentAssertions, but relied on inheriting it transitively from the project under test.
Both packages are now declared directly on the test project, where they belong.

### Compatibility

Consumers that (accidentally) relied on FluentAssertions arriving transitively through
this package must now reference it themselves. That is the intended correction — a
production package should not put a test-assertion library on its consumers' compile
path. The public API of the package is unchanged.

Refs: #273
