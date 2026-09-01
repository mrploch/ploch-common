# PR-preview deployments for the DocFX documentation site

**Issue:** [#201](https://github.com/mrploch/ploch-common/issues/201)
**Type:** CI / documentation tooling
**Breaking change:** No

## What changed

Every pull request now gets its DocFX site built automatically and a single sticky
**📖 Documentation preview** comment linking to the result. The comment is rewritten
in place on every push and replaced with a "preview removed" note once the pull
request closes or merges.

The preview is not restricted to pull requests that edit `docs/` or
`DocumentationSite/`. Most of the site is API reference generated from XML
documentation comments in the C# sources, so a pull request that touches no Markdown
at all can still change dozens of pages.

The work lives in two places:

- `.github/workflows/publish-docs.yml` gains two pull-request-only jobs —
  `publish-preview`, which pushes the built site under `pr-preview/pr-<N>/` on a
  dedicated preview branch, and `comment-preview`, which posts the sticky comment.
- `.github/workflows/docs-preview-cleanup.yml` (new) removes that directory on
  `pull_request: closed`, so previews cannot accumulate.

A new documentation page, `docs/docs-preview.md`, explains how reviewers use the
preview and how a maintainer enables the hosted lane.

## One build, not two

The preview was first implemented as a standalone `docs-preview.yml` with its own
DocFX build. [#330](https://github.com/mrploch/ploch-common/pull/330) then added a
`pull_request` trigger to `publish-docs.yml`, which made that a second build of the
same commit producing the same output — and the two copies had already drifted in a
way that mattered: the preview still passed `--warningsAsErrors` to
`docfx metadata`, the exact flag #330 had to replace with a narrower gate because
DocFX 2.78.5 always emits workspace warnings on the Linux runner
([#329](https://github.com/mrploch/ploch-common/issues/329)).

So the preview jobs were folded into `publish-docs.yml` and `docs-preview.yml` was
deleted. There is now one `build-docs` job feeding both the production Pages
deployment and the preview, which halves the runner time on a documentation pull
request and makes it structurally impossible for a preview to be built under
different rules from production.

## Isolation from production

The production site at `https://github.ploch.dev/ploch-common/` is deployed by the
`deploy-docs` job through `actions/upload-pages-artifact` and `actions/deploy-pages`,
which require `pages: write` and `id-token: write`. Neither preview job is granted
either permission, and `deploy-docs` is itself gated on
`github.event_name != 'pull_request'` — so the Pages deployment is protected twice
over, by the token and by the trigger, rather than by convention. The preview jobs
also stay out of the `"pages"` concurrency group and touch no production artefact.

The branch write is a separate concern that the token does *not* constrain —
`contents: write` covers every branch — so both `publish-docs.yml` and
`docs-preview-cleanup.yml` validate the resolved `DOCS_PREVIEW_BRANCH` before
touching anything and hard-fail on `master`, `main` and `gh-pages`.

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
