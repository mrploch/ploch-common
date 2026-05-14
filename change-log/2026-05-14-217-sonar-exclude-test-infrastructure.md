## Exclude build scripts and test-fixture assemblies from SonarCloud coverage

### CI

- **Updated `.github/workflows/build-dotnet.yml`** to add `scripts/**` and `**/TestAssemblies/**` to `sonar.coverage.exclusions`. The full pattern is now:

```
**/*.ps1,**/JetbrainsAnnotations.cs,**/*.Tests/**,**/*.Tests.csproj,scripts/**,**/TestAssemblies/**
```

### Why

After PRs #218 and #219, SonarCloud coverage climbed from `6.77%` to `72.3%` composite (`68.6%` line). Local Rider DotCover and `dotnet-reportgenerator-globaltool` both reported `~86%` line coverage — a remaining `~17`-percentage-point gap.

Querying the SonarCloud API for per-folder coverage revealed the gap was driven by **two folders being measured but not actually meaningful production code**:

| Folder | Lines counted | Uncovered | Coverage |
| --- | --- | --- | --- |
| `scripts/` | 427 | 427 | 0.0% |
| `tests/TestAssemblies/` | 112 | 35 | 68.8% |
| `src/` | 1331 | 125 | 90.6% |

- **`scripts/`** contains build/CI helpers — eight PowerShell files (caught by the existing `**/*.ps1` pattern, correctly excluded) and **`generate-api-reference.py`**, a ~24 KB Python script that Sonar's Python analyzer instruments. With ~400 effective lines reported as uncoverable, this single file accounted for `427 / 587 = 73 %` of the total uncovered-line count.
- **`tests/TestAssemblies/`** holds Reflection test fixtures (`Common.Tests.TestAssembly1`, `Common.Tests.TestAssembly2`, `Common.Tests.TestTypes`) used as instrumentation targets by `Common.Tests`. Despite living under `tests/`, the path does not match the existing `**/*.Tests/**` glob, so it leaked into the coverage measurement.

Neither set of files should drag the coverage badge down: build tooling is not consumer-visible, and test fixtures are intentionally not exhaustively tested.

### Expected impact

With these exclusions in place, the next master analysis should report:

- Covered lines: `1283` (unchanged)
- Total lines: `1870 - 427 - 112 = 1331`
- Line coverage: **`1283 / 1331 ≈ 90.6 %`**

That brings SonarCloud in line with the local `dotnet-reportgenerator` numbers (with-filter `88.7 %`, no-filter `86.2 %`) and the Rider DotCover figure (`~86 %`).

### Refs

- Closes the final part of #217 (SonarCloud reporting artificially low coverage). The Qodana badge issue noted in #217 is still pending — relates to #194 and will be addressed separately.
- Builds on #218 (per-TFM glob fix) and #219 (central Coverlet injection + duplicate-reference cleanup).
