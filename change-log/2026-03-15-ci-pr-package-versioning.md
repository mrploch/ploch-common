## CI: PR package versioning and publishing

- **PR builds** now publish NuGet packages to GitHub Packages with a branch-identifying prerelease tag (e.g. `3.1.1-pr.42.fix-serialization`), making it easy to test packages from in-progress work.
- **Master push builds** continue to publish prerelease packages (`3.1.1-prerelease`) to GitHub Packages only.
- **Release builds** continue to publish stable packages to NuGet.org via the manual release workflow.
- Build and test steps now consistently use Release configuration.
- Release workflow: GitHub Release is now created before the version bump commit, and the release commit uses deterministic timestamps for idempotent reruns.
