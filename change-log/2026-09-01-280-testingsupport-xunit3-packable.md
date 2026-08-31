## `Ploch.TestingSupport.XUnit3` family: the packages are now produced and can be published

**Fixed three shipping libraries evaluating to `IsPackable=false`, so no `.nupkg` was ever built for
them.** `Ploch.TestingSupport.XUnit3`, `Ploch.TestingSupport.XUnit3.AutoMoq` and
`Ploch.TestingSupport.XUnit3.Dependencies` are documented for consumption from other repositories
(`dotnet add package …`), but a `Release` build of the solution produced no package for any of them,
so none could reach NuGet.org or GitHub Packages. A `Release` build now produces all three.

### Root cause

Nothing in this repository set `IsPackable=false` for these projects. `Directory.Build.props`
classifies a project as a test project by name (`$(MSBuildProjectName.EndsWith('Tests'))`), and none
of the three matches, so all three took the non-test branch — which set `GeneratePackageOnBuild=true`
and `GenerateDocumentationFile=true` but left `IsPackable` **empty**. That handed the decision to the
SDK, where `NuGet.Build.Tasks.Pack.targets` applies its own default:

```xml
<IsPackable Condition="'$(IsPackable)'=='' AND ('$(IsTestProject)'=='true' OR '$(IsTestingPlatformApplication)'=='true')">false</IsPackable>
```

Any library that references xUnit v3 inherits `IsTestingPlatformApplication=true` from the
Microsoft.Testing.Platform build props — and these libraries must reference xUnit v3, because
providing xUnit v3 support is what they are for. The SDK therefore silently classified three shipping
packages as test applications. The repository's own name heuristic was never the trigger; the missing
`IsPackable` was.

`Directory.Build.props` now states `IsPackable=true` on the non-test branch, so the intent is
explicit and the SDK default no longer applies. Test projects are unaffected: they take the
`$(IsTestProject)` branch, which already sets `IsPackable=false`. A project that opts out in its own
`.csproj` also still wins, because `Directory.Build.props` is imported first.

`Ploch.TestingSupport.XUnit3.Dependencies` additionally excludes the `testhost.dll` / `testhost.exe`
content items that `Microsoft.NET.Test.Sdk` injects, which would otherwise be packed — raising NU5100
and shipping a duplicate test host that consumers already receive through the
`Microsoft.NET.Test.Sdk` dependency the meta-package declares.

### Compatibility

Additive. No public API, package identifier or behavioural contract changed, and no package that was
previously produced is affected. Three packages that could not previously be published now can be;
the release workflow's package filter (`find . -name '*.nupkg' -not -path './tests/*'`) already
includes them, so no workflow change is required.

Refs: #280
