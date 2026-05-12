# ADR-0001: Multi-target the test project to cover both `Ploch.Common` binaries

- **Status:** Accepted
- **Date:** 2026-05-09
- **PR:** [#209](https://github.com/mrploch/ploch-common/pull/209)
- **Issue:** [#207](https://github.com/mrploch/ploch-common/issues/207)

## Context

`Ploch.Common` ships **two** compiled outputs in its NuGet package: a `net8.0` binary and a `netstandard2.0` binary. Until #209, CI only ever exercised the `net8.0` binary — the `#if NETSTANDARD2_0` blocks in `Guard` and `PathGuard`, plus the `BitConverter.ToString(...)` fall-through branches in `Hashing`, were shipping untested. Issue #207 needed a way to run the full xunit v3 suite against the `netstandard2.0` binary as well, so consumers resolving to it (older Mono, .NET Framework 4.7.2+, Unity, etc.) get the same regression coverage as `net8`+ consumers.

Two options were on the table:

- **Option A** — resurrect a separate, side-by-side test project (e.g. `Ploch.Common.Tests.NetStandard2`) targeting `net6.0` or `net7.0`, which would consume the `netstandard2.0` binary by virtue of NuGet's TFM resolution.
- **Option B** — multi-target the existing `Ploch.Common.Tests` project to two TFMs (`net10.0;net8.0`) and use MSBuild's `SetTargetFramework` ProjectReference metadata to force the `net8.0` test leg to consume the `netstandard2.0` binary.

Two constraints shaped the choice:

- **xunit v3 does not support `net6.0` or `net7.0`.** Option A would have required either pinning the side project to xunit v2 (splitting the project across two test frameworks) or inventing a custom path that runs xunit v3 on a TFM xunit doesn't ship for.
- **An assembly's TFM is a *compile-time* contract, not a runtime requirement.** A `netstandard2.0` DLL is content-equivalent under any compatible runtime — what matters is *which* compiled output gets loaded into the test process, and that's something MSBuild's `SetTargetFramework` ProjectReference metadata controls directly.

## Decision

Multi-target `Ploch.Common.Tests` to `net10.0;net8.0` (option B). The `net10.0` leg uses NuGet's nearest-TFM resolution and loads the `net8.0` binary. The `net8.0` leg explicitly forces `netstandard2.0` consumption via:

```xml
<ProjectReference Include="..\..\src\Common\Ploch.Common.csproj">
  <SetTargetFramework>TargetFramework=netstandard2.0</SetTargetFramework>
</ProjectReference>
```

Tests that depend on `NET7_0_OR_GREATER` API members (`GuardNet7Tests.cs`, `PathGuardNet7Tests.cs`, `EnumerableQueriesTests.cs`) are excluded from the `net8.0` leg via `<Compile Remove>`. A new `PathGuardNetStandard2Tests` (compiled only on the `net8.0` leg) covers `PathGuard.RequireValidPath` — a member that exists *only* on the `netstandard2.0` partial.

To keep the test infrastructure compatible with both legs, `Ploch.TestingSupport.XUnit3` and `Ploch.TestingSupport.MockConsoleApp` were also multi-targeted.

## Consequences

**Positive:**

- Both shipped binaries are exercised by CI. Local results: `net10.0` leg → 804 passed (loads net8.0 binary); `net8.0` leg → 762 passed (loads netstandard2.0 binary). The 42-test delta is the three excluded net7+-only test files plus the eight netstandard2.0-only `PathGuardNetStandard2Tests`.
- `Hashing` parity is verified implicitly: `HashingTests` asserts specific expected hash strings and now runs against both binaries, so the netstandard2.0 fall-through and the `Convert.ToHexString` branches both produce identical output.
- The whole test infrastructure stays on a single test framework (xunit v3) with shared helpers and shared source; no parallel xunit v2 stack to maintain.
- Future divergences between the two `Guard`/`PathGuard` partials are caught early. The netstandard2.0 leg has already surfaced two latent inconsistencies (parameter order on `Guard.RequiredNotNull<T>` and divergent `Guard.NotNullOrEmpty` messages) that would have slipped past a single-leg suite.

**Negative / trade-offs:**

- The `<Compile Remove>` lists must stay in sync when new net7+-only test files are added — new contributors won't immediately know this.
- The `SetTargetFramework` mechanism is less well-known than a separate test project; it relies on MSBuild ProjectReference metadata that's easy to miss when reading the `.csproj`. (Mitigated by this ADR and an inline comment in the project file.)
- The CI build matrix grows: every CI run now compiles `Ploch.Common.Tests` twice and runs both test legs.

**Follow-up:**

- The two `Guard` partial inconsistencies surfaced during this work are tracked separately — see PR #209 description for details. Likely candidates for their own GitHub issues.
- If an `xunit v3` ever adds support for `net6/net7` the side-project approach (option A) becomes more attractive; revisit this ADR if that happens.

## Considered Alternatives

### Option A — Separate side-by-side test project on xunit v2

Resurrect a `Ploch.Common.Tests.NetStandard2` project targeting `net6.0` or `net7.0` and consuming the `netstandard2.0` binary directly via TFM resolution.

**Rejected because:**

- xunit v3 doesn't support `net6.0` / `net7.0`, so this would have forced a split across two test frameworks (xunit v2 in the side project, xunit v3 in the main project).
- That split would propagate into every `TestingSupport.XUnit*.AutoMoq` wrapper — they would all need to multi-target as well, doubling maintenance.
- Test source tends to diverge over time as features get added to one project and forgotten in the other, undermining the reason the side project exists.

### Option C — `#if NETSTANDARD2_0` inside the existing test files

Keep a single test project, but use preprocessor directives to gate net7+-only assertions.

**Rejected because:**

- A single-TFM project can only load one of the two compiled binaries, so even with `#if`-gated tests we'd still only cover the `net8.0` binary. This option doesn't address the core problem.
- Mixing TFM gating with test logic in the same file is hard to read and easy to break when refactoring.

## References

- PR [#209](https://github.com/mrploch/ploch-common/pull/209) — Cover the netstandard2.0 build of Ploch.Common
- Issue [#207](https://github.com/mrploch/ploch-common/issues/207) — original report
- Issue [#159](https://github.com/mrploch/ploch-common/issues/159) — `Guard.NotOutOfRange` flag-enums fix that originally exposed this gap
- PR [#205](https://github.com/mrploch/ploch-common/pull/205) — predecessor PR
- MSBuild docs: [Common MSBuild project items — `ProjectReference` metadata](https://learn.microsoft.com/en-us/visualstudio/msbuild/common-msbuild-project-items#projectreference)
- Workspace note: `notes/ploch-common/2026-05-09-pr-209-netstandard2-coverage.md` (local journal entry for this PR)
