---
uid: Ploch.Common.Collections
summary: *content
---

### Overview

`Ploch.Common.Collections` fills the gaps LINQ deliberately leaves open. LINQ is excellent at transforming
sequences but says nothing about the surrounding ergonomics: expressing "there are none" without a negated
`Any()`, applying a filter only when an optional parameter was actually supplied, iterating for side effects
while keeping a fluent chain, joining with a different final separator, or adding a range to an existing
collection. Those are the shapes this namespace covers, for `IEnumerable<T>`, `IQueryable<T>`, arrays and
dictionaries alike.

The conditional-query helpers deserve particular mention. `If()` lets a repository or search method compose an
`IQueryable` from a set of nullable filter arguments without a cascade of `if` statements, which keeps
translation to SQL intact. The dictionary helpers add get-or-add and merge semantics with an explicit
`DuplicateHandling` choice, rather than leaving the caller to pick between an exception and silent
overwriting.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
