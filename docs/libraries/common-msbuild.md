# Build Configuration (MSBuild)

> Repository-wide MSBuild infrastructure for versioning, packaging, code analysis, and centralised dependency management.

## Overview

The `ploch-common` repository uses a layered set of MSBuild files to centralise build behaviour across all projects in the solution. Rather than repeating settings in individual `.csproj` files, the configuration is declared once and inherited automatically by every project in the repository.

The infrastructure covers five concerns: project defaults (nullable, language version, output type), NuGet packaging, automatic versioning via Nerdbank.GitVersioning, source debugging via SourceLink, and centralised package version management. Analyser enforcement is layered on top through shared `.props` files imported from the sibling `mrploch-development` repository.

Note: `src/Common.MSBuild/` is a placeholder directory reserved for a future custom MSBuild tasks project. No deliverable content exists there yet. The documentation below describes the build infrastructure that is currently active.

## Key Files

| File | Purpose |
|------|---------|
| `Directory.Build.props` | Inherited by every project; sets defaults for packaging, documentation, nullable, analysers, versioning tools |
| `Directory.Packages.props` | Enables Central Package Management (`ManagePackageVersionsCentrally=true`) and imports shared version definitions |
| `version.json` | Nerdbank.GitVersioning (NBGV) configuration; controls version scheme, public release branches, and NuGet SemVer 2 format |
| `../mrploch-development/dependencies/*.props` | Shared package version definitions and global analyser references imported from the workspace sibling |

## Directory.Build.props

`Directory.Build.props` is automatically imported by MSBuild for every project under the repository root. It sets the following properties:

### Project identity

```xml
<Authors>Kris Ploch</Authors>
<Company>Ploch</Company>
<Product>Ploch.Common</Product>
<PackageProjectUrl>https://common.github.ploch.dev/</PackageProjectUrl>
<RepositoryUrl>https://github.com/mrploch/ploch-common.git</RepositoryUrl>
<Copyright>Kris Ploch $([System.DateTime]::Now.Year)</Copyright>
<PackageLicenseExpression>Apache-2.0</PackageLicenseExpression>
```

### Code quality

```xml
<Nullable>enable</Nullable>
<LangVersion>default</LangVersion>
<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
<EnableNETAnalyzers>true</EnableNETAnalyzers>
<AnalysisLevel>latest-Recommended</AnalysisLevel>
```

### Test project detection

Test projects are identified automatically by name suffix. The condition `$(MSBuildProjectName.EndsWith('Tests'))` is evaluated at import time:

```xml
<IsTestProject>$(MSBuildProjectName.EndsWith('Tests'))</IsTestProject>
```

Test projects have packaging and XML documentation disabled; non-test projects have both enabled:

```xml
<!-- Test projects -->
<GeneratePackageOnBuild>false</GeneratePackageOnBuild>
<GenerateDocumentationFile>false</GenerateDocumentationFile>
<IsPackable>false</IsPackable>

<!-- Library projects -->
<GeneratePackageOnBuild>true</GeneratePackageOnBuild>
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

### Versioning and source debugging

Every project automatically gets Nerdbank.GitVersioning and SourceLink references via `GlobalPackageReference`:

```xml
<PackageReference Include="Nerdbank.GitVersioning" PrivateAssets="all" />
<PackageReference Include="Microsoft.SourceLink.GitHub" PrivateAssets="all" />
```

SourceLink is configured to embed source information in symbols:

```xml
<PublishRepositoryUrl>true</PublishRepositoryUrl>
<EmbedUntrackedSources>true</EmbedUntrackedSources>
<IncludeSymbols>true</IncludeSymbols>
<SymbolPackageFormat>snupkg</SymbolPackageFormat>
```

Deterministic builds are activated automatically in CI:

```xml
<ContinuousIntegrationBuild Condition="'$(CI)' == 'true'">true</ContinuousIntegrationBuild>
```

## Directory.Packages.props

Central Package Management is enabled globally:

```xml
<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
```

With this enabled, individual `.csproj` files declare `<PackageReference>` elements **without** a `Version` attribute. Versions are resolved centrally from this file and the imported `.props` files. Adding a version attribute to a project-level reference is a build error.

The file imports five shared version catalogues from `mrploch-development/dependencies/`:

| Imported file | Contents |
|---------------|----------|
| `MicrosoftExtensions.Net9.Packages.props` | EF Core, Hosting, DI, Configuration, ASP.NET Core |
| `Ploch.Packages.props` | Ploch organisation packages (GenericRepository, Data.Model) |
| `Common.Packages.props` | Ardalis, AutoMapper, FastEndpoints, Newtonsoft.Json, Dawn.Guard, etc. |
| `Testing.Packages.props` | xUnit, FluentAssertions, Moq, AutoFixture, Coverlet |
| `Analyzers.Global.Packages.props` | StyleCop, Roslynator, SonarAnalyzer, MS NetAnalyzers (as `GlobalPackageReference`) |

The two repository-specific versions pinned directly in `Directory.Packages.props` are:

```xml
<PackageVersion Include="Nerdbank.GitVersioning" Version="3.7.115" />
<PackageVersion Include="Microsoft.SourceLink.GitHub" Version="8.0.0" />
```

## Nerdbank.GitVersioning (version.json)

Version numbers are derived from the git history using [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning). The `version.json` file at the repository root defines the version policy:

```json
{
  "$schema": "https://raw.githubusercontent.com/dotnet/Nerdbank.GitVersioning/main/src/NerdBank.GitVersioning/version.schema.json",
  "version": "3.1-prerelease",
  "assemblyVersion": {
    "precision": "revision"
  },
  "versionHeightOffset": -1,
  "nuGetPackageVersion": {
    "semVer": 2.0
  },
  "publicReleaseRefSpec": [
    "^refs/heads/master$",
    "^refs/tags/v\\d+\\.\\d+\\.\\d+"
  ],
  "cloudBuild": {
    "buildNumber": {
      "enabled": true
    }
  }
}
```

Key points:

- The base version is `3.1`. NBGV appends the commit height as a third segment automatically (e.g. `3.1.42`). The `versionHeightOffset` of `-1` shifts the computed commit height down by one.
- Builds from `master` or a tag matching `v\d+\.\d+\.\d+` are treated as public releases; all other branches produce pre-release packages.
- Assembly versions use four-part precision (`Major.Minor.Patch.Revision`).
- NuGet packages use SemVer 2 format, which enables pre-release metadata such as `3.1.42-g1a2b3c4`.
- Cloud build number stamping is enabled so the CI build number reflects the calculated version.

To inspect the current computed version locally:

```bash
nbgv get-version
```

To prepare a release commit (sets the version and creates a tag):

```bash
nbgv prepare-release
```

## Shared Analyser Configuration

The `Analyzers.Global.Packages.props` file imported via `Directory.Packages.props` uses `GlobalPackageReference` to inject the following analysers into **every** project in the solution without any per-project declaration:

| Analyser | Purpose |
|----------|---------|
| `StyleCop.Analyzers` | Enforces code style and documentation conventions |
| `Roslynator.Analyzers` | Broad set of code quality and refactoring diagnostics |
| `SonarAnalyzer.CSharp` | SonarQube rules for bug detection and code smell identification |
| `Microsoft.CodeAnalysis.NetAnalyzers` | Official .NET platform analysers |
| `Microsoft.VisualStudio.Threading.Analyzers` | Detects async/await and threading anti-patterns |
| `codecracker.CSharp` | Additional C# code quality rules |

All analyser packages are configured with `PrivateAssets=all` so they are build-time only and do not appear as transitive dependencies in published packages.

## Build Diagnostics Target

`Directory.Build.props` includes a `PrintSettings` target that runs before every build and logs key MSBuild property values at high importance. This aids in diagnosing unexpected build behaviour in CI:

```text
Building Ploch.Common with settings:
MSBuildProjectName -> 'Ploch.Common'
Configuration -> 'Release'
IsTestProject -> 'False'
GeneratePackageOnBuild -> 'True'
GenerateDocumentationFile -> 'True'
IsPackable -> 'True'
```

## Configuration

No installation step is required. The configuration files are repository-level infrastructure. Any project added to the solution automatically inherits all settings by virtue of MSBuild's directory-traversal import rules for `Directory.Build.props` and `Directory.Packages.props`.

To add a new package version to the central catalogue, add a `<PackageVersion>` entry to `Directory.Packages.props` (for repository-specific packages) or to the appropriate file in `mrploch-development/dependencies/` (for workspace-wide versions).

To change the base version, edit the `"version"` field in `version.json`.

## Related Libraries

- [Ploch.Common](common.md) — Core utility library
- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — ServicesBundle pattern for modular DI registration
