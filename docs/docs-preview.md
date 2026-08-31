# Documentation previews for pull requests

Every pull request that touches `docs/`, `DocumentationSite/`, or either docs
workflow gets its DocFX site built automatically and a single sticky comment
pointing at the result, so a change can be judged before it is merged rather than
after.

## For reviewers

Look for the **📖 Documentation preview** comment on the pull request. It is
rewritten in place on every push, so it always describes the current head commit,
and it is replaced with a "preview removed" note once the pull request closes.

The comment links to one of two things:

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
`docs-preview-pr-<N>` artifact from the **Docs Preview** workflow run instead.

## For maintainers

### How production is protected

Previews cannot reach `https://github.ploch.dev/ploch-common/`. Production is
deployed by `publish-docs.yml` using `actions/upload-pages-artifact` plus
`actions/deploy-pages`, which requires the `pages: write` and `id-token: write`
permissions. Neither preview workflow is granted either permission, so neither can
create a GitHub Pages deployment at all — the isolation is enforced by the token,
not by convention. The preview workflows also stay out of the `"pages"` concurrency
group, and write only into `pr-preview/pr-<N>/` on a separate branch.

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
   something other than `gh-pages-previews`.

Unsetting `DOCS_PREVIEW_BASE_URL` reverts to the artifact-only behaviour; nothing
else needs changing.

### Keeping the preview build honest

The preview build runs the same two DocFX commands as `publish-docs.yml`, including
`--warningsAsErrors` on the metadata phase (added in #290, restored in #317). That
guard must stay: a preview that is more permissive than production would let a
change go green here and then fail on `master`. When the remaining `InvalidFileLink`
warnings are cleared (#316) and the build phase is guarded too, guard it in both
workflows in the same change.

Because the preview build mirrors production exactly, it also inherits production's
failures. At the time of writing the metadata guard fails on the CI runner with three
intermittent `Found project reference without a matching metadata reference`
warnings, which is why `publish-docs.yml` is red on `master`; that is tracked in
#329. Fix it in both workflows together — a preview build that is quietly more
lenient than production defeats the point of having one.
