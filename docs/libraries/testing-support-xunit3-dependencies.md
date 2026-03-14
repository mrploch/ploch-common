# Ploch.TestingSupport.XUnit3.Dependencies

> Meta-package: install one package to get all xUnit v3 test dependencies, analysers, and coverlet coverage support.

## Overview

`Ploch.TestingSupport.XUnit3.Dependencies` is a meta-package — it contains no compilable C# source. Adding it to a test project installs a carefully pinned set of packages that are required for xUnit v3-based testing in the Ploch.Common workspace:

- **xUnit v3** (`xunit.v3`) — the test framework itself
- **AutoFixture** (`AutoFixture.AutoMoq`, `AutoFixture.Xunit3`) — auto-generated test data and Moq integration
- **FluentAssertions** (`FluentAssertions`, `FluentAssertions.Analyzers`) — fluent assertion library and associated Roslyn analyser
- **Moq** (via `AutoFixture.AutoMoq`) — mocking framework
- **Moq.Analyzers** — Roslyn analyser that catches common Moq mistakes at compile time
- **xunit.analyzers** — Roslyn analyser for xUnit best practices
- **xunit.runner.visualstudio** — Visual Studio test runner adapter
- **Microsoft.NET.Test.Sdk** — MSBuild/dotnet test integration
- **coverlet** (`coverlet.msbuild`, `coverlet.collector`) — code coverage collection
- **JetBrains.Annotations** (`PrivateAssets=all`) — code annotation attributes, development-only

The project also copies `xunit.runner.json` to the test project's output directory. This file configures xUnit runner behaviour (e.g. `methodDisplay`, `parallelizeAssembly`) consistently across all test projects.

The project targets `net8.0` (the XUnit3.Dependencies project currently uses a hardcoded `net8.0` target framework, while most other test projects in the workspace use `$(TargetFrameworkVersion)` which resolves to `net10.0`).

## Installation

Add this single reference to your test project instead of listing each dependency individually:

```xml
<ItemGroup>
    <ProjectReference Include="..\..\..\ploch-common\src\TestingSupport.XUnit3.Dependencies\Ploch.TestingSupport.XUnit3.Dependencies.csproj" />
</ItemGroup>
```

Or, when consuming from NuGet:

```shell
dotnet add package Ploch.TestingSupport.XUnit3.Dependencies
```

## Bundled Package Versions

The versions below reflect the pinned versions in the `.nuspec` manifest at the time of writing:

| Package | Version |
|---|---|
| `xunit.v3` | 3.2.2 |
| `AutoFixture.AutoMoq` | 4.18.1 |
| `AutoFixture.Xunit3` | 4.19.0 |
| `FluentAssertions` | 8.8.0 |
| `FluentAssertions.Analyzers` | 0.34.1 |
| `Moq.Analyzers` | 0.4.2 |
| `xunit.analyzers` | 1.27.0 |
| `xunit.runner.visualstudio` | 3.1.5 |
| `Microsoft.NET.Test.Sdk` | 18.3.0 |
| `coverlet.msbuild` | 8.0.0 |
| `coverlet.collector` | 8.0.0 |

> Analysers (`FluentAssertions.Analyzers`, `Moq.Analyzers`, `xunit.analyzers`, `JetBrains.Annotations`) are declared with `PrivateAssets=all` so they do not flow to consuming projects as transitive dependencies.

## Related Libraries

- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — xUnit v3 test helpers that depend on these packages
- [Ploch.TestingSupport.XUnit3.AutoMoq](testing-support-xunit3-automoq.md) — AutoFixture + AutoMoq integration; also depends on this meta-package
- [Ploch.TestingSupport.XUnit2.Dependencies](testing-support-xunit2-dependencies.md) — The equivalent meta-package for xUnit v2 (legacy projects)
- [Ploch.TestingSupport.Dependencies.MetaPackages](testing-support-meta-packages.md) — The `.nuspec` source files used to build this package
