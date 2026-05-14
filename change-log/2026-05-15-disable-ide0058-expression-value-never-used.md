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

### Why this is safe

`IDE0058` catches nothing that other analysers don't already catch on a tighter scope:

- **`csharpsquid:S2201`** ("Return values from functions without side effects should not be ignored") — Sonar's equivalent rule, but it correctly recognises that side-effecting methods like `Guard.NotNull` are fine to call without using the return value.
- **`CA1806`** ("Do not ignore method results") — similar Roslyn rule, fires only on pure / no-side-effect methods.

The codebase already declared the preference at `csharp_style_unused_value_expression_statement_preference = discard_variable:suggestion`, so the `_ = method()` discard remains the documented refactoring suggestion in the IDE if anyone wants it. We just don't enforce it.

### Verification

- Local `dotnet build ./Ploch.Common.slnx -c Release` after the change: zero `IDE0058` occurrences, zero errors.
- The next master analysis after merge should show `external_roslyn:IDE0058` drop out of the Sonar issue list entirely.
