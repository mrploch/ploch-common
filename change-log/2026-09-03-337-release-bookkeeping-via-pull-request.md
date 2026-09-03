# Release workflow: land post-release bookkeeping via a pull request (#337)

The release workflow's final step pushed the next-development-version bump and the archived
change-log entries straight to the default branch. Branch protection rejects that, so the
step failed on **every** release run — 3.0.0 on 2026-03-13 and 4.0.47 on 2026-09-03 — each
time *after* the packages had already been published to nuget.org.

```
remote: error: GH006: Protected branch update failed for refs/heads/main.
remote: - Changes must be made through a pull request.
remote: - 4 of 4 required status checks are expected.
 ! [remote rejected] HEAD -> main (protected branch hook declined)
```

The first line is the pull-request requirement. `enforce_admins: false` does **not** cover
it: that rule is governed separately by `bypass_pull_request_allowances`, which is unset on
this branch, so nobody may push directly — not even an organisation owner using a classic
PAT with `repo` scope. The token was never the problem.

## What changed

- **The bookkeeping now arrives as a pull request.** The step commits the bump and the
  archived change-log entries to `chore/post-release-<version>`, pushes that branch, and
  opens a PR against the default branch. Opening a PR needs no elevated permission and
  cannot be refused by branch protection, so **the release run now finishes green**.
- **Re-running the same release updates the same pull request** instead of accumulating
  them: the branch is named after the released version and its head ref is updated in
  place with a lease-guarded force push. It is deliberately *not* deleted and recreated
  — GitHub closes a pull request when its head ref is deleted, which would shut the very
  PR the re-run is meant to refresh. The branch holds only generated bookkeeping and is
  owned exclusively by the workflow, so rewriting it discards nobody's work.
- **Concurrent dispatches of the same release are serialised** by a `concurrency` group
  keyed on `release_version`, with `cancel-in-progress: false` so a run that has already
  pushed packages to nuget.org is never killed part-way through its bookkeeping.
- **`pull-requests: write`** added to the workflow permissions.
- **`docs/libraries/common-msbuild.md` is re-synced automatically.** It reproduces
  `version.json` verbatim in a fenced block and is required to stay byte-equal to it, but
  nothing updated it, so it drifted on every release. A new step rewrites just that block,
  leaving the surrounding prose alone, and is a no-op when the two already agree.
- The step is still idempotent: with nothing to commit it reports so and exits successfully
  rather than opening an empty pull request.

## Consequences for release operators

A release now produces **two** things to act on: the GitHub Release itself, and a
bookkeeping pull request that must be merged to bring the default branch in line with what
was published. The run's job summary links the PR.

Not merging it leaves the branch claiming the released version and the change-log entries
unarchived — which would duplicate every one of them into the next release's generated
notes.

## Notes

No library code changed; this is release tooling only. No API or behavioural change, and no
effect on published packages.
