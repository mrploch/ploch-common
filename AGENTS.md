# AGENTS.md

## Cursor Cloud specific instructions

### Prerequisites

This is a .NET library suite (no runtime services). The environment requires:

- **.NET 9.0 SDK** and **.NET 10.0 SDK (preview)** — some projects target `net10.0`.
- **.NET 6.0 and 8.0 runtimes** — needed to run legacy test projects targeting those frameworks.
- **`mrploch-development` sibling repository** — must be available at `../mrploch-development` relative to the workspace root (i.e., `/mrploch-development` when workspace is at `/workspace`). This repo provides centralized NuGet package version definitions imported by `Directory.Packages.props`.

### Building and testing

Standard commands per `CLAUDE.md`:

- **Build:** `dotnet build Ploch.Common.slnx`
- **Test:** `dotnet test Ploch.Common.slnx`
- **Lint:** Analyzers run during build (`EnforceCodeStyleInBuild=true`). Use `dotnet format Ploch.Common.slnx --verify-no-changes` for formatting checks.

### Known issues

- `Common.Net6.Tests` and `TestingSupport.XUnit2.Dependencies` abort at test time due to a `Microsoft.Extensions.FileSystemGlobbing` compatibility issue with `net6.0`. These are pre-existing and do not affect the other ~1,250 tests.
- `dotnet format --verify-no-changes` reports pre-existing formatting differences (exit code 2). This is not a blocker.
- The build produces ~980 analyzer warnings (mostly CA1852, IDE0052, StyleCop) — all pre-existing, zero errors.
