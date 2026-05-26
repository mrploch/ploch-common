## Remove redundant explicit `<DocumentationFile>` properties and gate `PrintSettings` target

### Build configuration

- **Removed `<DocumentationFile>$(OutputPath)$(AssemblyName).xml</DocumentationFile>` from every library project.** The central `<GenerateDocumentationFile>true</GenerateDocumentationFile>` in `Directory.Build.props` (already in effect for all non-test projects) is sufficient on the modern .NET SDK — the SDK auto-generates the XML doc file next to the assembly and packs it into the `.nupkg`. The explicit per-project line was a pre-SDK-style holdover.
- Affected projects: `Ploch.Common`, `Ploch.Common.AppServices`, `Ploch.Common.DawnGuard`, `Ploch.Common.DependencyInjection`, `Ploch.Common.Serialization`, `Ploch.Common.Serialiation.NewtonsoftJson`, `Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection`, `Ploch.Common.Serialization.SystemTextJson`, `Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection`, `Ploch.Common.WebUI`, `Ploch.TestingSupport`, `Ploch.TestingSupport.FluentAssertions`.
- `Ploch.Common.Serialization` additionally had configuration-specific `<DocumentationFile>bin\Debug\…xml</DocumentationFile>` and `<DocumentationFile>bin\Release\…xml</DocumentationFile>` overrides; the entire Debug/Release `PropertyGroup`s collapsed to nothing once the property was removed and were deleted.

### Test-fixture assemblies

- `Ploch.Common.Tests.TestAssembly1` and `Ploch.Common.Tests.TestAssembly2` are not auto-detected as test projects by the `MSBuildProjectName.EndsWith('Tests')` rule (their names end in `TestAssembly1`/`2`). They previously used `<DocumentationFile />` (empty) inside Debug/Release `PropertyGroup`s as a workaround to suppress the centrally-enabled XML doc generation. Replaced with a single explicit `<GenerateDocumentationFile>false</GenerateDocumentationFile>` to mirror the test-project convention — these are fixture assemblies used only for reflection-based tests and never published.

### Build log noise

- **Gated the `PrintSettings` debug `<Target>` behind a `PrintBuildSettings` opt-in property.** Previously it emitted ~10 `Importance="high"` `<Message>` lines on every build of every project, padding CI logs with ~270 lines per full build (27 projects × 10 messages). Now off by default; enable for a targeted diagnostic build with `-p:PrintBuildSettings=true`.

### Documentation

- Updated `docs/libraries/common-msbuild.md` § "Build Diagnostics Target" to describe the new opt-in behaviour and the enable command.

### Why

Surfaced while reviewing #117 (Feb 2024 stale PR that tried to *change* `<DocumentationFile>` to use `$(BaseOutputPath)` — that path drops the TFM subfolder, so multi-targeted projects would have collided both inner builds' XML doc onto one path). #117 was closed; this issue tracks the *correct* cleanup: remove the redundant property entirely rather than tweaking its value.

The `PrintSettings` target was useful when diagnosing the unexpected build behaviour that motivated the central `GenerateDocumentationFile=true` switch (see issue #117 history), but on a clean configuration it adds only log noise. Gating it behind `PrintBuildSettings` preserves the diagnostic capability for on-demand use without polluting every CI build.

### Out of scope

- DocFX-driven API site (`DocumentationSite/docfx.json`) is unaffected — it extracts metadata from `*.csproj` source via Roslyn, not from `bin/*.xml`. The published site at <https://common.github.ploch.dev/> continues to work.
- Several library `.csproj` files still carry `<None Remove="Ploch.<Name>.xml" />` items. These were companion workarounds for the explicit `<DocumentationFile>` paths; with the SDK now writing XML to the intermediate output directory they are likely no-ops, but verifying and removing them is a follow-up cleanup, not part of this issue's scope.
- `templates/Directory.Build.props` is a frozen legacy template (uses the pre-NBGV `VersionPrefix`/`BuildNumber` pattern) and was not modified.

### Verification

- `dotnet build Ploch.Common.slnx -c Release` — zero new warnings, zero errors. CI logs no longer contain per-project `PrintSettings` dumps.
- Each library `.nupkg` continues to ship `lib/<tfm>/<Assembly>.xml` per target framework (the SDK auto-derives the path from `GenerateDocumentationFile=true`).
