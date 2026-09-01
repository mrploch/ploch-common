# PR-preview deployments for the DocFX documentation site

**Issue:** [#201](https://github.com/mrploch/ploch-common/issues/201)
**Type:** CI / documentation tooling
**Breaking change:** No

## What changed

Pull requests that touch `docs/`, `DocumentationSite/`, or either documentation
workflow now get their DocFX site built automatically and a single sticky
**📖 Documentation preview** comment linking to the result. The comment is rewritten
in place on every push and replaced with a "preview removed" note once the pull
request closes or merges.

Two new workflows provide this:

- `.github/workflows/docs-preview.yml` — builds the site on `pull_request`
  (`opened`, `synchronize`, `reopened`) and publishes it under
  `pr-preview/pr-<N>/` on a dedicated preview branch.
- `.github/workflows/docs-preview-cleanup.yml` — removes that directory on
  `pull_request: closed`, so previews cannot accumulate.

A new documentation page, `docs/docs-preview.md`, explains how reviewers use the
preview and how a maintainer enables the hosted lane.

## Isolation from production

The production site at `https://github.ploch.dev/ploch-common/` is deployed by
`publish-docs.yml` through `actions/upload-pages-artifact` and
`actions/deploy-pages`, which require `pages: write` and `id-token: write`. Neither
preview workflow is granted either permission, so neither is capable of creating a
GitHub Pages deployment; for the Pages deployment the isolation is enforced by the
token rather than by convention. The preview workflows also stay out of the
`"pages"` concurrency group and touch no production artefact.

The branch write is a separate concern that the token does *not* constrain —
`contents: write` covers every branch — so both workflows validate the resolved
`DOCS_PREVIEW_BRANCH` before touching anything and hard-fail on `master`, `main`
and `gh-pages`.

## Hosting is opt-in

A repository serves exactly one GitHub Pages site, and this repository's is fed by
the production artifact, so a preview branch is not served automatically. Until a
host is configured the workflow links the downloadable site artifact instead of
posting a URL that would not resolve. Setting the repository variable
`DOCS_PREVIEW_BASE_URL` (and optionally `DOCS_PREVIEW_BRANCH`) switches the comment
to hosted preview URLs, with no other change required.

## Forks and Dependabot

Pull requests raised from forks, and those authored by Dependabot, receive a
read-only `GITHUB_TOKEN`. The build job needs only `contents: read` and therefore
runs for them, giving the same docs build signal; the publishing job detects the
condition and skips with a workflow notice rather than failing. No
`pull_request_target` is used.

## Comment on every outcome

The sticky comment is posted by a separate `if: always()` job, so a pull request
whose docs build fails still gets a comment saying so and linking the run, instead
of silence. The body is selected from the observed result of the build and publish
jobs — including a publish that stood down because the pull request closed mid-run —
so a preview URL is never advertised unless it was actually written.

## Notes

The preview build runs the same two DocFX commands as `publish-docs.yml`, including
`--warningsAsErrors` on the metadata phase (added in #290, restored in #317), so a
preview can never be more permissive than production.
