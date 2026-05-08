## Test coverage for the netstandard2.0 build of Ploch.Common

### Tests

- **Multi-targeted `Ploch.Common.Tests`** to `net10.0;net8.0`. The net10.0 leg keeps consuming the `net8.0` binary of `Ploch.Common`; the new net8.0 leg uses `SetTargetFramework=netstandard2.0` on its `ProjectReference` to force consumption of the netstandard2.0 binary. As a result every test case that does not depend on a `#if NET7_0_OR_GREATER` member now runs against **both** compiled outputs of the package, exercising the previously-unreached `#if NETSTANDARD2_0` and `BitConverter.ToString` fall-through code paths in `Guard`, `PathGuard`, and `Hashing`.
- **New `PathGuardNetStandard2Tests`** (compiled on the net8.0 leg only) covers the netstandard2.0-only `PathGuard.RequireValidPath` extension method (which was renamed to `RequiredIsValidPath` on the net7+ partial), plus parallel coverage of `EnsureFileExists` and `IsValidPath` against the netstandard2.0 implementations.
- **Hashing parity** is now implicit: the existing `HashingTests` asserts specific expected hash strings and runs on both legs, so the netstandard2.0 `BitConverter.ToString(...).Replace("-", string.Empty)` fall-through and the net6+ `Convert.ToHexString(...)` branch are both verified to produce identical output.
- **Scoping fixes in `GuardTests`:** removed a duplicate `RequiredNotNull` test that exercised net7+-specific message-format ordering (already covered by `GuardNet7Tests.cs`) and relaxed the wildcard on `NotNullOrEmpty_should_throw_for_empty_string` so it matches both BCL message texts.
- Removed the orphaned `tests/central-mgmt-disabled/` scaffold (parked since the xunit v3 migration; superseded by the multi-target approach above).

### Test infrastructure

- Multi-targeted `Ploch.TestingSupport.XUnit3` to `net10.0;net8.0` and `Ploch.TestingSupport.MockConsoleApp` to `net10.0;net8.0` so they can be referenced from the net8.0 leg of `Ploch.Common.Tests`.

### Refs

- #207 (test: cover the netstandard2.0 build of Ploch.Common)
- Builds on #159 / #205 (the flag-enums `Guard.NotOutOfRange` fix that motivated discovering this gap).

### Discoveries surfaced by the new coverage

The first runs on the netstandard2.0 binary uncovered two latent API inconsistencies between the two `Guard` partials. Both are documented as code comments in the test files for now and are candidates for follow-up issues:

- `Guard.RequiredNotNull<T>` has its `(memberName, message)` parameter order **inverted** between the netstandard2.0 partial (`memberName` first) and the net7+ partial (`messageFormat` first, `memberName` from `[CallerArgumentExpression]`). A single positional argument therefore has different semantics depending on which binary is loaded.
- `Guard.NotNullOrEmpty(string?, …)` raises a different message on each binary: net7+ delegates to `ArgumentException.ThrowIfNullOrEmpty` ("The value cannot be an empty string."), netstandard2.0 throws explicitly with "Argument cannot be null or empty.".
