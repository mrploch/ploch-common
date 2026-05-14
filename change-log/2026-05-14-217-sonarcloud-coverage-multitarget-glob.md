## Fix SonarCloud and Codacy coverage on multi-targeted test projects

### CI

- **Updated `.github/workflows/build-dotnet.yml`** so the SonarCloud and Codacy coverage globs pick up per-TFM OpenCover reports emitted by Coverlet when a test project is multi-targeted. Both globs changed from `**/CoverageResults/coverage.opencover.xml` to `**/CoverageResults/coverage*.opencover.xml`.

### Why

After #207 / #209, `Ploch.Common.Tests` was multi-targeted to `net10.0;net8.0` (so CI exercises both the `net8.0` and `netstandard2.0` builds of `Ploch.Common`), and Coverlet started emitting per-TFM filenames for that project:

- `tests/Common.Tests/CoverageResults/coverage.net10.0.opencover.xml`
- `tests/Common.Tests/CoverageResults/coverage.net8.0.opencover.xml`

The previous globs matched only the literal name `coverage.opencover.xml`, so neither file was ingested. SonarCloud therefore saw coverage from the small satellite test projects only and reported `6.77%` instead of the local `~86%`, which tripped the Quality Gate. Codacy was silently affected the same way.

Sonar and Codacy both ingest multiple OpenCover reports and merge them by source file path (set-union of covered lines), so reading both per-TFM reports correctly combines coverage of the `#if NETSTANDARD2_0` and `#if NET7_0_OR_GREATER` branches — which is precisely what #209 set out to achieve.

### Refs

- Closes part of #217 (SonarCloud badges reporting wrong coverage). The Qodana badge problem mentioned in the same issue is tracked separately and relates to #194.
- Builds on #207 / #209 (multi-targeted `Ploch.Common.Tests`).
