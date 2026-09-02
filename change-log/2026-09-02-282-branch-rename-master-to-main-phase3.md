# Branch rename `master` → `main`, phase 3: drop the dual-accept fallbacks (#282)

Completes the three-phase rename started in #284 (phase 1). The GitHub rename itself was
performed on 2026-09-02; this change removes the `master` fallbacks that phase 1 added so the
repository refers to a single default branch.

## Build and release behaviour

- **`build-dotnet.yml`** — `push` and `pull_request` branch filters narrowed from
  `[ "master", "main" ]` to `[ "main" ]`. The two GitHub Packages publish guards narrowed from
  `(github.ref == 'refs/heads/master' || github.ref == 'refs/heads/main')` to
  `github.ref == 'refs/heads/main'`, and the two `GH_PACKAGES_TOKEN` error messages updated.
- **`publish-docs.yml`** — `push` filter reduced to `main`; header comment updated.
- **`qodana_code_quality.yml`** — `push` filter reduced to `main` (plus `releases/*`, unchanged).
- **`release.yml`** — job guard narrowed to `github.ref == 'refs/heads/main'`. The version-bump
  push already used `HEAD:${GITHUB_REF_NAME}` and needed no change.
- **`version.json`** — `publicReleaseRefSpec` drops `^refs/heads/master$`, leaving
  `^refs/heads/main$` and the `v*.*.*` tag pattern. Builds from `main` continue to be public
  releases; nothing about `main` matching changed.

## Deliberately **not** changed

- **The reserved-branch safety guards** in `publish-docs.yml` and `docs-preview-cleanup.yml`
  (`case "$PREVIEW_BRANCH" in master|main|gh-pages)`) keep `master` in the blocklist. That list
  prevents the preview jobs from force-writing a protected branch name; it is not a trigger.
  Removing `master` would weaken the guard should the name ever be recreated. The corresponding
  passages in `docs/docs-preview.md` are unchanged for the same reason.
- **External repositories' URLs** that legitimately use `master`: `github/gitignore`
  (`.gitignore`, `.syncignore`), `Microsoft/azure-pipelines-vscode` (`my-schema.json`) and
  `DotNetAnalyzers/StyleCopAnalyzers` (`stylecop.json`).
- **`change-log/` history**, which records what was true at the time.

## Documentation

`README.md`, `DocumentationSite/index.md`, `DocumentationSite/docfx.json` (`"branch"`),
`DocumentationSite/articles/samples.md`, `docs/API_REFERENCE.md` (11 permalinks),
`docs/adr/README.md`, `docs/docs-preview.md`, `docs/libraries/common-msbuild.md`, `AGENTS.md`,
`CLAUDE.md`, `src/Common/README.md` (shipped in the `Ploch.Common` package), the two
`ExtensionsDependencyInjection` project comments, and the dormant `.github-test/` copy.

The `version.json` excerpt in `docs/libraries/common-msbuild.md` was re-synced and verified
byte-equal to the real file.

## Notes

No breaking changes. GitHub redirects `blob/master` and `tree/master` links after a branch
rename, so the permalinks were stale rather than broken.
