# #204 — Collections and enumerables article for the documentation site

**Type:** documentation
**Breaking changes:** none

## Summary

Added an authored article covering the `Ploch.Common.Collections` namespace to the Articles section of the
documentation site, organised around the task a developer is trying to accomplish rather than as a flat
API listing.

## Details

New page `DocumentationSite/articles/collections-and-enumerables.md`, wired into
`DocumentationSite/articles/toc.yml`.

It covers:

- membership and emptiness tests — `ValueIn`, `None`, `IsEmpty`, `IsNullOrEmpty` — and when each beats the
  equivalent LINQ expression;
- conditional query composition with `If`, and the eager-iteration semantics of `ForEach`;
- string building with `Join` and `JoinWithFinalSeparator`, including the empty and single-element cases;
- bulk mutation through `AddMany` on both collections and dictionaries, the `DuplicateHandling` policies,
  and the `params` overload's reversed argument order;
- `Add` / `AddIfNotNull` chaining over `ICollection<KeyValuePair<TKey, TValue?>>`;
- sampling with `Shuffle` and `TakeRandom`, their `System.Random` (non-cryptographic) basis, and the fact
  that sampling is without replacement over *positions* — a source containing equal values can return the
  same value more than once;
- the smaller helpers `GetWithEmptyProperty`, `AreIntegersSequentialInOrder`, `Second`, `ExceptItems` and
  `ArrayExtensions.Exists`, including `ExceptItems` inheriting set semantics and dropping duplicates;
- a per-method summary of how often each helper enumerates its source;
- a per-method summary of argument guarding, recording exactly which helpers validate their own arguments
  and which delegate to LINQ — and therefore surface LINQ's parameter names, or, for a null
  `GetWithEmptyProperty` selector, a deferred `NullReferenceException` instead of an `ArgumentNullException`.

A caveat was added that `AddMany` is not an atomic merge and so is unsafe against a concurrently written
`ConcurrentDictionary`, despite being generic over `IDictionary<TKey, TValue>`.

Every code sample was compiled against the real `Ploch.Common` API, and the documented sampling and
argument-guarding behaviour was verified by executing it.

## Review corrections

Three points raised in review were verified by execution and corrected:

- **`TakeRandom` sampling weights.** The advice to call `source.Distinct().TakeRandom(count)` for distinct
  values was followed by a sentence stating that a value repeated five times is five times likelier to be
  drawn — which describes the behaviour *without* `Distinct()`, and so contradicted the advice it was
  attached to. The passage now separates the two: occurrence weighting is a property of the raw source,
  and `Distinct()` removes it, giving every surviving value equal probability. Measured over 200,000
  trials: without `Distinct()` a five-times-repeated value is drawn 50.2% of the time against 9.9% for a
  singleton (5.05x); with `Distinct()` both are drawn 16.7% of the time (1.00x).
- **`AreIntegersSequentialInOrder` numeric wraparound.** The successor test is `array[i] + 1` in unchecked
  arithmetic, so "exactly one greater" holds only modulo the integer range. Confirmed by execution that
  both `[int.MaxValue, int.MinValue]` and `[long.MaxValue, long.MinValue]` return `true`. The contract now
  documents the wraparound instead of overstating the guarantee.
- **`collections-samples.md` accuracy.** Rather than documenting a disagreement between the two pages, the
  sibling reference was corrected: `AreSequentialInOrder<T>` → `AreIntegersSequentialInOrder` and
  `NullOrEmpty` → `IsNullOrEmpty`, with signatures taken from the source. All twelve samples in that file
  were compiled and executed against the real API. The now-obsolete "where the two disagree" disclaimer was
  removed from this article. Remaining coverage gaps in that reference are tracked in #332.

No changes to shipped library code — packages are unaffected.
