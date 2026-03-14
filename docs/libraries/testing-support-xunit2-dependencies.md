# Ploch.TestingSupport.XUnit2.Dependencies

> Legacy meta-package: single reference for xUnit v2 test dependencies in older projects.

## Overview

`Ploch.TestingSupport.XUnit2.Dependencies` is a meta-package for test projects that have not yet been migrated to xUnit v3. Like its xUnit v3 counterpart, it carries no compilable source and exists only to bundle a curated set of testing packages under a single project reference.

It targets `net6.0` with `SuppressTfmSupportBuildErrors=true`, which allowed it to be consumed by projects on older target frameworks during the workspace's migration period.

> New test projects should use [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md) instead. xUnit v2 is no longer actively maintained and receives only critical fixes.

## Bundled Packages

| Package | Notes |
|---|---|
| `xunit` | xUnit v2 test framework |
| `xunit.analyzers` | Roslyn analyser for xUnit best practices |
| `xunit.runner.visualstudio` | Visual Studio test runner adapter |
| `Xunit.SkippableFact` | Allows `[SkippableFact]` and `[SkippableTheory]` for runtime skip conditions |
| `FluentAssertions` | Fluent assertion library |
| `FluentAssertions.Analyzers` | Roslyn analyser for FluentAssertions usage |
| `Microsoft.NET.Test.Sdk` | MSBuild/dotnet test integration |
| `coverlet.msbuild` | Code coverage collection via MSBuild |

## Installation

```xml
<ItemGroup>
    <ProjectReference Include="..\..\..\ploch-common\src\TestingSupport.XUnit2.Dependencies\Ploch.TestingSupport.XUnit2.Dependencies.csproj" />
</ItemGroup>
```

## Migration

To migrate a test project from xUnit v2 to xUnit v3:

1. Replace the reference to `Ploch.TestingSupport.XUnit2.Dependencies` with `Ploch.TestingSupport.XUnit3.Dependencies`.
2. Update the `TargetFramework` to `net8.0` or later.
3. Replace `[SkippableFact]` / `[SkippableTheory]` with the `[SupportedOSPlatform]` attribute from [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) for OS-conditional skipping, or use xUnit v3's built-in `Skip` property on `[Fact]` and `[Theory]`.
4. Review data attributes — xUnit v3's `ITheoryDataRow` API differs from v2's `IEnumerable<object[]>`.

## Related Libraries

- [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md) — Recommended replacement for new and migrated projects
- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — xUnit v3 helpers including the OS-conditional skip attribute
