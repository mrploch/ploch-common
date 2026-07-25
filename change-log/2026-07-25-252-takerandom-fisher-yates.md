# TakeRandom optimised with a partial Fisher-Yates shuffle

**Issue:** [#252](https://github.com/mrploch/ploch-common/issues/252)

## Changed

- `EnumerableExtensions.TakeRandom<T>` now selects items with a partial Fisher-Yates shuffle
  over an index array instead of repeated `List<T>.RemoveAt` calls, improving complexity from
  O(count × n) to O(n) setup + O(count) selection. Uniform selection probability and
  no-duplicate-picks behaviour are unchanged.
- Behaviour for a zero or negative `count` is now explicitly documented: an empty sequence is
  returned, matching `Enumerable.Take` semantics (deliberate decision — no breaking change).
