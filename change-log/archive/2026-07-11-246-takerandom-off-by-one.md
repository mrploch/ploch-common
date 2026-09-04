## Fix: `TakeRandom` never selected the last element (off-by-one)

- **Bug fix** in `Ploch.Common.Collections.EnumerableExtensions.TakeRandom<T>`: the random index was drawn with `Random.Next(0, indexes.Count - 1)`, whose upper bound is exclusive. Because index removal preserves order, the source's last element could never be selected unless the requested count equalled the source size — skewing the selection distribution.
- The upper bound is now `indexes.Count`, making every element reachable with uniform probability.
- Added regression tests asserting that every element (including the last) can be selected, and that a single-element source returns its only element.

Refs: #246
