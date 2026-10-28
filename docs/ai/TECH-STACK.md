# Technology stack — ploch-common

| Field | Value |
|---|---|
| Last verified | 2026-10-10, against commit `1f1764b166dabfad4a01b166b94a1115b88e7c6d` |
| Verified by | Running restore, build, test, the DocFX build and the package-status queries in a throw-away copy of that commit (macOS, .NET SDK 10.0.401, only the .NET 10.0.12 runtime installed, warm package cache, nuget.org as the only source); reading every build file and workflow; querying branch protection, workflow runs, releases and nuget.org through their APIs on the same day |
| Entry point | [`CLAUDE.md`](../../CLAUDE.md) |

## 1. Summary

| Item | Value | Defined in |
|---|---|---|
| Language | C#, `LangVersion=default`, nullable reference types on | `Directory.Build.props:125-126` |
| Target frameworks | Mixed, per project: `netstandard2.0`, `netstandard2.0;net8.0` (the core), `net8.0`, `netstandard2.0;net6.0`, and net10.0 through `$(TargetFrameworkVersion)` (12 of the 33 `src` projects use it alone, 13 in total). Counts are in section 4.3 | each `.csproj`; `Directory.Build.props:42` |
| SDK | Not pinned: no `global.json`. CI asks `setup-dotnet` for `9.0.x` although the projects target net10.0 | `.github/workflows/*.yml` |
| Package management | Central Package Management. Most versions come from `../mrploch-development/dependencies/*.props`; the exceptions are the `Nerdbank.GitVersioning` pin in `Directory.Packages.props` and project-scoped `PackageVersion Update` overrides (section 4) | `Directory.Packages.props:3-9` |
| Versioning | Nerdbank.GitVersioning; base `4.1-prerelease` | `version.json:3`, `.config/dotnet-tools.json` |
| Test stack | xUnit v3 3.2.2 (xUnit v2 2.9.3 still in a few projects), FluentAssertions 8.10.0, AutoFixture 4.18.1, Moq 4.20.72 (transitive) | `mrploch-development/dependencies/Testing.Packages.props` |
| Analysers | StyleCop, Roslynator, SonarAnalyzer, .NET analysers and others, all as global package references from the sibling repository; four more only in `Ploch.Common.csproj` | `mrploch-development/dependencies/Analyzers.Global.Packages.props`; `src/Common/Ploch.Common.csproj` |
| Warnings policy | `TreatWarningsAsErrors=false`; `WarningsAsErrors NU1605;Nullable` in three projects; about 50 `CS86xx` nullable rules are `error` in `.editorconfig` | `Directory.Build.props:17`; `.editorconfig:221-267` |
| Documentation tooling | DocFX 2.78.5 (local tool), site in `DocumentationSite/`; `docfx_project/` is a legacy leftover | `.config/dotnet-tools.json`; `DocumentationSite/docfx.json` |
| Licence | Apache-2.0 (`LICENSE` text and `PackageLicenseExpression`) | `Directory.Build.props:12` |

## 2. Build prerequisites

- **A sibling checkout named `mrploch-development`** next to the repository (`../mrploch-development`). Restore fails at
  project-import time without it. CI clones its default branch, unpinned (`build-dotnet.yml`, `release.yml`,
  `publish-docs.yml`, `qodana_code_quality.yml`). `scripts/prepare-repo.ps1` clones or updates it.
- **No `ploch-data` checkout.** The only projects that referenced it, the CRUD web-endpoint libraries, were moved to [ploch-endpoints](https://github.com/mrploch/ploch-endpoints) by #353.
- **.NET 10 SDK.** `global.json` does not exist, so the newest installed SDK is used.
- **A .NET 8 runtime**, to run the net8.0 legs of three test projects (`Common.Tests`,
  `Common.Serialization.SystemTextJson.ExtensionsDependencyInjection.Tests`, `TestingSupport.XUnit3.Dependencies`). On a
  machine with only .NET 10 set `DOTNET_ROLL_FORWARD=Major` (an environment variable, no file edit).
- **No credentials and no private feed** for restore, build or test. There is no `nuget.config` in the repository;
  nuget.org is the only source. GitHub Packages is added only inside CI, using the secret `GH_PACKAGES_TOKEN`.
- `dotnet tool restore` for `nbgv` and `docfx`. Java 17 is used by the SonarCloud scanner in CI only.
- Full git history (`fetch-depth: 0`): version numbers derive from commit height.

## 3. Commands

Observed on 2026-10-10 in a clean copy, environment as in the stamp. Nothing needed a token.

| Command | Result |
|---|---|
| `dotnet tool restore` | Works. `nbgv` 3.7.115, `docfx` 2.78.5 |
| `dotnet restore Ploch.Common.slnx` | Works, about 2 s with a warm cache |
| `dotnet build Ploch.Common.slnx -c Release --no-restore` | Works, 0 errors and 0 warnings, about 7 s, 21 `.nupkg` files produced (`GeneratePackageOnBuild`). **No analyser (CA, SA, S, RCS) warning**. Before #353 removed the `Microsoft.SourceLink.GitHub` pin, every restore also printed NuGet audit warnings (`NU1902`) |
| `dotnet test Ploch.Common.slnx -c Release --no-build` | **Exit 1 on a machine without a .NET 8 runtime**: the three net8.0 test hosts cannot start. All net10.0 assemblies pass |
| `DOTNET_ROLL_FORWARD=Major dotnet test Ploch.Common.slnx -c Release --no-build` | Works, exit 0: **2,220 passed, 0 failed, 38 skipped** in about 4 s. The skips are Windows- and path-specific tests |
| `dotnet docfx metadata DocumentationSite/docfx.json` | Exit 0, about 51 s, 35 warning lines (see section 6: this is what fails the CI guard) |
| `dotnet docfx build DocumentationSite/docfx.json` | Exit 0, about 55 s, 2 warnings, 0 errors, about 12,850 HTML files |
| `dotnet list Ploch.Common.slnx package --outdated` | Works, 28 packages (section 4.3). `--deprecated` works too: `Dawn.Guard` only |
| Build of a project outside the solution, for example `dotnet build src/Common.Maui/Common.Maui/Ploch.Common.Maui.csproj -c Release` | Works for `Common.Maui`, `Common.Maui.Fonts`, `Common.ObjectBuilder`, `Common.Dependencies`, `Common.DependencyInjection.Hosting` and `Common.Extensions.Configuration`. **Does not compile:** `src/Common.NSwag` (`string.ToPascalCase` missing) and `src/TestingSupport.FluentAssertions.IOAbstractions` (FluentAssertions 8 API break). CI never notices because neither is in the solution |

```bash
dotnet tool restore
dotnet restore Ploch.Common.slnx
dotnet build Ploch.Common.slnx -c Release --no-restore
DOTNET_ROLL_FORWARD=Major dotnet test Ploch.Common.slnx -c Release --no-build   # only if .NET 8 is not installed
dotnet test Ploch.Common.slnx --filter "FullyQualifiedName~Ploch.Common.Tests.Collections"
dotnet docfx DocumentationSite/docfx.json --serve                                   # local docs preview
```

Things that do not work as you might expect:

- xUnit v3 test projects are executables (`OutputType=Exe`) and run on the Microsoft Testing Platform, which is why the
  three xUnit v3 support libraries are also `Exe`.
- The `.slnx` files mix `\` and `/` separators in project paths; both build.
- The `docs/API_REFERENCE.md` page is generated by `scripts/generate-api-reference.py` from the built XML documentation;
  do not edit it by hand.

## 4. Dependencies

Every `PackageReference` in the repository resolves to a central version; no project writes a `Version` or
`VersionOverride`. The exceptions to central pins are `Nerdbank.GitVersioning`, pinned in
`Directory.Packages.props:11`, and a handful of `PackageVersion Update` overrides in project files (section
9).

### 4.1 Runtime

| Package | Version | Where the version is set | Used by |
|---|---|---|---|
| `Microsoft.Extensions.*`, `System.Text.Json`, `System.Management`, `Microsoft.Extensions.FileSystemGlobbing`, `Microsoft.AspNetCore.OpenApi`, EF Core abstractions | 10.0.5 | `mrploch-development/dependencies/MicrosoftExtensions.Net9.Packages.props` (the file name says Net9; it also sets `TargetFrameworkVersion=net10.0`) | Core, Serialization, DI, Maui |
| `Newtonsoft.Json` | 13.0.4 | `Common.Packages.props:26` | Newtonsoft serializer, XUnit3 |
| `Dawn.Guard` | 1.12.0, **deprecated** | `Common.Packages.props:16` | `Common.DawnGuard`, and, unused, `Common.DependencyInjection`, `Common.Serialization`, `Common.WebUI` |
| `NSwag.CodeGeneration.CSharp` | 14.6.3 | `Common.Packages.props:27` | `Common.NSwag` |
| `CommunityToolkit.Mvvm` 8.4.0, `Microsoft.Maui.Controls.Xaml` 9.0.90 | as stated | `Common.Packages.props` | `Common.Maui` |
| `JetBrains.Annotations` | 2025.2.4 | `Common.Packages.props:22` | `Common.Dependencies`, test projects |

Serilog is not used by any project in this repository, although a Serilog props file exists in the sibling repository.

### 4.2 Build, test and analysis

| Package | Version | Where the version is set |
|---|---|---|
| `xunit.v3` (and `.core`, `.extensibility.core`, `.runner.msbuild`) | 3.2.2 | `Testing.Packages.props:42-48` |
| `xunit` (v2) | 2.9.3, still referenced by `TestingSupport.FluentAssertions.IOAbstractions.Tests`, `Common.Maui.Tests`, `Common.WebUI.Tests` | `Testing.Packages.props:30` |
| `xunit.runner.visualstudio` | 3.1.5 | `Testing.Packages.props:37` |
| `FluentAssertions` | 8.10.0 | `Testing.Packages.props:14` |
| `AutoFixture` / `.AutoMoq` 4.18.1, `AutoFixture.Xunit3` 4.19.0, `Moq` 4.20.72 | as stated | `Testing.Packages.props:3-19` |
| `Microsoft.NET.Test.Sdk` | 18.6.0 | `Testing.Packages.props:18` |
| `coverlet.msbuild` 8.0.0 / `coverlet.collector` 10.0.1 | as stated | `Common.Packages.props:14` / `Testing.Packages.props:10` |
| `Nerdbank.GitVersioning` | 3.7.115 (package and tool) | `Directory.Packages.props:11`; `.config/dotnet-tools.json:6` |
| SourceLink | included in the .NET SDK; no package | `Directory.Build.props` sets `PublishRepositoryUrl` and `EmbedUntrackedSources` |
| `docfx` (tool) | 2.78.5 | `.config/dotnet-tools.json` |
| Global analysers | codecracker.CSharp 1.1.0, Microsoft.CodeAnalysis.NetAnalyzers 10.0.201, Roslynator.Analyzers 4.15.0, SonarAnalyzer.CSharp 10.21.0.135717, StyleCop.Analyzers 1.1.118, Microsoft.VisualStudio.Threading.Analyzers 17.14.15 | `Analyzers.Global.Packages.props` (as `GlobalPackageReference`) |
| Analysers only in `Ploch.Common.csproj` | ErrorProne.NET.CoreAnalyzers 0.1.2, Philips.CodeAnalysis.DuplicateCodeAnalyzer 1.6.4, PrimaryConstructorAnalyzer 1.0.6, ReflectionAnalyzers 0.3.1 | `Common.Packages.props:18,28-30` |

### 4.3 Notes

- **Target frameworks of the 33 `src` projects**, from the project files: 12 use `$(TargetFrameworkVersion)` alone (net10.0),
  11 are `netstandard2.0` only, 6 are `net8.0` only, and one each are `netstandard2.0;net8.0` (the core),
  `netstandard2.0;net6.0`, `net10.0;net8.0`, `net10.0` and `$(TargetFrameworkVersion);net8.0`. Of the 20 test projects,
  15 use `$(TargetFrameworkVersion)`.
- **Newer versions exist for 28 packages** (nuget.org, 2026-10-10). Notable: `xunit.v3` 3.2.2 → 4.0.2,
  `xunit.runner.visualstudio` 3.1.5 → 4.0.1, `xunit.analyzers` 1.27.0 → 2.2.0, `Roslynator.Analyzers` 4.15.0 → 5.0.1,
  `Philips.CodeAnalysis.DuplicateCodeAnalyzer` 1.6.4 → 2.0.0, `coverlet.msbuild` 8.0.0 → 10.1.0, `Nerdbank.GitVersioning` 3.7.115 → 3.10.94, `SonarAnalyzer.CSharp` 10.21 → 10.35,
  `Microsoft.Extensions.*` 10.0.5 → 10.0.12, `FluentAssertions` 8.10.0 → 8.11.0, `Moq` 4.20.72 → 4.21.0.
- **Deprecated:** `Dawn.Guard` 1.12.0 (marked legacy). The repository's own `Ploch.Common.DawnGuard` package is
  deprecated in its metadata and carries `[Obsolete]`.
- **Where updates are made:** nearly all pins live in the sibling repository, so a bump there affects every consumer.
  Dependabot here (`.github/dependabot.yml`: NuGet, weekly, `/`) only sees what is in this repository; whether it can
  follow the imports to the sibling is `UNVERIFIED`.

## 5. Static analysis and code style

| Layer | What | Where configured |
|---|---|---|
| Compiler and .NET analysers | `EnableNETAnalyzers`, `AnalysisLevel=latest-Recommended` (`latest-all` in `Ploch.Common`), `EnforceCodeStyleInBuild` | `Directory.Build.props:14-16`; `src/Common/Ploch.Common.csproj` |
| Style and severities | `.editorconfig` (`root = true`); IDE0058 disabled with a rationale (`:286-294`); `ca1707` off for underscored test names; `tests/.editorconfig` silences documentation warnings for tests | `.editorconfig`, `tests/.editorconfig` |
| StyleCop | `stylecop.json`, `stylecop.ruleset` (the ruleset is wired only into `Ploch.Common.csproj:28`) | repository root |
| SonarCloud | Project key `mrploch_ploch-common`, organisation from a repository variable; no `sonar-project.properties` is committed | `build-dotnet.yml:12,131-147` |
| Codacy | Coverage upload in CI. The only config file is `.codacy.yml.test`, **misnamed**, so no repo-level Codacy configuration is in effect | `.codacy.yml.test` |
| CodeFactor / DeepSource | `.codefactor.yml` excludes `scripts/**` and one vendored script; `.deepsource.toml` sets test patterns | repository root |
| Qodana | `jetbrains/qodana-cdnet:2026.1`, solution `Ploch.Common.slnx`, baseline `qodana.sarif.json` (a committed output file) | `qodana.yaml`, `qodana_code_quality.yml` |
| Markdown | No Markdown linter configuration is committed | — |

In-source suppressions follow a pattern: `GlobalSuppressions.cs` for assembly-wide choices, `[SuppressMessage]` with a
justification on members, `#pragma warning disable ... // reason`, and `// skipcq: <rule> - <reason>` for
Codacy/DeepSource. There are bare disables without a reason in about ten places in `src/Common`.

## 6. CI/CD

| Workflow (file) | Trigger | What it does |
|---|---|---|
| Build (`build-dotnet.yml`) | push to `main`; pull requests to `main`; manual | Checkout (full history), clone the sibling repository, restore, build and test **only `Ploch.Common.slnx`** with Coverlet; SonarCloud analysis; Codacy coverage; a test-report check run named `Test Results`; a DocFX smoke build (non-blocking). On `main` it pushes prerelease packages to GitHub Packages; on pull requests it pushes PR-versioned packages |
| Release (`release.yml`) | manual only, from `main`; inputs `release_version`, optional `next_version` | See section 7 |
| Publish Docs (`publish-docs.yml`) | push to `main`; every pull request; manual | `build-docs` runs `docfx metadata` through a wrapper that **fails on any `warning:` line except one known message**, then `docfx build`; `deploy-docs` publishes to GitHub Pages (not for pull requests); a per-PR preview is published only if the repository variable `DOCS_PREVIEW_BASE_URL` is set (it is not) |
| Docs Preview Cleanup (`docs-preview-cleanup.yml`) | pull request closed | Deletes the PR's preview folder |
| Qodana (`qodana_code_quality.yml`) | push to `main` and `releases/*`; pull requests; manual | Qodana analysis against the committed baseline |
| Diagnose GH_TOKEN (`diagnose-gh-token.yml`) | manual | Prints the token's identity and expiry and a short fingerprint, never the value |

Facts worth knowing:

- **The real merge gate is four external checks, not the Actions "Build" job.** Branch protection on `main` requires
  `Codacy Static Code Analysis`, `CodeFactor`, `SonarCloud Code Analysis` and `Test Results` (branch must be up to
  date), one approving review, **code-owner review** and resolved conversations; administrators are not subject to it
  (`enforce_admins: false`). The only repository ruleset is disabled. **No `CODEOWNERS` file exists**, so the code-owner
  requirement has nothing to match. The Build job, Qodana, Publish Docs and CodeQL are not required checks.
- **Publish Docs was failing on `main` at the verified commit**, so the Pages deploy was not happening. The metadata
  step failed because a NuGet audit warning (`NU1902`) from the `Microsoft.SourceLink.GitHub` 8.0.0 pin was printed as
  a `warning:` line, and the guard in `publish-docs.yml:125-130` whitelists only one message. #353 removed the package,
  because the .NET SDK includes SourceLink. Any new restore warning, including a NuGet audit warning, trips the same
  guard.
- **Secrets, by name:** `SONAR_TOKEN`, `CODACY_PROJECT_TOKEN`, `GH_TOKEN` (a personal access token used for tags, pushes
  and the release pull request), `QODANA_TOKEN`, `GH_PACKAGES_TOKEN` (not a repository-level secret; presumably
  organisation level, `UNVERIFIED`). **Variables:** `NUGET_USER` and `SONAR_ORGANIZATION` (also not repository-level).
  NuGet publishing uses Trusted Publishing, with no API-key secret.
- **Cross-repository input:** every workflow clones `mrploch-development` at its default branch. A change there changes
  this repository's build without any commit here.
- **Bots on pull requests:** five to twelve per pull request (CodeRabbit, Sourcery, Codacy, CodeAnt, Codex, Bito, Qodo,
  GitHub Code Quality, DeepSource, SonarCloud, Copilot reviewer and others).
- **Health, last 200 runs:** Build 43 runs, 1 failure; Qodana 43/0; CodeQL Code Quality 52/0; Publish Docs 45 runs, 6
  failures (the latest on `main` is a failure); Release 2 runs in the window, both failures (section 7).
- A stale "Docs Preview" workflow registration still exists on GitHub for a file that is gone.
- Two **gitlinks** are committed under `.claude/worktrees/` with no `.gitmodules`; checkout post-steps log
  `fatal: No url found for submodule path` for them.

## 7. Versioning and release

- **Version computation.** `version.json` declares `4.1-prerelease` with `versionHeightOffset: -1`. A development build
  is `4.1.<height>-prerelease`. In pull-request builds CI rewrites the suffix to `-pr.<N>.<branch>` on an unpushed
  commit. A release drops the suffix, and the patch number is the commit height, which is why the current release is
  `4.0.47`. Builds from `main` and from tags `v<major>.<minor>.<patch>` count as public releases
  (`publicReleaseRefSpec`).
- **Release procedure** (`release.yml`, manual run on `main`):
  1. Run the workflow with `release_version` (for example `4.1`) and optionally `next_version`.
  2. It validates the inputs, the `GH_TOKEN` and that `NUGET_USER` is not an email address.
  3. Checks out, clones the sibling repository, sets the version with `nbgv set-version` and commits it.
  4. Restores, builds and tests `Ploch.Common.slnx`.
  5. Creates and pushes the annotated tag `v<version>` **before** publishing (deliberately, so a retry is safe).
  6. Exchanges a GitHub OIDC token for a short-lived nuget.org key (`NuGet/login`; a Trusted Publishing policy must
     exist for this repository, the workflow file name `release.yml` and the package glob) and pushes every `.nupkg` and
     `.snupkg` outside `tests/` with `--skip-duplicate`.
  7. Builds release notes from `change-log/*.md`, creates the GitHub Release, bumps `version.json` to the next
     development version, archives the consumed change-log files, re-syncs the `version.json` excerpt in
     `docs/libraries/common-msbuild.md` and opens a **pull request** with that bookkeeping, because branch protection
     forbids pushing to `main`.
- **Manual escape hatch:** `scripts/publish-nugetorg.ps1` pushes the same set with an operator-supplied key.
- **What has actually shipped.** Tags: `rel-2.0.1`, `v3.0.0`, `v4.0.47`. GitHub Releases: the same three, none a draft
  or prerelease; `v4.0.47` (2026-09-03) is the latest. **nuget.org holds 21 package ids, all at 4.0.47** and no
  prerelease versions (prereleases go to GitHub Packages only). `v4.0.47` is not an ancestor of `main` (checked with
  `git merge-base --is-ancestor`): by the workflow's design the tag is placed on the release commit that sets the
  version, and `main` receives only the bookkeeping pull request afterwards, so `git describe` on `main` still reports
  `rel-2.0.1`.
- **The Release workflow's two most recent runs concluded `failure`**, as did the six earlier runs on the old default
  branch, although the releases exist. The workflow comments attribute this to the final bookkeeping push being
  rejected; the logs have expired, so the cause is `UNVERIFIED`.
- **Published versus packable.** 8 packable projects have no package on nuget.org:
  `Common.Dependencies`, `Common.DependencyInjection.Hosting`, `Common.Extensions.Configuration`, `Common.Maui`,
  `Common.Maui.Fonts`, `Common.NSwag`, `Common.ObjectBuilder` and
  `TestingSupport.FluentAssertions.IOAbstractions`. All sit outside `Ploch.Common.slnx`, which is why no release packs
  them. Two MAUI fixture projects under `src/` are also packable by the default rule.

## 8. External services

| Service | Used for |
|---|---|
| nuget.org | Stable packages, through Trusted Publishing |
| GitHub Packages | Prerelease and PR-versioned packages from CI |
| GitHub Pages | The documentation site (hostnames `common.github.ploch.dev` and `github.ploch.dev/ploch-common` both appear; which is live is `UNVERIFIED`) |
| SonarCloud | Static analysis, coverage, required status check |
| Codacy | Static analysis (required status check) and coverage reporting |
| CodeFactor | Static analysis (required status check) |
| Qodana Cloud | Static analysis against a baseline |
| DeepSource | Static analysis (configuration only) |
| Dependabot | NuGet updates, weekly |
| Review bots | CodeRabbit, Sourcery, CodeAnt, Bito, Qodo, Codex and others, on every pull request |

## 9. Risks and debt

Tooling and delivery only; code debt is in [`ARCHITECTURE.md`](ARCHITECTURE.md) section 7.

| Item | Detail |
|---|---|
| 14 of 53 projects unbuilt by CI | They sit outside `Ploch.Common.slnx`; two do not compile and nothing notices |
| Unpinned inputs | The sibling repository is cloned at its default branch; no `global.json`; CI requests SDK `9.0.x` while projects target net10.0 (how the runner supplies net10.0 is `UNVERIFIED`) |
| Branch protection mismatches | Code-owner review is required but no `CODEOWNERS` exists; the Actions Build job is not a required check; two commits on `main` are direct pushes without a pull request number (how, given the protection, is `UNVERIFIED`) |
| Release workflow | Every recorded run ends in `failure` even when publishing worked |
| Floating action versions | Mostly tags, not SHAs; Build has no `permissions` block |
| Stale per-project overrides | `PackageVersion Update` blocks for `Roslynator.Analyzers` 4.12.11 and `SonarAnalyzer.CSharp` 10.6 in several project files (`Common.NSwag` and the Maui projects) disagree with the global pins; their effect against `GlobalPackageReference` is `UNVERIFIED` |
| Stale files | `dotnet-releaser.toml` points at a `Ploch.Common.sln` that does not exist; `templates/Directory.Build.props` and `deps/` describe the pre-NBGV scheme; `scripts/Clean-DocFx.ps1` targets the legacy `docfx_project/`; every solution lists a root-level `Clean-DocFx.ps1` and `.codacy.yml` that do not exist |
| Documentation site content | `DocumentationSite/api-tmp/` and `docfx.tmp.json` are generated leftovers that are committed |
| Reproducing CI locally | Needs the sibling repository and, on a machine without .NET 8, `DOTNET_ROLL_FORWARD=Major` |
