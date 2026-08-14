# TakeRandom optimised with a partial Fisher-Yates shuffle

**Issue:** [#252](https://github.com/mrploch/ploch-common/issues/252)

## Changed

- `EnumerableExtensions.TakeRandom<T>` now selects items with a partial Fisher-Yates shuffle
  performed directly on a private copy of the source list instead of repeated
  `List<T>.RemoveAt` calls, improving complexity from O(count × n) to O(n) setup +
  O(count) selection. A full-size draw returns the shuffled copy without an extra
  prefix copy. Uniform selection probability and no-duplicate-picks behaviour are
  unchanged.
- Behaviour for a zero or negative `count` is now explicitly documented: an empty sequence is
  returned, matching `Enumerable.Take` semantics (deliberate decision — no breaking change).
