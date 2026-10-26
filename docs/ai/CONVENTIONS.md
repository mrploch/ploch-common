# Conventions — ploch-common

| Field | Value |
|---|---|
| Last verified | 2026-10-10, against commit `1f1764b166dabfad4a01b166b94a1115b88e7c6d` |
| Verified by | The last 40 commits on `main` and a search of the whole history; ten merged pull requests read in full (#350, #348, #345, #340, #336, #335, #330, #328, #326, #323); the sources and tests of every family; the committed rules and the workflows |
| Entry point | [`CLAUDE.md`](../../CLAUDE.md) |

This document records **practice**. Where practice departs from a written rule it says so, and section 10 lists the
differences.

## 1. Order of authority

1. What the tooling enforces: the compiler and analysers (`.editorconfig`, `Directory.Build.props`), the four required
   status checks and branch protection on `main` ([`TECH-STACK.md`](TECH-STACK.md) section 6).
2. The pull-request template (`.github/pull_request_template.md`) and the short `Contributing` paragraph in `README.md`.
   There is no `CONTRIBUTING.md`.
3. Agent rules (`.claude/rules/`) and `CLAUDE.md`. Only two rules are committed (`change-log.md`,
   `todo-tasks-execution.md`); a developer checkout may also hold copies of a wider workspace rule set, which is not
   part of this repository.
4. This document. It records observed practice; where practice and a rule disagree it flags the conflict (section 11)
   but does not override the rule.

## 2. Repository layout and naming

| Topic | Practice | Example |
|---|---|---|
| Layout | Sources in `src/`, tests in `tests/`, docs in `docs/` (library pages in `docs/libraries/`), site in `DocumentationSite/`, per-change notes in `change-log/` | — |
| Project name | `Ploch.<Area>[.<Feature>]`; the file name is the PackageId, the AssemblyName and (by default) the RootNamespace | `src/Common.Serialization/Ploch.Common.Serialization.csproj` |
| Directory name | The project name without `Ploch.` | `src/Common.DependencyInjection/` |
| Test project | Source name plus `.Tests`, directory mirrors the source directory | `tests/Common.DependencyInjection.Tests/` |
| Family split | A base implementation project and a separate `.ExtensionsDependencyInjection` project | `Common.Serialization.SystemTextJson` + `...ExtensionsDependencyInjection` |
| Namespace | The project name (`Ploch.Common.Collections` and so on); file-scoped | — |
| Solutions | `.slnx`, at the root; a new library must be added to `Ploch.Common.slnx` to be built by CI | — |

Exceptions to these are listed in section 10 (for example four test projects and two fixture assemblies live under
`src/`).

## 3. C# code

| Topic | Practice | Example |
|---|---|---|
| Parameter checks | `Ploch.Common.ArgumentChecking` extension methods that return the value; `NotX` for argument errors, `RequiredX` for state errors. The `netstandard2.0` build needs the name, the `net8.0` build captures it | `type.NotNull(nameof(type))` in `Reflection/TypeLoader.cs` |
| Constructors | Primary constructors on classes; collection expressions (`= []`); `params IEnumerable<T>` | `SystemTextJsonSerializer(JsonSerializerOptions options)` |
| Multi-targeting | `#if NETSTANDARD2_0` / `NET7_0_OR_GREATER` for guards; `NET6_0_OR_GREATER` / `NET5_0_OR_GREATER` for small API differences | `src/Common/ArgumentChecking/` |
| Errors | Throw exceptions; custom exceptions for domain failures; `Ploch.Common.Results.ErrorInfo` is a plain data holder. Libraries take no logging dependency | `TypeConversionException` |
| Unused-expression diagnostics | IDE0058 is disabled in `.editorconfig`; never add `_ =` discards to silence it on fluent calls | `.editorconfig:286-294` |
| Suppressions | A justification is expected: assembly-level in `GlobalSuppressions.cs`, `[SuppressMessage(..., Justification = ...)]` on members, `#pragma warning disable ... // reason`, `// skipcq: <rule> - <reason>` for Codacy and DeepSource | `src/Common/GlobalSuppressions.cs` |
| Nullability | Enabled everywhere; many `CS86xx` rules are `error` | `.editorconfig:221-267` |
| Naming | PascalCase members, `_camelCase` private fields, `I` prefix on interfaces, `Async` suffix on awaitable methods | — |

Documentation comments: `///` with `<summary>`, `<param>`, `<returns>`, `<exception>` and `<example>`, continuation
lines indented by four spaces after the slashes, British English. Public API is expected to be documented; types in
`Common.Extensions.Configuration`, `Common.ObjectBuilder`, `Common.DependencyInjection.Hosting` and most of the CRUD
endpoint types are not (the build does not fail because warnings are not errors). Formatting is 4 spaces;
`Reflection/TypeExtensions.cs` uses 2.

## 4. Tests

| Topic | Practice |
|---|---|
| Stack | xUnit v3 3.2.2, FluentAssertions 8.10.0, AutoFixture 4.18.1 with Moq 4.20.72 through `AutoFixture.AutoMoq`. Four test projects still use xUnit v2 (`TECH-STACK.md` section 4.2) |
| Class names | `<TypeUnderTest>Tests` (`StringExtensionsTests`, `ActionHandlerManagerTests`) |
| Method names | `<Member>_should_<behaviour>`, for example `RequiredTrue_should_not_throw_exception_when_condition_is_true`. In `Common.Tests`, 621 of 660 test methods follow it; about 770 methods across `tests/` do. A scenario name may replace the member name in integration-style tests |
| Style | Arrange/Act/Assert, `act.Should().Throw<...>()`, `[Theory]` with `[InlineData]`/`[MemberData]`, `[AutoMockData]` for mocked dependencies, `TestContext.Current.CancellationToken` |
| Shared helpers | `Ploch.TestingSupport.XUnit3(.AutoMoq)`: `[JsonFileData]`, `[TextFileLinesData]`, `[AutoMockData]`, `AlphabeticalOrderer`. Data files resolve against `AppContext.BaseDirectory` and must be copied to the output directory |
| Contract tests | `tests/Common.Serialization.Tests` holds abstract base classes (`JsonSerializerTests<T>` and others) that each implementation's test project subclasses |
| Fixtures | `tests/TestAssemblies/*` are assemblies loaded by the `TypeLoader` tests; `TestingSupport.MockConsoleApp` is a stub program for process tests |
| Size | 804 `[Fact]`/`[Theory]` attribute lines under `tests/` (651 in `Common.Tests`) and 15 more in test projects under `src/`; 2,266 test cases pass and 38 are skipped on a full run (`TECH-STACK.md` section 3). A `[Theory]` counts once in the attribute figure |

Gaps: handler priority and fall-through, hosting, AppServices(.Web), the Minimal API and FastEndpoints adapters and
`Common.ObjectBuilder` have no tests ([`ARCHITECTURE.md`](ARCHITECTURE.md) section 7).

## 5. Documentation and change log

- **Per-library pages** in `docs/libraries/` (index: `docs/INDEX.md`), the generated `docs/API_REFERENCE.md`, and the
  DocFX site built from `DocumentationSite/docfx.json`. Public behaviour changes should update the relevant page.
- **A change-log file per change.** `change-log/YYYY-MM-DD-<issue>-<slug>.md`, with an H1 that ends in `(#NNN)`; every
  pull request in the sample added one. The release workflow concatenates the files into the release notes and moves
  them to `change-log/archive/`. The written rule (`.claude/rules/change-log.md`) says the id carries a `#`; only the
  two oldest files do.
- `RELEASE_NOTES.md` (v2.1) and `release-notes.md` (2.0.1) are stale and differ only in case; the live notes come from
  `change-log/`.

## 6. Commits

- **Conventional Commits**: `type(scope): Subject`, imperative, capitalised. In the last 40 commits: `chore` 9, `docs`
  7, `ci` 7, `fix` 6, `test` 3, `build` 3, `refactor` 1, `perf` 1, `feat` 1, and 2 subjects that are not conventional
  (both direct pushes). Scopes seen: `site`, `release`, `common`, `ci`, `workflows`, `serialization`, `testing-support`,
  `docs`, `webui`, `webapi`, `maui`, `solution`.
- **Footers:** `Refs: #<GitHub issue number>` (118 occurrences in those 40 commits); `BREAKING CHANGE:` with `!` in the
  header (4 of 40 headers).
- **Merge style is squash**: every commit that landed a pull request is a single-parent squash commit, and 38 of 40
  subjects end in `(#<PR number>)`.
- AI-assistant `Co-Authored-By` trailers are present on most recent commits (195 trailer lines in the whole history).

## 7. Branches and worktrees

- **Practice:** `<type>/<GitHub issue number>-<slug>`, for example `feature/347-assignableto-guards`,
  `docs/204-reflection`, `ci/279-...`, `chore/342-...`. Types match the commit types.
- Branches are deleted on merge (repository setting). Stale remote branches remain:
  `cursor/development-environment-setup-9926`, `gh-book`, `kploch-patch-1`, `project-cleanup`, `templates`.
- Two `.claude/worktrees/*` entries are committed as gitlinks without a `.gitmodules`; do not add agent worktrees to the
  index.

## 8. Pull requests and merge

- Template: `## Describe your changes`, `## Issue ticket number and link`, a checklist. Authors usually add Changes,
  Design Decisions, Testing, Not covered, Related and Breaking Changes sections. Bot summaries (Sourcery, CodeRabbit,
  CodeAnt) are appended to the body.
- **Closing keywords use GitHub issue numbers**: `Closes #329`, `Refs #290`. The size label (`size:S` to `size:XL`) is
  applied automatically.
- A pull request normally adds a change-log file and updates the affected `docs/libraries/` page.
- Five to twelve review bots comment on each pull request; in the sampled ten, every inline thread was resolved before
  merge (conversation resolution is required).
- **Merge requires** the four status checks, an approving review, a code-owner review (no `CODEOWNERS` file exists) and
  resolved conversations. Definition of done in practice: all checks green on the latest commit, every thread answered
  or resolved, change log and docs updated.

## 9. Issues

- In-repository history uses **GitHub issue and pull-request numbers** (`#279`, `Refs: #290`). No commit message or
  pull-request body in the history refers to any other identifier scheme.
- The maintainer also keeps a private tracker with its own identifiers; those identifiers do not appear in this
  repository, and nothing here depends on them. See section 10 for the consequence for new work.
- Follow-up work found in review is filed as an issue, not left in a comment.

## 10. Where practice differs from the written rules

| Rule says | Practice | Note |
|---|---|---|
| `CLAUDE.md` (before this knowledge base): "Most other libraries target `netstandard2.0` only" | 19 of 41 `src` projects use net10.0, 11 are `netstandard2.0` only | Corrected in `TECH-STACK.md` section 4.3 |
| `CLAUDE.md`: "27+ source projects" | 41 under `src/`, 21 under `tests/` | — |
| `CLAUDE.md`: "Warnings as errors for Nullable and NU1605" | `TreatWarningsAsErrors=false`; the setting is in three projects; the `.editorconfig` makes about 50 nullable rules errors | — |
| `CLAUDE.md`: the four extra analysers apply to the repository | They are referenced only by `Ploch.Common.csproj` | — |
| `CLAUDE.md` sample: `ConfigureServices(IServiceCollection, IConfiguration)` on a bundle | The API is `DoConfigure()` / `Configure(IConfiguration)` | — |
| `CLAUDE.md`: `ActionHandlerManager` "dependency resolution" | First-success-wins fallback chain, no resolution | — |
| `CLAUDE.md`: `[TextFileData]` reads a file; `[JsonFileData(file, "propertyPath")]` | `TextFileDataAttribute` is abstract; `JsonFileData` takes one `propertyName` | — |
| `CLAUDE.md` Git workflow: branches `#<issue>-description` | `<type>/<number>-<slug>` | — |
| Maintainer workspace rules (not in this repository): branches `<type>/plo-<n>-<slug>`, commit footer `Refs: PLO-<n>`, pull-request keyword `Closes PLO-<n>` | At the snapshot, `PLO-` appeared nowhere in the history or pull-request bodies; the knowledge-base pull request (`PLO-820`) is the first to use it | **Resolved:** new work uses the Linear identifier; earlier GitHub-number references stay valid (see `CLAUDE.md`, Workflow) |
| `change-log.md`: file name contains `#taskid` | No `#` after the two oldest files; some files have no id at all | The rule file is also stale on the tracker |
| `todo-tasks-execution.md`: record follow-ups as GitHub issues, run `/execute-todo` | That skill does not exist | Stale |
| Tests live under `tests/` | `Common.WebUI.Tests`, `Common.Maui.Tests` (+2 fixtures), `WebApi.Endpoints.Tests`, `CrudEndpoints.IntegrationTests` are under `src/` | The two MAUI fixtures are consequently packable |
| Tests use xUnit v3 | Four projects still use xUnit v2 | — |
| Test names are `<Member>_should_<behaviour>` | 39 of 660 methods in `Common.Tests` and several in `TestingSupport` deviate (`EnrollStudent_Success`, a trailing underscore, a template test that asserts nothing) | — |
| Never set `RootNamespace`; no target framework in a project name | `Common.Net9` and its tests override it; the name says `Net9` but it builds net10.0 | A rename to `AssemblyLoading` is planned and not done |
| Dawn.Guard is deprecated | Still referenced, unused, by `Common.Serialization` and `Common.DependencyInjection` | — |
| Required code-owner review | No `CODEOWNERS` file | — |
| `README.md` licence section | The text is the placeholder `[Your License Here]`; the licence is Apache-2.0 | A defect in the README |
| `.codacy.yml` is listed by every solution | The file is committed as `.codacy.yml.test` | — |
| Warnings policy for XML docs | Public types without documentation exist (section 3) | The build does not fail |
