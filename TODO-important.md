# Important Notes

## Issue #210 / #211: Guard cross-TFM alignment

- **[Release]:** Binary-breaking change to the `netstandard2.0` asset of `Ploch.Common` (parameter order of
  `Guard.RequiredNotNull<T>` and `Guard.RequiredNotNullOrEmpty` changed). `version.json` bumped
  `3.1-prerelease` → `4.0-prerelease`; NBGV now computes `4.0.0-prerelease.*`. When cutting the release via
  `release.yml`, use `release_version=4.0`. See `change-log/2026-06-27-210-211-guard-cross-tfm-alignment.md`.

- **[Pre-existing tech debt — out of scope]:** A full `dotnet build Ploch.Common.slnx` emits ~814 analyzer
  warnings, all in **test** projects and all pre-existing on `master` (none from this change — the files
  touched here build clean). CI does not treat test-project warnings as errors. Worth a dedicated cleanup pass.

- **[Orphaned project — out of scope]:** `tests/Common.Net6.Tests` (net6.0, xUnit v2) is **not** referenced by
  `Ploch.Common.slnx` and is not built by CI. It still contains old-order `RequiredNotNull(nameof(x), "...")`
  call sites. Left untouched; consider re-wiring it into the solution and updating its call sites, or deleting it.

- **[Repo hygiene — out of scope]:** `src/Common/Ploch.Common.xml` is a generated XML-doc file tracked in
  source control and appears stale. Likely a leftover from before PR #228 removed `DocumentationFile`. Consider
  untracking it. Untracked `*.cs.bak` files also linger under `tests/Common.Tests/ArgumentChecking/`.
