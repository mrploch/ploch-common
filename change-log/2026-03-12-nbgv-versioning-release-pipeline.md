# NBGV Versioning and Automated Release Pipeline

## Type: build, ci

## Summary

Switched project versioning from manual `VersionPrefix`/`RELEASEVERSION` to Nerdbank.GitVersioning (NBGV) and added a fully automated release pipeline for publishing packages to NuGet.org.

## Changes

### Versioning

- Added `version.json` with NBGV configuration, starting at version `2.1-prerelease`
- Removed manual versioning properties (`VersionPrefix`, `BuildNumber`, `VersionSuffix`, `RELEASEVERSION` conditional) from `Directory.Build.props`
- Added `Nerdbank.GitVersioning` NuGet package (v3.7.115) to all projects via `Directory.Build.props`
- Registered `nbgv` as a local dotnet tool (`.config/dotnet-tools.json`)

### Release Pipeline

- Created `.github/workflows/release.yml` — a `workflow_dispatch` workflow that:
  - Accepts a version number (e.g., `3.0`)
  - Updates `version.json`, builds in Release mode, runs tests
  - Publishes `.nupkg` and `.snupkg` to NuGet.org
  - Creates a git tag and GitHub Release with auto-generated release notes
  - Bumps version to next development cycle (e.g., `3.1-prerelease`)

### Open-Source Enhancements

- Enabled **SourceLink** (`Microsoft.SourceLink.GitHub` v8.0.0) for source debugging
- Enabled **symbol packages** (`.snupkg`) for the NuGet symbol server
- Enabled **deterministic builds** in CI (`ContinuousIntegrationBuild`)
- Added NuGet.org version and download badges to `README.md`

### CI Workflow Updates

- Added `fetch-depth: 0` to checkout step (required by NBGV for git height computation)
- Updated NuGet publish step to also push symbol packages

## Breaking Changes

- The `RELEASEVERSION` environment variable is no longer recognised — version is now driven by `version.json`
