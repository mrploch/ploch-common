# Backlog — ploch-common

| Field | Value |
|---|---|
| Snapshot | 2026-10-10, against commit `1f1764b166dabfad4a01b166b94a1115b88e7c6d` |
| Sources | The repository's public GitHub issues (138), pull requests (211), tags (3) and releases (3); nuget.org; the findings of this review. It does not count any pull request that adds this knowledge base (none had been opened) |
| Verified by | Re-running each list command and checking that the result count is below the cap (`--limit 3000` against 138 issues and 211 pull requests) |
| Entry point | [`CLAUDE.md`](../../CLAUDE.md) |

The maintainer plans work in a tracker with a narrower audience than this public repository, and publishing from it was
not approved. **Kept in a tracker with a narrower audience; not published here:** its counts, themes, priorities,
milestones and roadmap. Everything below comes from the public GitHub issue list, pull requests, tags and releases,
which any reader can see. The public issue list mirrors most of the planned work, but its open/closed state can lag the
tracker's.

## 1. Snapshot

| Measure | Value |
|---|---|
| GitHub issues | 138 in total: **27 open**, 111 closed. Oldest created 2023-03-30, newest 2026-09-04 |
| Milestones | One in use: `Release 4.0` on 18 issues; the other 120 have none |
| Labels on issues | `.NET` 19, `documentation` 13, `enhancement` 10, `refactor and cleanup` 8, `CI` 7, `not-done` 6, `bug` 4, `important` 2 |
| Pull requests | 211 in total: **2 open**, 132 merged, 77 closed without merging (most by dependency bots). `gh pr list --state closed` returns 209 because it includes merged pull requests |
| Tags / releases | Tags `rel-2.0.1`, `v3.0.0`, `v4.0.47`; the same three GitHub Releases, none a draft or prerelease; `v4.0.47` (2026-09-03) is the latest |
| Published packages | 21 package ids on nuget.org, all at 4.0.47 and none a prerelease; 13 packable projects are unpublished ([`TECH-STACK.md`](TECH-STACK.md) section 7) |
| Development version | `version.json` is `4.1-prerelease`; four change-log entries (2026-09-03 and 2026-09-04) are waiting for the next release |
| Delivery health | The Publish Docs workflow fails on `main`, so the documentation site is not being deployed ([`TECH-STACK.md`](TECH-STACK.md) section 6) |

## 2. In progress

No open issue has a pull request of its own. The two open pull requests:

| Pull request | State |
|---|---|
| [#352](https://github.com/mrploch/ploch-common/pull/352) `cursor/cloud-env-setup-22a2` | Draft, about a week old, changes `AGENTS.md` only. 14 checks pass, 2 skipped, **`build-docs` fails** (the same documentation guard as on `main`). Review required |
| [#351](https://github.com/mrploch/ploch-common/pull/351) `docs/204-usage-sample-articles` | About four weeks old, **conflicts with `main`**, 14 files (+4,120/−101). Its own description calls it a stale branch recovered from an old worktree, not ready to merge and superseded by smaller documentation pull requests. Checks pass. Not alive: close it or salvage from it |

Stale remote branches with no pull request: `cursor/development-environment-setup-9926`, `gh-book`, `kploch-patch-1`,
`project-cleanup`, `templates` (one to four commits ahead and 107 to 507 behind `main`).

## 3. Planned

The 27 open issues, newest first. None carries a priority on GitHub. Areas are the paths each issue concerns.

| Issue | Summary | Area |
|---|---|---|
| [#349](https://github.com/mrploch/ploch-common/issues/349) | Finish `Common.ObjectBuilder` and bring it under CI | `src/Common.ObjectBuilder`, `tests/Common.ObjectBuilder.Tests` |
| [#346](https://github.com/mrploch/ploch-common/issues/346) | The symbol push to GitHub Packages is suppressed (`\|\| true`) and may be a no-op | `.github/workflows/build-dotnet.yml` |
| [#344](https://github.com/mrploch/ploch-common/issues/344) | Move `Common.WebUI.Tests` out of `src/` | `src/Common.WebUI.Tests` |
| [#343](https://github.com/mrploch/ploch-common/issues/343) | Harden the `src/` and `tests/` root predicates against prefix matching | `Directory.Build.props` |
| [#341](https://github.com/mrploch/ploch-common/issues/341) | Retire stale and superseded `Ploch.*` NuGet packages and orphan source folders | nuget.org, `src/Common.Web`, `src/Common.NSwag` |
| [#332](https://github.com/mrploch/ploch-common/issues/332) | Complete `EnumerableExtensions` coverage in `collections-samples.md` | `DocumentationSite/articles/` |
| [#327](https://github.com/mrploch/ploch-common/issues/327) | `TestingSupport.XUnit3.Dependencies` ships fewer packages than documented; stale legacy nuspec | `src/TestingSupport.XUnit3.Dependencies`, `src/TestingSupport.Dependencies.MetaPackages` |
| [#325](https://github.com/mrploch/ploch-common/issues/325) | `RequiredIsValidPath` does not forward `parameterName` on its null branch | `src/Common/ArgumentChecking/PathGuardNet7.cs` |
| [#324](https://github.com/mrploch/ploch-common/issues/324) | netstandard2.0 `RequireValidPath` throws `InvalidOperationException`; `EnsureFileExists` is not portable | `src/Common/ArgumentChecking/PathGuard.cs` |
| [#320](https://github.com/mrploch/ploch-common/issues/320) | Decide the public shape of the generated icon-font classes | `src/Common.Maui/Common.Maui.Fonts` |
| [#319](https://github.com/mrploch/ploch-common/issues/319) | XUnit3 `TextFileLinesDataAttribute` splits on `Environment.NewLine` only | `src/TestingSupport.XUnit3/TestData/` |
| [#318](https://github.com/mrploch/ploch-common/issues/318) | Guard `filePath` at the `TestDataFilePathResolver` boundary | `src/TestingSupport/TestData/` |
| [#316](https://github.com/mrploch/ploch-common/issues/316) | Clear two DocFX `InvalidFileLink` warnings, then fail the docs build on warnings | `docs/`, `DocumentationSite/docfx.json`, `publish-docs.yml` |
| [#313](https://github.com/mrploch/ploch-common/issues/313) | `SafaParseToEnum` throws instead of returning null for unmapped integers | `src/Common/EnumerationConverter.cs` |
| [#300](https://github.com/mrploch/ploch-common/issues/300) | About 90 analyser warnings in `WebApi.Endpoints.CrudEndpoints`, invisible to CI | `src/Common.WebApi/` |
| [#292](https://github.com/mrploch/ploch-common/issues/292) | `GetByIdEndpointHandler` caching bypasses query operations and can collide across namespaces | `src/Common.WebApi/` |
| [#289](https://github.com/mrploch/ploch-common/issues/289) | Add the MAUI projects to `Ploch.Common.slnx` so CI guards them | `Ploch.Common.slnx`, `src/Common.Maui/` |
| [#288](https://github.com/mrploch/ploch-common/issues/288) | `QueryStringBinder` throws for a read-only property | `src/Common.WebApi/` |
| [#285](https://github.com/mrploch/ploch-common/issues/285) | Decide whether to publish `Ploch.Common.WebApi` (the docs advertise a package that does not exist) | `docs/libraries/common-webapi.md` |
| [#269](https://github.com/mrploch/ploch-common/issues/269) | Modernise workflow actions (Node 20, `set-output`, `setup-java` v5, Qodana arguments) | `.github/workflows/` |
| [#266](https://github.com/mrploch/ploch-common/issues/266) | Secondary solution files reference removed projects | `Ploch.Common.*.slnx` |
| [#244](https://github.com/mrploch/ploch-common/issues/244) | Test-quality suggestions from review bots (deterministic data, post-condition assertions) | `tests/Common.Tests`, `tests/Common.Net9.Tests`, `tests/Common.DependencyInjection.Tests` |
| [#232](https://github.com/mrploch/ploch-common/issues/232) | Cut Ploch.Common v4.0 stable (labels `CI`, `important`). **Probably stale:** `v4.0.47` is a full release and `version.json` is already `4.1-prerelease` | `version.json`, `release.yml` |
| [#208](https://github.com/mrploch/ploch-common/issues/208) | Cache flags-enum metadata per `TEnum` in `Guard.NotOutOfRange` | `src/Common/ArgumentChecking/` |
| [#199](https://github.com/mrploch/ploch-common/issues/199) | Pin GitHub Actions to commit SHAs. Partly done: some pins are SHAs, most are tags | `.github/workflows/` |
| [#158](https://github.com/mrploch/ploch-common/issues/158) | Implement the `Ploch.Common.TypeConversion` generic conversion mechanism (overlaps #349) | `src/Common/TypeConversion/` |
| [#155](https://github.com/mrploch/ploch-common/issues/155) | Replace project-reference cross-dependencies on other repositories with packages | whole repository |

Between now and the next release the visible blockers are the failing Publish Docs workflow, the stale #232, and the
decision on which of the unpublished packages to ship or remove (#285, #341, #349).

## 4. Recently completed — themes

111 issues are closed; 95 of them were closed from March 2026 onward (March 12, April 21, May 11, June 7, July 6, August
28, September 10). 61 of the closed titles carry no conventional-commit prefix (older issues). The 50 that do:

| Type of issue (title prefix) | Closed | Typical subjects |
|---|---:|---|
| `chore` | 13 | `DawnGuard` deprecation (#342), default-branch rename to `main` (#282), retiring `azure-pipelines.yml` (#283), the DocFX warning backlog (#290) |
| `docs` | 8 | documentation-site articles (#204), PR preview deployments (#201), per-namespace overwrites (#200), README sponsorship links |
| `build` | 8 | packaging filters and accidental packing (#331, #279, #278, #280), an `AutoMapper` upgrade for an advisory (#287) |
| `fix` | 7 | web-API operation ids (#298), query binding (#293), `TakeRandom` off-by-one (#246), `CommandLineInfo` hash contract (#247), data-file paths (#309) |
| `ci` | 5 | Trusted Publishing for NuGet (#301), release bookkeeping through a pull request (#337), Qodana de-duplication (#198), Publish Docs failures (#196) |
| `test` | 5 | analyser-warning clean-up (#233), a flaky test (#299), `netstandard2.0` coverage (#207) |
| `refactor`, `perf`, `feat` | 4 | `AssignableTo` guards (#347), MAUI `IViewModel` move (#286), `TakeRandom` optimisation (#252), analyser fixes in `src/` (#242) |

Frequent scopes: `common` 6, `site` 3, `serialization` 3, `release` 3, `ci` 3.

## 5. Recurring problem areas

| Problem area | Evidence | Code or configuration area |
|---|---|---|
| Packability and publish scoping | #343, #344, #346, #327, #341; 13 packable projects unpublished | `Directory.Build.props`, `build-dotnet.yml`, `release.yml`, `scripts/publish-nugetorg.ps1`, `TestingSupport*` projects |
| Projects outside `Ploch.Common.slnx`, so CI never builds them | #289, #300, #349, #266; 10 projects in no solution, two of them not compiling | `Ploch.Common*.slnx`, `src/Common.Maui`, `src/Common.WebApi`, `src/Common.ObjectBuilder` |
| Documentation site health | #316, #332; Publish Docs fails on `main`; pull request #352 red | `DocumentationSite/docfx.json`, `publish-docs.yml`, `docs/` |
| Guard parity between `netstandard2.0` and `net7.0+` | #324, #325, #208 | `src/Common/ArgumentChecking/` |
| Test data attributes | #318, #319 | `src/TestingSupport*/TestData/` |
| Web endpoint correctness | #288, #292, #300; the review findings in section 6 | `src/Common.WebApi/` |
| Supply chain and workflow upkeep | #199, #269; unpinned sibling clone | `.github/workflows/` |
| Stale published packages | #341 | nuget.org, `src/Common.Web`, `src/Common.NSwag` |

## 6. Technical debt register

Findings of this review that no open GitHub issue covers. They are grouped; the evidence is in
[`ARCHITECTURE.md`](ARCHITECTURE.md) section 7 and [`TECH-STACK.md`](TECH-STACK.md) section 9. **No triage issue was
filed**: the review ran without authority to write to any tracker. This list is the proposed content of one.

### Decide before the next release

| Finding | Where |
|---|---|
| The System.Text.Json serializer mutates shared options and throws once they have been used; with `AddSystemTextJsonSerializer()` every configuration overload throws | `src/Common.Serialization.SystemTextJson/SystemTextJsonSerializer.cs:85` |
| Publish Docs fails on `main` (the warning guard trips on a NuGet audit warning); the Pages deploy is stalled | `.github/workflows/publish-docs.yml:125-130` |
| `GetListEndpointHandler` compares `PageNumber * PageNumber`; FastEndpoints maps Create to `PUT` and Update to `POST`; `MapCrudEndpoints` doubles the base path and hard-codes `{id:int}` | `src/Common.WebApi/WebApi.Endpoints.CrudEndpoints*/` |
| `Common.NSwag` and `TestingSupport.FluentAssertions.IOAbstractions` do not compile and nothing builds them; their published-looking documentation remains | `src/Common.NSwag/`, `src/TestingSupport.FluentAssertions.IOAbstractions/` |
| Which unpublished libraries ship: ObjectBuilder, Extensions.Configuration, Dependencies, DependencyInjection.Hosting, Ardalis.Result, Maui, the Endpoints family | `Ploch.Common.slnx`, `docs/libraries/` |
| `Ploch.Common.Net9` is named for a framework it no longer targets; the planned rename to `Ploch.Common.AssemblyLoading` is not done | `src/Common.Net9/`, `docs/` |

### Correctness and documentation

| Finding | Where |
|---|---|
| `IsSimpleType` is true for every value type; `AppDomainTypesLoader` leaks an event subscription and promises thread safety it lacks | `src/Common/Reflection/TypeExtensions.cs:124`, `src/Common.Net9/` |
| `ServicesBundle` can configure a shared dependency twice; `ActionHandlerManager` priority and fall-through are untested | `src/Common.DependencyInjection/`, `tests/Common.Apps/` |
| `CreateEndpointHandler` never commits (whether `AddAsync` persists is unverified) | `src/Common.WebApi/WebApi.Endpoints.CrudEndpoints/` |
| `NullabeTypeConverter` and `SafaParseToEnum` are public typos | `src/Common.ObjectBuilder/`, `src/Common/` |
| Documentation that disagrees with the code: `docs/libraries/common-webapi.md` and several others advertise unpublished packages; `common-net9.md` claims thread safety; `common-serialization-system-text-json.md` contradicts itself; `src/TestingSupport/README.md` says the project is removed from the solution; `README.md` licence section is the placeholder `[Your License Here]` | `docs/libraries/`, `README.md` |
| Unused package references that flow to consumers: `Dawn.Guard` in Serialization and DI, file-system abstractions in DI, five packages in `Ploch.TestingSupport` | project files |
| Duplicated `Ploch.TestingSupport` and `Ploch.TestingSupport.XUnit3` sources, drifting; `TestPriorityAttribute` is never read | `src/TestingSupport*/` |
| Four test projects under `src/` (and two packable MAUI fixtures); four on xUnit v2; three skipped tests in `Common.Tests` (one marked `TODO: Needs fixing`); a template test that asserts nothing | `src/`, `tests/` |

### Build, CI and hygiene

| Finding | Where |
|---|---|
| Branch protection requires code-owner review but there is no `CODEOWNERS`; the Actions Build job is not a required check | `.github/` |
| The sibling repository is cloned unpinned; no `global.json`; CI asks for SDK `9.0.x` while projects target net10.0; the Build job has no `permissions` block | `.github/workflows/`, repository root |
| The release workflow's recorded runs all end in `failure` although publishing works | `.github/workflows/release.yml` |
| Stale overrides and files: `PackageVersion Update` blocks, `dotnet-releaser.toml`, `templates/`, `deps/`, `lib-src-target.editorconfig`, `RELEASE_NOTES.md`/`release-notes.md`, `TODO.md`, `temp.md`, `last-session.md`, generated `DocumentationSite/api-tmp/`, `.codacy.yml.test`, the legacy `docfx_project/`, solution entries for files that do not exist | repository root, `DocumentationSite/` |
| A hook-written session-state file and two agent-worktree gitlinks are committed | `.claude/` |
| A machine-specific `HintPath` to a Windows ASP.NET Core 9.0.0 reference | `src/Common.WebApi/WebApi.Endpoints/*.csproj:14-16` |
| `.editorconfig:309` sets `dotnet_diagnostic.rcs0022 = false`, which is not a valid setting | `.editorconfig:309` |

## 7. Roadmap

Kept in a tracker with a narrower audience; not published here. The public facts: `version.json` is `4.1-prerelease`,
and the only GitHub milestone is `Release 4.0`.

## 8. How to refresh

Re-run, and check that each result count is below its cap before trusting it:

```bash
gh issue list -R mrploch/ploch-common --state all --limit 3000 --json number,title,state,labels,milestone,createdAt,closedAt
gh pr list -R mrploch/ploch-common --state all --limit 3000 --json number,state,mergedAt,headRefName
gh release list -R mrploch/ploch-common --exclude-drafts --limit 100
git ls-remote --tags --refs origin
```

Then re-derive section 3 from the open issues and section 4 from the closed ones. A full refresh uses the
`repo-knowledge-base` skill (`refresh`).
