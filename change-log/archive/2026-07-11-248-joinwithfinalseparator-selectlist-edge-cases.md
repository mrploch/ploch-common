## Fix: Harden `JoinWithFinalSeparator` and `SelectListHelper` edge cases

- **Bug fix** in `Ploch.Common.Collections.EnumerableExtensions.JoinWithFinalSeparator`: an empty source threw `IndexOutOfRangeException` and a single-element source incorrectly prepended the final separator (e.g. `" and x"`). An empty source now returns `string.Empty` and a single-element source returns just that element, matching `string.Join` boundary semantics.
- **Bug fix** in `Ploch.Common.WebUI.TagUtilities.SelectListHelper.CreateFor`: a `textFunc`/`valueFunc` delegate returning `null` for an item caused a `NullReferenceException`. Null delegate results now map to `string.Empty` for the item's `Text`/`Value`.
- Added regression tests for the empty, single-element, and two-element `JoinWithFinalSeparator` boundaries and for null `SelectListItem` text/value delegate results.

Refs: #248
