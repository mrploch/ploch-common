## Inject `coverlet.msbuild` centrally for all test projects

### CI

- **Updated `Directory.Build.props`** so every test project (any project whose name ends with `Tests`) automatically gets a `coverlet.msbuild` `PackageReference`. Previously, individual test projects had to opt in by adding the reference manually, and 6 in-solution test projects were missing it — `dotnet test /p:CollectCoverage=true` was a silent no-op for them.
- **Added `AggregatedCoverage/`** to `.gitignore` so locally-generated merged coverage reports don't get staged.

### Why

After PR #218 fixed the per-TFM glob, SonarCloud climbed from 6.77% to 67.07% — but local Rider DotCover still reported 86%. The remaining gap traced to six in-solution test projects that lack `coverlet.msbuild`:

- `tests/Common.Net9.Tests` (covered `Ploch.Common.Net9.AssemblyLoading.AppDomainTypesLoader` — reported as 0% on Sonar despite having tests)
- `tests/Common.Serialization.Tests`
- `tests/Common.Serialization.NewtonsoftJson.Tests`
- `tests/Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection.Tests`
- `tests/Common.Serialization.SystemTextJson.Tests`
- `tests/Common.Serialization.SystemTextJson.ExtensionsDependencyInjection.Tests`

Coverlet's MSBuild integration ships in the `coverlet.msbuild` package. Without that PackageReference, the `/p:CollectCoverage=true` flag is parsed harmlessly by MSBuild but no instrumentation runs. The CI workflow produced 10 OpenCover reports out of 14 in-solution test projects.

### How

Existing `Directory.Build.props` already classifies projects via:

```xml
<IsTestProject>$(MSBuildProjectName.EndsWith('Tests'))</IsTestProject>
```

Used the same property to inject the package once for every test project:

```xml
<ItemGroup Condition="$(IsTestProject)">
    <PackageReference Include="coverlet.msbuild" PrivateAssets="all" />
</ItemGroup>
```

`coverlet.msbuild` is already pinned at `8.0.0` in `mrploch-development/dependencies/Common.Packages.props`, so Central Package Management handles the version. The 8 test projects that already had a manual reference are now redundant but harmless (CPM dedupes) — leaving them in place to keep the diff minimal; they can be cleaned up in a follow-up.

### Local verification

Ran the exact CI flags locally on `Ploch.Common.slnx` after applying the fix and aggregated the resulting OpenCover reports with `dotnet-reportgenerator-globaltool`:

| Source | Line Coverage |
| --- | --- |
| SonarCloud (before this fix) | 67.07% |
| JetBrains Rider DotCover | 86% |
| Coverlet aggregate (after this fix) | **88.7%** |

`AppDomainTypesLoader` specifically jumped from 0% → 70.2%. The local aggregate landing slightly above Rider's number is expected: PR #209 multi-targets `Common.Tests` so Coverlet runs against the `netstandard2.0` binary too, exercising `#if NETSTANDARD2_0` branches that Rider does not. CI on master should land in the 85–89% range.

### Refs

- Closes the remaining part of #217 (SonarCloud reported coverage is too low). The Qodana badge problem is still pending — relates to #194 and will be addressed separately.
- Builds on #218 (per-TFM glob fix) and #207 / #209 (multi-targeted `Common.Tests`).
