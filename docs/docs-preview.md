# Documentation previews for pull requests

Every pull request gets its DocFX site built automatically and a single sticky
comment pointing at the result, so a documentation change can be judged before it is
merged rather than after.

The preview is not limited to pull requests that edit `docs/` or
`DocumentationSite/`. Most of the site is API reference generated from XML
documentation comments in the C# sources, so a pull request that touches no Markdown
at all can still change dozens of pages.

## For reviewers

Look for the **📖 Documentation preview** comment on the pull request. It is
rewritten in place on every push, so it always describes the current head commit,
and it is replaced with a "preview removed" note once the pull request closes.

The comment is posted whatever the outcome — the commenting job runs with
`if: always()`, so a pull request whose docs build *broke* gets a comment saying so
and linking the run, rather than silence. It links to one of these:

- **A note that the build failed**, with a link to the workflow run. There is no
  preview to look at; fix the DocFX failure and push again.
- **A hosted preview URL**, when the repository has a preview host configured.
  Follow it and walk the site the way a reader would: home page → the Libraries
  entry in the sidebar → an API page → the search box. Sub-path hosting is the
  thing most likely to break, and those four steps exercise it.
- **A downloadable site artifact**, when no preview host is configured. Download
  it from the linked workflow run, unzip it, and open `index.html`. DocFX emits
  relative links, so the site works straight from the file system.

Pull requests raised from a fork, and those opened by Dependabot, get no comment:
GitHub gives those runs a read-only token, so nothing can be posted or published.
The site is still built and uploaded on those runs — download the
`docs-preview-pr-<N>` artifact from the **Publish Docs** workflow run instead.

## For maintainers

### Where the preview lives

Everything is in `.github/workflows/publish-docs.yml`, alongside the production
build and deployment, plus `.github/workflows/docs-preview-cleanup.yml` for removing
a preview when its pull request closes.

The preview was originally its own workflow with its own DocFX build. That was
consolidated in #201 once #330 added a `pull_request` trigger to `publish-docs.yml`:
two copies of the same build on the same commit is not only twice the runner time,
it is a copy that goes stale silently. It already had — the preview copy still passed
`--warningsAsErrors` to `docfx metadata`, the exact flag #330 had to replace with a
narrower grep-based gate because DocFX 2.78.5 always emits workspace warnings on the
Linux runner (#329). There is now one build job, `build-docs`, and four jobs total:

| Job | Runs on | Permissions |
|---|---|---|
| `build-docs` | every push to `master` and every pull request | `contents: read` |
| `deploy-docs` | pushes only (`github.event_name != 'pull_request'`) | `pages: write`, `id-token: write`, `actions: read`, `contents: read` |
| `publish-preview` | pull requests only | `contents: write` |
| `comment-preview` | pull requests only, `if: always()` | `pull-requests: write` |

### How production is protected

Previews cannot reach `https://github.ploch.dev/ploch-common/`. Production is
deployed by `deploy-docs` using `actions/upload-pages-artifact` plus
`actions/deploy-pages`, which requires the `pages: write` and `id-token: write`
permissions. Neither preview job is granted either permission, and `deploy-docs` is
itself gated on the event not being a pull request — so for the *Pages deployment*
the isolation is enforced twice over, by the token and by the trigger, not by
convention. The preview jobs also stay out of the `"pages"` concurrency group, and
write only into `pr-preview/pr-<N>/` on a separate branch.

The **branch write is a different matter**, and the token does not protect it:
`contents: write` grants write access to every branch in the repository, so the only
thing separating the preview lane from a real branch is the value of
`DOCS_PREVIEW_BRANCH`. Both `publish-docs.yml` and `docs-preview-cleanup.yml`
therefore validate the resolved branch name before touching anything and hard-fail on
`master`, `main` and `gh-pages`:

```bash
case "$PREVIEW_BRANCH" in master|main|gh-pages) echo "::error::Refusing to publish previews to $PREVIEW_BRANCH"; exit 1;; esac
```

This matters because the publishing step deletes the preview directory recursively,
stages the whole worktree with `git add -A`, and runs
`git push HEAD:$PREVIEW_BRANCH`. Pointing `DOCS_PREVIEW_BRANCH` at a branch that
anything else serves or reads would overwrite it. Set it only to a branch that exists
solely for previews, and keep the reserved list in the two workflows identical.

The publishing and cleanup jobs do hold `contents: write`, and a `pull_request`
workflow grants that only to same-repository pull requests — that is, to people who
already have push access. A contributor who could subvert those jobs by editing the
workflow could equally push to a branch directly, so this does not widen the trust
boundary. It is nonetheless why the eligibility gate exists, why the publishing job
checks nothing out, and why `pull_request_target` is not used: that trigger would
extend the same write token to forks.

### Enabling the hosted preview lane

A repository has exactly one GitHub Pages site, and this repository's is fed by the
production artifact, so the preview branch is not served out of the box. Until a
host is configured, the workflow deliberately links the build artifact rather than
posting a URL that would not resolve.

To turn hosted previews on:

1. Choose a host that can serve **this repository's** `gh-pages-previews` branch.
   The workflow pushes into the repository it runs in and has no cross-repository
   credentials, so a second GitHub repository is only viable with a mirroring step
   and a token that is not in scope here. An external static host (Cloudflare Pages,
   Netlify, and similar all support serving an arbitrary branch) is the
   straightforward option.
2. Set the repository variable `DOCS_PREVIEW_BASE_URL` to the base URL of that host.
   The workflow appends `/pr-preview/pr-<N>/`.
3. Optionally set `DOCS_PREVIEW_BRANCH` if the preview branch should be named
   something other than `gh-pages-previews`. It **must** name a branch that exists
   only to carry previews — the workflows rewrite the whole branch on every publish
   and reject `master`, `main` and `gh-pages` outright (see
   [How production is protected](#how-production-is-protected)).

Unsetting `DOCS_PREVIEW_BASE_URL` reverts to the artifact-only behaviour; nothing
else needs changing.

### Keeping the preview build honest

There is nothing to keep in step: the preview *is* the production build. The site a
reviewer looks at is byte-for-byte the artifact `deploy-docs` would publish, produced
by the same `docfx metadata` and `docfx build` invocations under the same warning
gate. A change cannot go green in the preview and then fail on `master`, because
there is only one build to go green.

That is the property to preserve. If a future change needs the preview to build
differently from production — a different DocFX configuration, a relaxed warning
gate, an extra step — treat that as a reason to reconsider the change, not as a
reason to fork the build job again.

When the remaining `InvalidFileLink` warnings are cleared (#316) and the build phase
is guarded as well as the metadata phase, that is a single edit to one step.
