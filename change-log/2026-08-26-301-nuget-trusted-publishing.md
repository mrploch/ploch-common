# NuGet Trusted Publishing (OIDC) replaces the long-lived API key

**Type:** CI / release infrastructure. No library code, public API, or package content is
affected — consumers see no change.

`release.yml` no longer authenticates to nuget.org with the long-lived `NUGET_API_KEY`
organisation secret. It now uses
[NuGet Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing):
GitHub Actions issues a short-lived, signed OIDC token, nuget.org validates it against a
policy bound to (repository owner, repository, workflow filename) and returns an API key
valid for one hour.

## Changes

- `permissions` gains `id-token: write`, required for GitHub to issue the OIDC token.
- A `NuGet/login` step, pinned to a full commit SHA, runs **immediately before** the push
  steps. The returned key is valid for one hour, so requesting it earlier — before a full
  build and test run — risks expiry mid-release.
- Both the package and symbol pushes take their key from
  `steps.nuget-login.outputs.NUGET_API_KEY`.
- A new early validation step fails the run in seconds if the `NUGET_USER` variable is
  missing, instead of after a full build.

## Operational notes

- Requires the org variable `NUGET_USER` (set to the nuget.org **profile name**, not the
  email address) and a trusted publishing policy on nuget.org for this repository and the
  `release.yml` filename. The policy matches on the workflow **filename**, so renaming
  `release.yml` breaks publishing.
- The `NUGET_API_KEY` organisation secret is **deliberately retained**. It is shared with
  other repositories that have not yet migrated; removing it is a one-way door and must
  wait until every publishing repo uses OIDC.
- Rollback is reverting this commit — the secret still exists and the previous workflow
  keeps working.

Refs: #301
