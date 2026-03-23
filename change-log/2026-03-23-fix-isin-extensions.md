## Fix: IsInExtensions equality check and null guard

- **Bug fix:** `In` and `NotIn` extension methods now correctly match default values (e.g. `0`, `null`) by using `EqualityComparer<TValue?>.Default.Equals` instead of the previous `value.Equals(v)` approach which skipped default values.
- **Null guard:** Added `comparer.NotNull()` validation for `IComparer`-based overloads to fail fast with `ArgumentNullException` instead of `NullReferenceException`.
- **Test coverage:** Added comprehensive tests for enum values, `IEnumerable` overloads, `IComparer` overloads, and `NotIn` variants.
