## Refactor: Resolve analyzer warnings in shipping libraries (`src/`)

Cleaned up the analyzer-warning backlog across the shipping `src/` libraries (split out from the test-project cleanup in #233). The bulk are documentation, style, and code-quality fixes with no behavioural impact, but a few are user-visible:

- **New public API — `EnumName.FromString(string?)`** (`Ploch.Common.TypeConversion`): a named alternative to the implicit `string`→`EnumName` conversion operator (satisfies CA2225). Non-breaking addition.
- **`ProcessExtensions` (`Ploch.Common.Diagnostics`)**: `SetSingleProcessorAffinity`, `SetEnabledProcessors`, and `GetEnabledProcessors` are now annotated `[SupportedOSPlatform("windows")]` / `[SupportedOSPlatform("linux")]` (on `net8.0`), so consumers calling them from code reachable on unsupported platforms now get a platform-compatibility analyzer warning (CA1416). The methods also now validate their `process` argument (`ArgumentNullException`) and use `checked` conversions to `IntPtr` (CA2020).
- **`Hashing` (`Ploch.Common.Cryptography`)**: `ToHashString`/`ToMD5HashString` now guard their arguments (`ArgumentNullException`); the legacy `BitConverter`-based fallback was moved into a `#else` branch so it only compiles for pre-`net6` targets (removing dead-code and culture-sensitivity warnings on current TFMs). Output is unchanged.
- **`StopwatchUtil.TimeAsync`**: now awaits with `ConfigureAwait(false)` (CA2007); no behavioural change for callers.

Internal-only changes: added argument null-guards to several public methods (CA1062), XML documentation for previously undocumented public members (CS1591 et al.), and a number of style/formatting fixes. Where a rule genuinely did not apply, a narrowly-scoped, justified suppression was used instead of a fix — notably `CA5394` on the non-security `Randomizers` and collection-shuffle helpers, `CA1032` on the property-access exception hierarchy (which is constructed from a property name, making the standard message-only constructor ambiguous), and `CA1034` on the `DateTimeFormats.DateOnly` format-constant grouping.

No breaking changes.

Refs: #242
