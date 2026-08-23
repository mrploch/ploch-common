## Build: repair the projects that blocked DocFX metadata extraction

Five projects outside `Ploch.Common.slnx` did not compile. Because DocFX globs
`src/**/*.csproj` from the filesystem rather than reading the solution, every docs
build reported "Build failed" and exited non-zero regardless of the rest of the run.
PR #202 worked around this with temporary `docfx.json` excludes; those are now lifted
for the shipping WebApi libraries.

### `Ploch.Common.Maui`

- **Fixed a `NullReferenceException` in the `x:NameOf` markup extension.** `NameOfExtension.ProvideValue`
  guaranteed that *either* a matching property *or* a matching field existed, then unconditionally
  dereferenced the property. Resolving a **field** by name threw instead of returning its name.
  The lookup now short-circuits on the property and falls through to the field.
- `BaseContentView` no longer dereferences `Application.Current` (nullable in .NET MAUI) or a
  null `ViewModel` when the view appears.

### `Ploch.Common.Maui.Tests.TestAssembly1` / `TestAssembly2`

- Added the missing `ProjectReference` to `Ploch.Common.Maui`. The two fixture assemblies referenced
  `BaseViewModel`, `BaseContentPage`, `IViewModel` and `IView` without referencing the project that
  defines them. The `Ploch.Lists.UI.MauiUI.ViewModels` namespace they import was **not** stale —
  `IViewModel` really is declared under that namespace inside `Ploch.Common.Maui` (see the follow-up issue).
- `Ploch.Common.Maui.Tests` gained `xunit.runner.visualstudio`, without which its four
  `TypeDiscoverer` tests were built but never discovered or executed.

### `Ploch.Common.WebApi`

- **Resolved a hard package conflict.** `Microsoft.AspNetCore.OpenApi` 10.0.5 requires
  `Microsoft.OpenApi` 2.x, where `OpenApiInfo` moved out of the `Microsoft.OpenApi.Models`
  namespace, while `Swashbuckle.AspNetCore.SwaggerGen` 7.2.0 was still compiled against
  `Microsoft.OpenApi` 1.6.x. Swashbuckle is now pinned to 10.2.3 for this project, which unifies
  both packages on `Microsoft.OpenApi` 2.7.5.
- **Cleared a high-severity advisory.** The previously resolved `Microsoft.OpenApi` 2.0.0 was subject
  to [GHSA-v5pm-xwqc-g5wc](https://github.com/advisories/GHSA-v5pm-xwqc-g5wc) (NU1903); 2.7.5 is not.
- The project now builds as a class library (`Microsoft.NET.Sdk` + a framework reference) instead of
  using the Web SDK. Packaging stays explicitly disabled: both CI workflows glob `**/*.nupkg`, so
  enabling it would publish a package that has never shipped. Whether to ship it is tracked in #285.
- Removed an unreachable `OperationIdSelector` that was overwritten by the following
  `CustomOperationIds` call, and which referenced a non-existent `ToPascalCase` extension.
- Added XML documentation and argument guards to `ConfigureOpenApiOptions`.

### `QueryStringBinder`

- **Fixed a `KeyNotFoundException`.** Binding any type with a property absent from the query string
  threw, because the binder used the `IDictionary` indexer instead of `TryGetValue`.
- **Fixed culture-dependent parsing.** Numbers, dates and times are now parsed with
  `CultureInfo.InvariantCulture`; previously a query string was interpreted differently depending on
  the server's locale (`03/04/2024` meant March in one locale and April in another).
- Null query values are skipped rather than dereferenced, and a blank value (`?page=`) leaves a
  non-string property at its default instead of throwing `FormatException` from `int.Parse("")`.
- A property type that cannot be bound is now detected even when the caller omits it from the query
  string; previously an absent key skipped the check, so `TryParse` reported success for a type it
  documents as unsupported.
- Conversions use the `Try*` pattern, so a malformed value raises a `FormatException` naming the
  property and expected type (`Query string value 'abc' for property 'Page' is not a valid Int32.`)
  rather than the framework's message, which echoed only the bad input.
- Added `tests/Common.WebApi.Tests` with 13 tests covering every supported property type plus the
  two regressions above. The project is registered in `Ploch.Common.slnx`, so CI builds
  `Ploch.Common.WebApi` and runs these tests — the class of rot behind this issue was precisely that
  none of these projects were in the solution.

### `Ploch.Common.WebApi.Endpoints.CrudEndpoints`

- `GetByIdEndpointHandler` no longer treats the `IMemoryCache.TryGetValue` out-parameter as
  non-nullable, which had blocked compilation of this project and the two framework integrations
  that depend on it.

### Documentation build

- `DocumentationSite/docfx.json` no longer excludes `**/Common.WebApi/**`, so `Ploch.Common.WebApi`
  and `Ploch.Common.WebApi.Endpoints` are documented again.
- The CrudEndpoints family (`WebApi.Endpoints.CrudEndpoints` and its FastEndpoints and
  MinimalApiEndpoints integrations) stays excluded for a different reason: those projects
  `ProjectReference` two projects in the sibling `ploch-data` repository, and no workflow clones it,
  so DocFX cannot resolve them on a CI runner. Documenting them needs either that clone step or a
  switch to package references — tracked separately.
- The test-project excludes remain: test assemblies do not belong in the published API reference.
