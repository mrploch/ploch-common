## Release pipeline: TestingSupport packages are published to NuGet.org again

### Fixed

- **The release workflow no longer silently drops every `TestingSupport.*` package.** `.github/workflows/release.yml` selected packages to push with `find . -name '*.nupkg' -not -path '*/test*' -not -path '*/Test*'`. The intent was to skip test projects, but `find -path` matches the whole path and `*` spans `/`, so `*/Test*` also matched `./src/TestingSupport*/bin/Release/*.nupkg`. Every `TestingSupport.*` library was therefore excluded from the NuGet.org publish, with no error and no failed check — the only symptom was a package quietly not appearing. Both the `.nupkg` and `.snupkg` selection now exclude `./tests/*` structurally instead.

### Impact on the next release

The next release publishes three packages that previous releases skipped:

| Package | Previously on NuGet.org | Next release |
|---|---|---|
| `Ploch.TestingSupport` | 2.0.1 | current version |
| `Ploch.TestingSupport.FluentAssertions` | 2.0.1 | current version |
| `Ploch.TestingSupport.MockConsoleApp` | never published | first publish |

`Ploch.TestingSupport` and `Ploch.TestingSupport.FluentAssertions` had been frozen at **2.0.1** on NuGet.org across several releases while the repository moved on to 4.0.x. Consumers pinned to those versions will see a two-major jump become available.

Verified against a real `dotnet build -c Release` of the solution: the old filter selected 15 packages, the new one selects 18, and the three added are exactly the `TestingSupport.*` libraries above. `tests/TestAssemblies/Common.Tests.TestTypes` — which is packable only because its project name ends in `TestTypes` rather than `Tests`, and which must not ship — remains excluded.

### Notes

The corrected filter deliberately expresses the exclusion structurally (`./tests/*`) rather than by matching the word `Test` anywhere in a path, and carries a comment recording the original defect so it is not reintroduced.

Two related problems were found while fixing this and are tracked separately rather than folded in: the GitHub Packages publish in `build-dotnet.yml` applies no filter at all and ships test scaffolding to that feed (#279), and `Ploch.TestingSupport.XUnit3` / `.AutoMoq` evaluate to `IsPackable=false` because of an xUnit v3 props file and produce no package at all, so they cannot be published by any pipeline (#280).

### Refs

- #278 (release workflow filter excludes all TestingSupport packages from NuGet.org)
- Follow-ups: #279, #280
- Unblocks the NuGet.org publish of `Ploch.TestingSupport.MockConsoleApp` added in #275 / #276
