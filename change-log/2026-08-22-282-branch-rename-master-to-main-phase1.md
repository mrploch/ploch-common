## Branch rename `master` → `main`: CI and versioning accept both names (phase 1 of 3)

`ploch-common` is the last repository in the `mrploch` organisation still defaulting to `master`;
every sibling repo (`ploch-data`, `ploch-commandline`, `ploch-lists`, `ploch-endpoints`,
`ploch-ai-tools`, `ploch-ini-parser`, `ploch-tools-editorconfig`, `ploch-dotnet-templates`,
`ploch-github-actions`, `ploch-tiny-tools`, `mrploch-development`) is already on `main`.

This change is the **transitional first phase**: every branch reference in CI and versioning now
accepts **both** names, so the actual rename can happen without a gap in which builds, publishes,
or package versions are wrong. Nothing is renamed yet and no behaviour on `master` changes.

### Changed

- **`.github/workflows/build-dotnet.yml`** — `push` and `pull_request` branch filters accept
  `master` and `main`; both GitHub Packages publish steps now gate on either ref.
- **`.github/workflows/publish-docs.yml`** — GitHub Pages deployment triggers on either branch.
- **`.github/workflows/qodana_code_quality.yml`** — Qodana scan triggers on either branch
  (`releases/*` unchanged).
- **`.github/workflows/release.yml`** — the release job's ref guard accepts either branch, and the
  post-release version bump pushes to `HEAD:${GITHUB_REF_NAME}` instead of the hard-coded
  `master`. That step is now branch-agnostic and needs no further change after the rename.
- **`version.json`** — `publicReleaseRefSpec` gains `^refs/heads/main$` alongside
  `^refs/heads/master$`.
- **`azure-pipelines.yml`** — the legacy Azure DevOps trigger accepts either branch. This
  pipeline is dead configuration — it targets the deleted `Ploch.Common.sln`, .NET SDK 7.x, and
  the stale SonarCloud key `ploch_common`, so it cannot run successfully — but `main` was added
  anyway as zero-cost insurance, so that no branch trigger anywhere in the repository is
  `master`-only. Retiring the file is tracked separately as #283.

### Why `version.json` had to change first

NBGV uses `publicReleaseRefSpec` to decide whether a build is a *public release*. Measured on this
repository at commit `ea77f33`:

| Branch | Matches refspec | `PublicRelease` | `NuGetPackageVersion` |
|---|---|---|---|
| `master` | yes | `True` | `4.0.20-prerelease` |
| a non-matching branch | no | `False` | `4.0.20-prerelease.gea77f33807` |

Renaming the branch before this landed would have moved every `main` build into the second row —
prerelease packages silently gaining a `.g<commit>` suffix and no longer matching the established
`4.0.x-prerelease` shape. Landing the refspec first means there is never a window where it is wrong.

### Why the workflow filters dual-accept rather than switch

`build-dotnet.yml` produces three of this repository's four required status checks
(`Test Results`, `SonarCloud Code Analysis`, `Codacy Static Code Analysis`) and is gated on
`pull_request: branches: [...]`. For `pull_request` events GitHub selects which workflows run using
the workflow files from the merge ref — the pull request's own proposed version. A single change
that switched the filter straight to `main` while still targeting `master` would therefore have
stopped triggering its own required checks, and branch protection would have blocked it
permanently. Accepting both names is safe under any reading of that mechanic.

### Not included

No branch was renamed and no documentation, permalink, or agent-instruction file was updated. Those
are phases 2 and 3 of #282: the rename itself is the repository owner's call, and the cleanup that
removes these `master` fallbacks must land on `main` afterwards.

Two categories are deliberately left alone. Historical `change-log/` entries that mention `master`
record what was true at the time and are not rewritten. The `.github-test/workflows/` copies still
reference `master` only; GitHub reads workflows exclusively from `.github/workflows`, so those
files are inert and are handled in phase 3.

### Refs

- #282 (rename default branch from master to main for org-wide consistency) — phase 1 of 3
- Follow-up: #283 (retire the dead `azure-pipelines.yml`)
