# Stop publishing test scaffolding to GitHub Packages (#279)

`build-dotnet.yml` pushed to GitHub Packages with a bare glob and no filter, on both the
push-to-`main` build and every pull-request build:

```yaml
dotnet nuget push ./**/*.nupkg --source github --skip-duplicate -k "$GH_PACKAGES_TOKEN"
dotnet nuget push ./**/*.snupkg --source github --skip-duplicate -k "$GH_PACKAGES_TOKEN" || true
```

That picked up `tests/TestAssemblies/Common.Tests.TestTypes` — a fixture assembly that
exists only to be loaded by `Ploch.Common.Tests` — so `Ploch.Common.Tests.TestTypes` was
published to the internal feed on every build. Lower stakes than #278, which was the same
class of mistake pointed at NuGet.org, but the two workflows disagreed on what is
publishable and that disagreement was the bug.

## Why the project packed at all

`Directory.Build.props` derives `IsTestProject` from a project-*name* heuristic:

```xml
<IsTestProject>$(MSBuildProjectName.EndsWith('Tests'))</IsTestProject>
```

`Ploch.Common.Tests.TestTypes` ends in `TestTypes`, not `Tests`, so it fell into the
library branch — which sets `GeneratePackageOnBuild=true` and forces `IsTestProject=false`.
A comment in that file claimed the SDK's own default would still switch packing off outside
`src/`. It does not. NuGet's default only fires when `IsTestProject` or
`IsTestingPlatformApplication` is true, and this project is neither, so the empty
`IsPackable` fell through to NuGet's general `'' -> true` default instead.

Measured before the fix:

```text
Ploch.Common.Tests.TestAssembly1 -> IsPackable=false GeneratePackageOnBuild=false
Ploch.Common.Tests.TestAssembly2 -> IsPackable=false GeneratePackageOnBuild=true
Ploch.Common.Tests.TestTypes     -> IsPackable=true  GeneratePackageOnBuild=true
```

The two `TestAssembly` fixtures escaped only because they hard-code `IsPackable=false` in
their own `.csproj`. `TestTypes` did not, and a `Release` build of it really did produce
`Ploch.Common.Tests.TestTypes.4.1.1-prerelease.nupkg` and a matching `.snupkg`.

## What changed

- **`Directory.Build.props` now excludes the whole `tests/` tree structurally.** A new
  `_PlochTestRoot`, mirroring the existing `_PlochSourceRoot`, drives a property group that
  forces `IsPackable=false` and `GeneratePackageOnBuild=false` for any project whose
  directory sits under `tests/`. This fixes the class rather than the instance: a future
  fixture cannot reintroduce the bug by being named the wrong thing.

  The group is placed **after** the non-test group on purpose — MSBuild takes the last
  assignment, so moving it earlier would let `GeneratePackageOnBuild=true` win again.
  `IsTestProject` is deliberately left alone; flipping it for the whole tree would pull
  test tooling such as coverage collectors into plain fixture assemblies.

  Neither root carries a trailing directory separator, because both are compared with
  `StartsWith` against `$(MSBuildProjectDirectory)`, which uses `\` on Windows and `/` on
  the Linux CI runners.

  This changes the *default*, not a lock. `Directory.Build.props` is imported before the
  `.csproj`, so a project under `tests/` that sets `IsPackable=true` itself still wins. That
  is deliberate — the failure mode is a fixture packing by accident, not one opting in on
  purpose — and the publish filters below would still keep such a package off the feeds.

- **`build-dotnet.yml` now filters the same way `release.yml` does**, in both publish steps
  and for symbol packages as well as packages:

  ```bash
  PKGS=$(find . -name '*.nupkg' -not -path './tests/*')
  ```

  The exclusion is structural rather than a match on the name `Test`: a `-not -path
  '*/Test*'` filter also swallows `./src/TestingSupport*`, because `find -path` matches the
  whole path and `*` spans `/`. That was #278. An empty package set is now a hard error, as
  it already is in `release.yml`, so a build that silently produces nothing fails loudly.

  This filter is the second line of defence. The `Directory.Build.props` guard is the fix;
  the workflow filter is what keeps the workflows honest about agreeing.

- **`scripts/publish-nugetorg.ps1`** — the manual NuGet.org escape hatch — previously pushed
  a bare `**/*.nupkg` glob. It now agrees with the workflows on both *what* is published and
  *what counts as failure*:

  - The `tests/` exclusion is anchored to the repository root and carries a trailing
    separator, so it matches `./tests/*` exactly. A project under `src/tests/` is still
    published, as the workflows would, and a sibling such as `tests-integration/` is not
    excluded by accident.
  - An empty package set now **exits non-zero**. `Write-Error` writes a *non-terminating*
    error, so the previous `Write-Error` + `return` reported a failed publish to the caller
    as exit code 0.
  - Each push is checked via `$LASTEXITCODE`. `dotnet` is a native command, so a non-zero
    exit raises no PowerShell error and is invisible to `try`/`catch` and to
    `$ErrorActionPreference`; without the check, one rejected package was followed by a
    successful-looking script exit — a partial publish reported as a complete one.
  - Symbol packages (`.snupkg`) are now pushed too, matching `release.yml`.

- **`release.yml`** — comment only. It stated that `Common.Tests.TestTypes` "is packable
  only because its name ends in TestTypes"; that is now past tense.

- **`docs/libraries/common-msbuild.md`** documents both directory guards and why the name
  heuristic is insufficient on its own in either direction.

## Verification

After the change, all three fixture projects report `IsPackable=false` and
`GeneratePackageOnBuild=false`, and a `Release` build of `TestTypes` produces no package
directory at all. `src/Common`, `src/TestingSupport.XUnit3` (the #280 regression guard) and
`src/Common.DawnGuard` still report `IsPackable=true` and still produce a `.nupkg`.

## Not covered

Copies of `Ploch.Common.Tests.TestTypes` already on the GitHub Packages feed are left in
place; `--skip-duplicate` means they are simply never added to again. Removing them is a
feed-administration task, not a repository change.
