---
uid: Ploch.Common.Randomizers
summary: *content
---

### Overview

`Ploch.Common.Randomizers` provides a small abstraction over random value generation, so that "produce a random
value of type `T`" becomes an injectable dependency rather than a direct call to `Random`. The point is
testability and composition: code that depends on `IRandomizer<T>` can be handed a deterministic stub in a unit
test, and a container can resolve the right generator for a type without the consuming code knowing which
concrete implementation is in play.

Implementations are supplied for the types that come up most often — `int`, `bool`, `string`, `DateTime` and
`DateTimeOffset` — with `IRangedRandomizer<T>` adding bounded generation where a range is meaningful.
`BaseRandomizer<T>` is the extension point for adding a generator for a domain type of your own. This namespace
is a convenience layer for seeding test data and sampling, not a cryptographic facility; use
`System.Security.Cryptography` where unpredictability is a security requirement.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
