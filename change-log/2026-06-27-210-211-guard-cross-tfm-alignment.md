## Fix: Align `Guard` argument-checking semantics across `netstandard2.0` and `net7.0+` targets

`Ploch.Common` multi-targets `netstandard2.0;net8.0`, and `Guard` is a partial class whose body is selected by target framework (`Guard.cs` for `netstandard2.0`, `GuardNet7.cs` for `net7.0+`). Two divergences between the two partials meant that the same call site behaved differently depending on which asset NuGet resolved for the consumer's TFM. Both are now aligned on the `net7.0+` behaviour.

### Breaking Changes

- **`Guard.RequiredNotNull<T>` (reference-type and value-type overloads) and `Guard.RequiredNotNullOrEmpty` — `netstandard2.0` parameter order changed (issue #210).** The `netstandard2.0` partial previously declared `(this T? argument, string? memberName, string? message = null)` — the **inverse** of the `net7.0+` partial's `(this T? argument, string? messageFormat = null, string? memberName = null)`. The two are now identical: the first optional string is always `messageFormat`, the second is always `memberName`.
  - A `netstandard2.0` consumer calling `value.RequiredNotNull("argName")` previously treated `"argName"` as the **member name**; it is now treated as a **composite format string**, matching `net7.0+`. Pass the name explicitly via `value.RequiredNotNull(memberName: "argName")` to preserve the old intent.
  - This is a **binary-breaking** change to the `netstandard2.0` asset (the IL signatures of the affected methods change), in addition to the source/semantic change. It warrants a **major version bump** at release time.

### Fixes

- **`Guard.NotNullOrEmpty(string?)` empty-string message aligned with the BCL (issue #211).** The `netstandard2.0` partial threw `ArgumentException("Argument cannot be null or empty.")` for an empty string, whereas the `net7.0+` partial delegates to `ArgumentException.ThrowIfNullOrEmpty`, producing `"The value cannot be an empty string."`. The `netstandard2.0` partial now raises the same `"The value cannot be an empty string."` text. The enumerable `NotNullOrEmpty<TEnumerable>` overload is unchanged (it already produced consistent text across both targets).
- **Default exception messages unified.** The `netstandard2.0` `RequiredNotNull`/`RequiredNotNullOrEmpty` now use the shared `"Variable {0} cannot be null."` / `"Variable {0} cannot be empty."` formats (and drop the `netstandard2.0`-only "both memberName and message cannot be null" guard), matching `net7.0+`.

### Versioning

Because the parameter-order change is binary-breaking, `version.json` has been bumped from `3.1-prerelease` to `4.0-prerelease`: the next release is **4.0.0**, not 3.1. The previously planned 3.1 scope rolls into 4.0.

### Known limitation (documented)

The `netstandard2.0` target cannot auto-capture `memberName` via `CallerArgumentExpression` (that would require raising the language version), so on `netstandard2.0` the name must be supplied explicitly; when omitted, the `{0}` placeholder is empty (`"Variable  cannot be null."`). The `net7.0+` target auto-captures it. This residual difference is documented in the affected methods' XML `<remarks>`.

### Refs

- #210 (Guard.RequiredNotNull parameter order inverted between partials)
- #211 (Guard.NotNullOrEmpty empty-string message divergence)
