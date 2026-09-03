## Refactor: Resolve analyzer warnings in shipping libraries (`src/`)

Cleaned up the analyzer-warning backlog across the shipping `src/` libraries (split out from the test-project cleanup in #233). The bulk are documentation, style, and code-quality fixes with no behavioural impact, but a few are user-visible:

- **New public API — `EnumName.FromString(string?)`** (`Ploch.Common.TypeConversion`): a named alternative to the implicit `string`→`EnumName` conversion operator (satisfies CA2225). Non-breaking addition.
- **`ProcessExtensions` (`Ploch.Common.Diagnostics`)**: `SetSingleProcessorAffinity`, `SetEnabledProcessors`, and `GetEnabledProcessors` are now annotated `[SupportedOSPlatform("windows")]` / `[SupportedOSPlatform("linux")]` (on `net8.0`), so consumers calling them from code reachable on unsupported platforms now get a platform-compatibility analyzer warning (CA1416). The methods also now validate their `process` argument (`ArgumentNullException`) and wrap the affinity-mask-to-`IntPtr` conversion in `unchecked` (with a justified CA2020 suppression, because the bitmask's high bit must wrap to the native pointer pattern rather than throw `OverflowException`).
- **`Hashing` (`Ploch.Common.Cryptography`)**: `ToHashString`/`ToMD5HashString` now guard their arguments (`ArgumentNullException`); the legacy `BitConverter`-based fallback was moved into a `#else` branch so it only compiles for pre-`net6` targets (removing dead-code and culture-sensitivity warnings on current TFMs). Output is unchanged.
- **`StopwatchUtil.TimeAsync`**: now awaits with `ConfigureAwait(false)` (CA2007); no behavioural change for callers.

Internal-only changes: added argument null-guards to several public methods (CA1062), XML documentation for previously undocumented public members (CS1591 et al.), and a number of style/formatting fixes. Where a rule genuinely did not apply, a narrowly-scoped, justified suppression was used instead of a fix — notably `CA5394` on the non-security `Randomizers` and collection-shuffle helpers, `CA1032` on the property-access exception hierarchy (which is constructed from a property name, making the standard message-only constructor ambiguous), and `CA1034` on the `DateTimeFormats.DateOnly` format-constant grouping.

### Breaking changes

A few public types were sealed or made static to satisfy analyzer recommendations. These are source- and binary-breaking for consumers that subclassed (or, for the static conversions, instantiated) them, and are shipped as part of the `4.0` major release:

- **`Ploch.TestingSupport.Moq.FluentVerifier`** and **`Ploch.TestingSupport.XUnit3.Moq.FluentVerifier`** — changed from `public class` to `public static class`. All members were already static; the implicit public parameterless constructor is removed, so `new FluentVerifier()` and any subclass no longer compile.
- **`Ploch.Common.DataAnnotations.RequiredNotDefaultDateAttribute`** — now `sealed`.
- **`Ploch.TestingSupport.XUnit3.AutoMoq.AutoMockDataAttribute`** — now `sealed`.
- **`Ploch.TestingSupport.TestData.TextFileLinesDataAttribute`** and **`Ploch.TestingSupport.XUnit3.TestData.TextFileLinesDataAttribute`** — now `sealed`.

Consumers that derived from any of these types must stop doing so; the types were not designed for extension.

Refs: #242
