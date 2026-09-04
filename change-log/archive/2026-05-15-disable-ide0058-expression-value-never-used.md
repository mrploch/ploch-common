## Disable IDE0058 ("Expression value is never used") globally

### Code quality

- **Updated `.editorconfig`** to set `dotnet_diagnostic.IDE0058.severity = none`. The rule was producing 137+ false positives on SonarCloud and SonarLint, primarily on intentional discards of `ArgumentChecking` extension-method results.

### Why

`ArgumentChecking` extension methods such as `Guard.NotNull`, `PathGuard.RequireValidPath`, `Guard.NotNullOrEmpty`, and `Guard.NotOutOfRange` return their input argument so callers can fluent-chain them when convenient, but their **primary purpose is the validation side-effect** — throwing on bad input. Callers idiomatically write `arg.NotNull(nameof(arg));` and discard the result. Roslyn's `IDE0058` (surfaced by SonarCloud as `external_roslyn:IDE0058`) flags every such call as "Expression value is never used", producing many false positives.

The rule also fires on numerous other intentional fluent-API discards in the codebase:

- `StringBuilder.Append(...).Append(...)` chains where the final result isn't reassigned
- `ICollection<T>.Add(...)` (returns `bool` indicating "was added")
- LINQ-for-side-effect helpers (`ForEach` over results of `Where`)
- Reflection helpers and `TestingSupport` setup methods that mutate state and return `this`

### Why per-method suppression isn't possible

`IDE0058` fires **on the caller**, not on the method being called. C# has no built-in attribute that lets a method declare "discarding my return value is allowed" (`[Pure]` does the opposite — it tells callers they *must* use the result). Suppression would therefore require either:

- `_ = method();` discards at every call site (130+ sites; impractical to apply retroactively, and noisy on every new call going forward), or
- Per-file `[SuppressMessage]` / `#pragma warning disable IDE0058` (same problem at scale).

### Trade-off

Other analysers cover most of the cases `IDE0058` does, but on a **narrower scope**:

- **`csharpsquid:S2201`** ("Return values from functions without side effects should not be ignored") — Sonar's nearest equivalent. Correctly recognises that side-effecting methods like `Guard.NotNull` are fine to call without using the return value, which is why it does not fire on the false-positive cases.
- **`CA1806`** ("Do not ignore method results") — fires only on a specific list of known pure / no-side-effect APIs (string methods, `Immutable*` types, etc.).

The genuine residual risk is **lazy-LINQ chains**: something like `myList.Where(x => x.Active)` with no `.ToList()` / no assignment is a real bug that `IDE0058` would catch but `S2201` and `CA1806` will not. We accept this trade-off because:

1. The rule produced 137+ false positives on master at the time of this change, almost all on intentional fluent / side-effect discards. Signal-to-noise is too low to be useful.
2. The wider Sonar issue list — surfaced after the coverage-instrumentation fixes (#218 / #219 / #220) — is what motivated this; the rule was actively obscuring the genuine code smells in the project.
3. The existing IDE preference `csharp_style_unused_value_expression_statement_preference = discard_variable:suggestion` (line 163 of `.editorconfig`) is **left in place**, so `_ = method()` remains the documented refactoring suggestion when anyone wants it. The hint just no longer fails / clutters Sonar.

### Verification

- Local `dotnet build ./Ploch.Common.slnx -c Release` after the change: zero `IDE0058` occurrences, zero errors.
- The next master analysis after merge should show `external_roslyn:IDE0058` drop out of the Sonar issue list entirely.
