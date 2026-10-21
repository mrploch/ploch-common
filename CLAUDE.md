# ploch-common — agent entry point

A family of small .NET utility libraries: extension methods and guard clauses, serialization and dependency-injection
abstractions with implementations, an action-handler framework, web and MAUI helpers, and xUnit v3 test-support
packages. There is no application, database or UI in this repository.

Public repository, Apache-2.0. 21 packages are on nuget.org (all at 4.0.47); prereleases go to GitHub Packages from
`main`.

This file holds the essentials. The verified detail lives in [`docs/ai/`](docs/ai/README.md) — read the document that
matches the task before changing anything.

## Index

| Document | Read it when you need |
|---|---|
| [`docs/ai/ARCHITECTURE.md`](docs/ai/ARCHITECTURE.md) | Components, key types, flows, invariants, sharp edges |
| [`docs/ai/TECH-STACK.md`](docs/ai/TECH-STACK.md) | Toolchain, verified commands, dependencies, CI/CD, release |
| [`docs/ai/CONVENTIONS.md`](docs/ai/CONVENTIONS.md) | Code, test, commit, branch, PR and issue conventions |
| [`docs/ai/BACKLOG.md`](docs/ai/BACKLOG.md) | Open work, themes, technical debt, roadmap |
| [`docs/ai/AGENT-CONFIG.md`](docs/ai/AGENT-CONFIG.md) | What agent configuration exists and what should change |
| [`docs/ai/README.md`](docs/ai/README.md) | The document standard and how to keep it current |

Human-facing documentation: [`README.md`](README.md), [`docs/INDEX.md`](docs/INDEX.md) (per-library pages in
`docs/libraries/`), and the DocFX site built from `DocumentationSite/`.

## Layout

```text
src/                 41 projects: Common (core), Common.* families, TestingSupport.*
tests/               21 test projects mirroring src/ (+ TestAssemblies/ fixtures)
docs/                human documentation; docs/ai/ is this knowledge base (excluded from the site)
DocumentationSite/   the DocFX site (docfx_project/ is a legacy leftover)
change-log/          one file per change; consumed by the release workflow
scripts/             PowerShell/Python helpers
Ploch.Common.slnx    the only solution CI builds, tests and packs (three more .slnx exist for endpoints, MAUI, local work)
```

## Build and test

- Needs a sibling checkout `../mrploch-development` (central package versions; restore fails without it) and the .NET 10
  SDK. `../ploch-data` is needed only for the web-endpoint projects. No token and no private feed is needed.
- On a machine without a .NET 8 runtime set `DOTNET_ROLL_FORWARD=Major` (three test projects have net8.0 legs).

```bash
dotnet restore Ploch.Common.slnx
dotnet build Ploch.Common.slnx -c Release --no-restore          # 0 errors, ~7 s
DOTNET_ROLL_FORWARD=Major dotnet test Ploch.Common.slnx -c Release --no-build   # 2,266 passed, 38 skipped
dotnet test Ploch.Common.slnx --filter "FullyQualifiedName~Ploch.Common.Tests.Collections"
dotnet docfx DocumentationSite/docfx.json --serve               # after `dotnet tool restore`
```

## Rules that break the build or the release

- Versions are central: never write a `Version` in a `.csproj`; pins live in
  `../mrploch-development/dependencies/*.props`.
- Nullable diagnostics (`CS86xx`) are errors via `.editorconfig`; other warnings are not errors but add none. Do not add
  `_ =` discards to silence IDE0058 (it is disabled in `.editorconfig`).
- Public API needs XML documentation (British English). Guards live in two files: change both `Guard.cs`
  (`netstandard2.0`) and `GuardNet7.cs` (`net7+`) unless the difference is deliberate.
- A library not listed in `Ploch.Common.slnx` is never built, tested or packed by CI. Anything under `src/` is packable
  by default; anything under `tests/` is not.
- Every change adds a `change-log/YYYY-MM-DD-<issue>-<slug>.md` file; the release workflow builds the notes from them.
- Do not rename `release.yml`: the nuget.org Trusted Publishing policy matches the workflow file name.
- Never commit credentials or conversation databases; stage named files, not whole directories.

## Behaviour that surprises people

- `Ploch.Common` targets `netstandard2.0;net8.0`, but 19 of the 41 `src` projects target net10.0; "most libraries are
  `netstandard2.0`" is false.
- Ten projects are in no solution; two of them do not compile and nothing notices. 13 packable projects are not
  published.
- `ActionHandlerManager` is a first-success-wins fallback chain, not a pipeline and not a dependency resolver.
- The System.Text.Json serializer's per-call configuration mutates shared options and throws once they are in use.
- `Ploch.Common.Net9` ships net10.0 and its namespace is `Ploch.Common.AssemblyLoading`.
- Several pages in `docs/libraries/` describe packages that do not exist on nuget.org.
- The merge gate is four external checks (Codacy, CodeFactor, SonarCloud, `Test Results`), not the Actions Build job;
  the Publish Docs workflow currently fails on `main`.
- Tag `v4.0.47` is not an ancestor of `main`; `git describe` on `main` reports `rel-2.0.1`.

## Workflow

- **Identifiers.** The repository's history uses GitHub issue and pull-request numbers (`Refs: #290`, `Closes #329`,
  branch `feature/347-assignableto-guards`). A newer maintainer convention (`plo-<n>` branches, `PLO-<n>` footers)
  appears nowhere in this repository's history; where the two disagree follow the history unless the owner says
  otherwise.
- **Commits** are Conventional Commits (`type(scope): Subject`, imperative, `BREAKING CHANGE:` footer and `!` when
  breaking) with a `Refs:` footer. Never amend; create new commits.
- **Pull requests** follow `.github/pull_request_template.md`, are squash-merged, and are done when all checks are green
  on the latest commit and every review thread is answered or resolved. Update the pull-request body after later pushes.
- **Do not push to `main`**; open a pull request. Code-owner review is required by branch protection (there is no
  `CODEOWNERS`).
- Keep `docs/ai/` current in the same pull request as the change it describes (table in `docs/ai/README.md`).
