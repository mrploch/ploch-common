# Architecture — ploch-common

| Field | Value |
|---|---|
| Last verified | 2026-10-10, against commit `1f1764b166dabfad4a01b166b94a1115b88e7c6d` |
| Verified by | Reading every project file and the non-trivial sources of each family; re-checking 20+ claims directly (project and solution membership, target frameworks, dependency direction, handler semantics, attribute definitions, test counts) |
| Entry point | [`CLAUDE.md`](../../CLAUDE.md) |

## 1. Summary

A family of small, independently packaged .NET libraries: extension methods and guard clauses (`Ploch.Common`),
serialization and dependency-injection abstractions with concrete implementations, a priority-based action-handler
framework, MVC view helpers, a MAUI helper library, and a
test-support family (xUnit v3, FluentAssertions, AutoFixture/Moq helpers). The core has no project references and
nothing in it points back to another family. Most families build on the core, but not all: `Common.Serialization`,
`Common.AppServices`, `Common.DataAnnotations` and `Common.WebUI`, among others, have no project reference at all. Do not add a core dependency to a package that does not have one.

The repository has **no database, no application host and no UI of its own**. Nothing here is an executable product:
`OutputType=Exe` appears only on test projects, on the three xUnit v3 support libraries (xUnit v3 requires it) and on
`TestingSupport.MockConsoleApp`, a stub program that tests launch.

The solution split matters more than the folder layout: **only `Ploch.Common.slnx` is built, tested and packed by CI**.
14 projects, the MAUI family among them, are in no solution at all (section 2). The CRUD web-endpoint libraries and
`Common.Ardalis.Result` were moved to [ploch-endpoints](https://github.com/mrploch/ploch-endpoints) by #353.

## 2. Component map

Counts: 33 `.csproj` under `src/`, 20 under `tests/`, 53 in total. `Ploch.Common.slnx` lists 39 of them (22 `src`, 17
`tests`). "Published" means the package id exists on nuget.org (21 ids, all at 4.0.47, checked 2026-10-10).

Target framework key: **net10** is `$(TargetFrameworkVersion)`, set once in `Directory.Build.props:42`. A hard-coded
value is written out.

```text
Ploch.Common (netstandard2.0;net8.0)          <- no project references
  ^   ^   ^   ^
  |   |   |   +-- Common.Net9 (net10), Common.ObjectBuilder, Common.Extensions.Configuration, Common.NSwag,
  |   |   |       Common.Maui (+Fonts), Common.Apps/Actions.Model
  |   |   +------ Common.DependencyInjection <- Serialization.*.ExtensionsDependencyInjection
  |   +---------- Common.Serialization.SystemTextJson (uses ArgumentChecking)
  +-------------- TestingSupport.* (all use ArgumentChecking)
Common.Serialization (no references) <- SystemTextJson, NewtonsoftJson <- *.ExtensionsDependencyInjection
```

| Family | Projects (under `src/`) | Target | In `Ploch.Common.slnx` | Published |
|---|---|---|---|---|
| Core | `Common` | `netstandard2.0;net8.0` | yes | yes |
| Core add-ons | `Common.Net9` | net10 | yes | yes (id still `Ploch.Common.Net9`; see section 7) |
| | `Common.DataAnnotations` | `netstandard2.0;net6.0` | yes | yes |
| | `Common.DawnGuard` (deprecated) | `netstandard2.0` | yes | yes (marked deprecated) |
| | `Common.ObjectBuilder` | net10 | **no** | no |
| | `Common.Extensions.Configuration` | `net8.0` | **no** | no |
| | `Common.Dependencies` (empty meta-package) | `net10.0` | **no** | no |
| Serialization | `Common.Serialization`, `.SystemTextJson`, `.NewtonsoftJson` and the two `*.ExtensionsDependencyInjection` | `netstandard2.0` | yes | yes (5 ids) |
| Dependency injection | `Common.DependencyInjection` | `netstandard2.0` | yes | yes |
| | `Common.DependencyInjection.Hosting` | `net8.0` | **no** | no |
| Application services | `Common.AppServices`, `Common.AppServices.Web` | `net8.0` | yes | yes |
| Apps | `Common.Apps/Shared`, `Common.Apps/Apps.Actions/Actions.Model` | net10 | yes | yes |
| Web | `Common.WebUI` | net10 | yes | yes |
| | `Common.NSwag` | net10 | **no** (does not compile) | no |
| | `Common.Web` | — | **no `.csproj`**: one orphan source file | no |
| MAUI | `Common.Maui/Common.Maui`, `.Fonts` | net10 | **no** (no solution) | no |
| Test support | `TestingSupport`, `TestingSupport.FluentAssertions` | `netstandard2.0` | yes | yes |
| | `TestingSupport.XUnit3` | `net10;net8.0` | yes | yes |
| | `TestingSupport.XUnit3.AutoMoq`, `.XUnit3.Dependencies` | `net8.0` | yes | yes |
| | `TestingSupport.MockConsoleApp` | `net10.0;net8.0` | yes | yes |
| | `TestingSupport.FluentAssertions.IOAbstractions` | `netstandard2.0` | **no** (does not compile) | no |
| | `TestingSupport.Dependencies.MetaPackages` | — | **no `.csproj`**: an orphan `.nuspec` | no |

Test projects that live under `src/` (against the layout rule): `Common.WebUI.Tests`, `Common.Maui/Common.Maui.Tests`
and its two fixture assemblies.

Solutions at the repository root (there is no `.sln`):

| Solution | Projects | Role |
|---|---|---|
| `Ploch.Common.slnx` | 39 | The CI and release solution. `build-dotnet.yml` and `release.yml` restore, build and test only this file |
| `Ploch.Common.LocalDev.slnx` | 39 | Mirror of the main solution for local work; one path does not exist |

Projects in **no** solution: `Common.Dependencies`, `Common.DependencyInjection.Hosting`,
`Common.Extensions.Configuration`, `Common.Maui`, `Common.Maui.Fonts`, `Common.Maui.Tests` (+ its two fixture
assemblies), `Common.NSwag`, `Common.ObjectBuilder` (+ its tests), `TestingSupport.FluentAssertions.IOAbstractions`
(+ its tests), `tests/Common.Apps/Apps.Shared.Tests`.

## 3. Key types

### `Ploch.Common` (`src/Common`, about 100 files)

| Type or area | File | Role |
|---|---|---|
| `Guard`, `PathGuard` (partial; `ArgumentChecking`) | `ArgumentChecking/Guard.cs`, `GuardNet7.cs`, `PathGuard.cs`, `PathGuardNet7.cs` | Fluent guard clauses that return the validated value. `NotX` throws the `ArgumentException` family; `RequiredX` throws `InvalidOperationException`. `AssignableTo`/`AssignableToOrNull` are the newest additions |
| `EnumerableExtensions` and `CollectionExtensions` | `Collections/` | `ForEach`, `None`, `Join`, `JoinWithFinalSeparator`, `If`, `Shuffle`, `TakeRandom`, `IsNullOrEmpty`, `Second`, `ExceptItems`, `AddMany`, `AddIfNotNull` |
| `StringExtensions`, `StringParsingExtensions`, `IsInExtensions` | repository root of `src/Common` | String null/empty tests, conversions, `ContainsAny`, `In`/`NotIn` |
| `TypeExtensions`, `PropertyHelpers`, `ObjectReflectionExtensions` | `Reflection/` | `IsImplementing`, `IsConcreteImplementation`, `GetReadableTypeName`, `IsSimpleType`, property get/set helpers |
| `TypeLoader` (+ `TypeLoaderConfigurator`) | `Reflection/TypeLoader.cs` | Assembly scan with fluent configuration and glob matching on assembly and type names |
| `EnumerationConverter`, `ITypeConverter` family | `EnumerationConverter.cs`, `TypeConversion/` | Enum parsing and a pluggable conversion contract |
| `PathUtils`, `CommandLineParser` | `IO/` | Path helpers and command-line splitting |
| `Randomizer` family, `Hashing`, `ProcessExtensions`, `ExpressionExtensions`, matchers | `Randomizers/`, `Cryptography/`, `Diagnostics/`, `Linq/`, `Matchers/` | Small utilities |
| `JetbrainsAnnotations.cs` | root | 106 `internal` annotation types, a vendored copy; guards use it, the NuGet package is not referenced by the library |

### Other families

| Type | File | Role |
|---|---|---|
| `AppDomainTypesLoader`, `TypeLoadingConfiguration` | `src/Common.Net9/` | Whole-`AppDomain` type discovery; namespace `Ploch.Common.AssemblyLoading` (the project and package are still named `Net9`) |
| `ISerializer`, `ISerializer<TSettings>`, `IAsyncSerializer`, `IAsyncSerializer<TSettings>` | `src/Common.Serialization/` | Contracts; `Serializer<TSettings,TDataJsonObject>` and `AsyncSerializer<TSettings,TDataJsonObject>` are the base classes |
| `SystemTextJsonSerializer`, `NewtonsoftJsonObjectSerializer` | `src/Common.Serialization.*Json/` | Implementations. Newtonsoft is synchronous only |
| `ServicesBundle`, `ConfigurableServicesBundle`, `DelegatingServicesBundle`, `ServicesBundleRegistration` | `src/Common.DependencyInjection/` | Modular service registration (section 4, flow 2) |
| `IScopedService<T>` / `ScopedService<T>` | `src/Common.DependencyInjection/IScopedService.cs` | Wrapper that owns a scope and one resolved service. Not registered by any extension method |
| `ActionHandlerManager<,,>`, `IActionHandler<,,>`, `ActionHandlerResult`, `ActionExecutionId<T>` | `src/Common.Apps/Apps.Actions/Actions.Model/` | Priority-ordered fallback chain (flow 3). Namespace is `Ploch.Common.Apps.Model`, not the project name |
| `AppPage`, `SelectListHelper` | `src/Common.WebUI/` | MVC view helpers |
| `BaseViewModel`, `BaseContentPage`, `AppConfigurator`, font-glyph classes | `src/Common.Maui/` | MAUI helpers; plain SDK with the XAML package, no MAUI workload needed to build |
| `JsonFileData`, `TextFileLinesData`, `AlphabeticalOrderer`, `FluentVerifier`, `SupportedOSPlatform` | `src/TestingSupport/`, `src/TestingSupport.XUnit3/` | xUnit v3 extensibility; two near-duplicate packages (section 7) |
| `AutoMockData` (`AutoMockDataAttribute`) | `src/TestingSupport.XUnit3.AutoMoq/` | AutoFixture + Moq theory data |
| `NullEmptyCollectionEquivalencyStep`, `PropertyInfoCollectionAssertions` | `src/TestingSupport.FluentAssertions/` | FluentAssertions extensions |

## 4. Key flows

1. **Guard a parameter.** `value.NotNull(nameof(value))` in the `netstandard2.0` build (`Guard.cs`, name required) or
   `value.NotNull()` in the `net8.0` build (`GuardNet7.cs`, `[CallerArgumentExpression]`). Which file compiles is
   decided by `#if NETSTANDARD2_0` / `NET7_0_OR_GREATER`; a `net6.0` consumer resolves the `netstandard2.0` asset.
2. **Register a services bundle.** `services.AddServicesBundle<T>(configuration)` (`ServicesBundleRegistration.cs`):
   check for null; if the bundle is an `IConfigurationConsumer` (every `ServicesBundle` is), set `Configuration`; call
   `bundle.Configure(services)`, which stores `Services`, configures every entry of `Dependencies` first (each is given
   the **parent's** configuration), then calls the abstract `DoConfigure()`. `ConfigurableServicesBundle` turns that
   into `Configure(IConfiguration)` and throws `InvalidOperationException` when configuration is null. On the generic
   host: `hostBuilder.AddServicesBundle<T>()` (`Common.DependencyInjection.Hosting`, not built by CI).
3. **Run an action through `ActionHandlerManager`** (`ActionHandlerManager.cs`): handlers are ordered by `Priority`
   (lower first; the `OrderBy` is lazy and is evaluated on each run); for each handler: check cancellation, run it,
   record the result; the **first success ends the run** and returns `Success` with all results so far; if none succeeds
   the result is `Failure` with every handler result. Handler exceptions are not caught.
4. **Serialise with System.Text.Json.** `services.AddSystemTextJsonSerializer(options?)` registers one singleton
   `SystemTextJsonSerializer` under five service types. `Serialize(obj)` uses the options as they are. The overloads
   that take an `Action<JsonSerializerOptions>` mutate the shared options in place (section 7).
5. **Discover types.** `TypeLoader.Configure(c => c.WithBaseType<IFoo>().WithAssemblyGlob(...)).LoadTypes<TMarker>()`
   scans one assembly; `AppDomainTypesLoader.ProcessAllAssemblies()` scans every loaded assembly and subscribes to
   `AssemblyLoad`. A type matches when it implements **any** configured base type, then the optional type-name glob is
   applied.
6. **Consume the test support.** A test project references `Ploch.TestingSupport.XUnit3.AutoMoq`; the transitive
   `XUnit3.Dependencies` supplies `xunit.v3`, FluentAssertions, AutoFixture, the test SDK and the runner. Data files
   used by `[JsonFileData]`/`[TextFileLinesData]` resolve against `AppContext.BaseDirectory`, so they must be copied to
   the output.

## 5. Design rules and invariants

- **Exception contract of the guards.** `NotX` methods throw `ArgumentNullException`/`ArgumentException`/
  `ArgumentOutOfRangeException`; `RequiredX` methods throw `InvalidOperationException` (state, not argument, is wrong).
  `PathGuard.RequireValidPath` (netstandard) throws `InvalidOperationException` although its documentation says
  `ArgumentException`. Pinned by `tests/Common.Tests/ArgumentChecking/`.
- **Two guard surfaces.** `NotNullOrDefault` and `RequiredFileExists` exist only for `net7.0+`; `RequireValidPath` only
  for `netstandard2.0`. `tests/Common.Tests` runs on net10.0 and net8.0, and its net8.0 leg is forced onto the
  `netstandard2.0` asset so both surfaces are exercised (`Ploch.Common.Tests.csproj`).
- **Packability.** Anything whose directory starts with `src/` is packable by default; everything under `tests/` is
  forced unpackable; the order of the property groups in `Directory.Build.props` matters (lines 82–104). A fixture
  project under `src/` whose name does not end in `Tests` becomes packable unless it opts out.
- **Dependency direction.** The core and the serialization abstractions reference no project. Implementations reference
  abstractions; DI packages reference implementations; nothing refers back. Test-support libraries reference the core.
- **Configuration hand-off** is a single path: `AddServicesBundle(..., configuration)` → `IConfigurationConsumer` →
  `Configure(IConfiguration)`. A bundle that appears under two parents is configured twice; there is no de-duplication
  and no cycle detection.
- **Versions** are never written in a project file; they are central (see `TECH-STACK.md` section 4).
- **Names.** PackageId, AssemblyName and the project file name are the same string; `RootNamespace` is overridden in two
  projects (section 7).

## 6. Extension points

| To change | Do this |
|---|---|
| Add a guard clause | Add it to both `Guard.cs` (`#if NETSTANDARD2_0`) and `GuardNet7.cs` (`#if NET7_0_OR_GREATER`) unless it is deliberately one-sided, and test both builds |
| Add a library to the CI/release set | Add it to `Ploch.Common.slnx`; CI only builds, tests and packs that file |
| Add a services bundle | Derive from `ServicesBundle` (`DoConfigure()`) or `ConfigurableServicesBundle` (`Configure(IConfiguration)`); list prerequisite bundles in `Dependencies` |
| Add a serializer | Derive from `Serializer<,>`/`AsyncSerializer<,>` and subclass the abstract contract tests in `tests/Common.Serialization.Tests` |
| Add an action handler | Implement `IActionHandler<,>` with a `Priority`; supply the collection to `ActionHandlerManager` yourself (no DI wiring ships) |
| Add a test data source | A concrete `DataAttribute` in both `TestingSupport` packages, or accept divergence (section 7) |

## 7. Known gaps and sharp edges

Cross-referenced from [`BACKLOG.md`](BACKLOG.md) section 6. Items marked `UNVERIFIED` were read but not executed.

| Area | Finding |
|---|---|
| System.Text.Json serializer | `SystemTextJsonSerializer.GetSettings` runs the caller's configuration against the shared options and then sets `UnknownTypeHandling` (`SystemTextJsonSerializer.cs:85`). `JsonSerializerOptions` is immutable after first use, so the set throws `InvalidOperationException` once the options have serialised anything, and always when the options are `JsonSerializerOptions.Default`. With `AddSystemTextJsonSerializer()` (no options) every configuration overload therefore throws. The existing tests pass because each builds a fresh serializer. The docs contradict themselves on this |
| Newtonsoft serializer | Same in-place mutation of shared settings, without the exception. Its DI package has no `ServicesBundle` and no async surface |
| `ActionHandlerManager` | A fallback chain, not a pipeline, and not a dependency resolver (older documentation said so). `ActionExecutionException` is never thrown. Priority ordering, cancellation and multi-handler fall-through are untested |
| `ServicesBundle` | Dependencies are configured with the parent's configuration and may run twice |
| `Common.ObjectBuilder` | `DefaultConverter.CanHandle` returns `true` for everything and `ConvertValue` throws `NotImplementedException`; the `DateTimeOffset` converter is a stub; the test project has no source files; `IObjectTypeConverter` is unused. Runtime behaviour `UNVERIFIED` |
| `AppDomainTypesLoader` | `ProcessAllAssemblies()` subscribes to `AssemblyLoad` on every call and never unsubscribes; `LoadedTypes` is enumerated without the lock although the documentation promises thread safety; `GetTypes()` is not guarded against `ReflectionTypeLoadException`; errors go to `Console.WriteLine` |
| `IsSimpleType` | Returns `true` for every value type (`TypeExtensions.cs:124`), broader than "primitives, strings, decimals, enums and nullables" |
| `EnumerationConverter` | `SafaParseToEnum` is a public typo; its XML example promises `null` for out-of-range input but the code throws. `NullabeTypeConverter` is another public typo. Renaming is a breaking change |
| `Guard` documentation | The `netstandard2.0` remarks claim automatic argument-name capture that those overloads do not have |
| `Common.NSwag` | Does not compile (`string.ToPascalCase` is missing); wrong namespace (`Ploch.Lists.Api.WebApi`), typo in the type name, `Console.WriteLine` in library code |
| `Common.Web` | One source file without a project; uses a Swashbuckle-7 namespace; never compiled |
| Test support | `Ploch.TestingSupport` and `Ploch.TestingSupport.XUnit3` duplicate seven files with drift (newline handling, discovery flags, null checks, an `Async` suffix). The XUnit3 `TextFileLinesData` splits on `Environment.NewLine` only; its `JsonFileData` has no row-length check. `TestPriorityAttribute` is not read by any orderer. `IOAbstractions` does not compile against FluentAssertions 8, and its test project is an xUnit v2 island with no reference to its subject |
| Orphan meta-package | `src/TestingSupport.Dependencies.MetaPackages/*.nuspec` has the id of the live `XUnit3.Dependencies` package but older pins; documentation treats it as current |
| Unused package references | `Dawn.Guard` (deprecated) in `Common.Serialization` and `Common.DependencyInjection`; the file-system abstraction packages in DI; five packages in `Ploch.TestingSupport`. They flow to consumers. `XUnit3`, `AutoMoq` and `Dependencies` force `xunit.v3`, the test SDK and coverage tooling onto every consumer |
| Identity drift | `Common.Apps.Actions.Model` declares `namespace Ploch.Common.Apps.Model` and its `GlobalSuppressions.cs` targets a stale assembly name; `Common.Apps.Shared` declares `Ploch.CommandLine.Spectre.AppInfo`; `Common.Net9` ships net10.0, overrides `RootNamespace` and its namespace is `Ploch.Common.AssemblyLoading`; `IOAbstractions` overrides `RootNamespace` without the `Ploch.` prefix |
| Target frameworks | `Common.DataAnnotations` targets the end-of-life `net6.0`; several libraries hard-code `net8.0` while siblings use `$(TargetFrameworkVersion)`; `TestAssemblies/Common.Tests.TestTypes` is `net6.0` |
| Documentation | Several pages under `docs/libraries/` describe packages that are not published or not built (see `BACKLOG.md`); `docs/libraries/common-net9.md` claims thread safety |

## 8. Where to look

| Question | Open first |
|---|---|
| Which projects does CI build? | `Ploch.Common.slnx`, then `.github/workflows/build-dotnet.yml:75,172,178` |
| Where is a package version set? | `Directory.Packages.props`, then the imported `mrploch-development/dependencies/*.props` |
| Why is a project (not) packable? | `Directory.Build.props:82-104`, then the project file |
| How does a guard behave per target framework? | `src/Common/ArgumentChecking/` (four files) and `tests/Common.Tests/ArgumentChecking/` |
| How do services bundles run? | `src/Common.DependencyInjection/ServicesBundle.cs`, `ServicesBundleRegistration.cs` |
| How are action handlers ordered? | `src/Common.Apps/Apps.Actions/Actions.Model/ActionHandlerManager.cs` |
| Per-library user documentation | `docs/libraries/` (index: `docs/INDEX.md`) |
| What a test attribute does | `src/TestingSupport.XUnit3/TestData/` and `tests/TestingSupport.XUnit3.Tests/` |
