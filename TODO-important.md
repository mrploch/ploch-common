# Important Notes

The follow-ups originally captured here from the #210 / #211 Guard cross-TFM alignment work are
now tracked as GitHub issues. This file points at them so nothing is lost.

## Tracked follow-ups (from #210 / #211 cleanup)

- **#232** — *release:* Cut `Ploch.Common` v4.0 stable. Binary-breaking change to the `netstandard2.0`
  asset (parameter order of `Guard.RequiredNotNull<T>` / `Guard.RequiredNotNullOrEmpty` changed).
  `version.json` is on `4.0-prerelease`; when cutting the release via `release.yml`, use
  `release_version=4.0`. See `change-log/2026-06-27-210-211-guard-cross-tfm-alignment.md`.

- **#233** — *test:* Clean up the ~814 pre-existing analyzer warnings across the test projects (all
  pre-existing on `master`, none from the #210/#211 change; CI does not fail on test-project warnings).

- **#234** — *chore:* Untrack the stale generated `src/Common/Ploch.Common.xml` and remove leftover
  `*.cs.bak` files under `tests/Common.Tests/ArgumentChecking/`.

- **#235** — *test:* Remove the orphaned `tests/Common.Net6.Tests` project — **being done now** (this PR).
  The project was not in any solution and never built by CI; its coverage is duplicated by the maintained
  xUnit v3 suites (`Common.Tests/ArgumentChecking/GuardTests.cs`, `Common.Tests/Collections/EnumerableExtensionsTests.cs`).
  Note: PR #231 (commit `1e236a6`) had already updated its `Guard` call sites to the new order, so the
  earlier "stale old-order call sites" framing no longer applied — the orphan status is the reason for removal.

- **#236** — *chore:* Decide the fate of `src/TestingSupport.XUnit2.Dependencies` (a published xUnit v2
  meta-package). Surfaced by #235: it is not in any solution and, once `Common.Net6.Tests` is gone, has no
  in-repo consumers. Either delete it (if unpublished/unused) or re-home it into `Ploch.Common.slnx` so CI
  builds and packs it.
