---
uid: Ploch.Common.Reflection
summary: *content
---

### Overview

`Ploch.Common.Reflection` exists to make reflection code readable and, where possible, safe. Raw
`System.Reflection` answers questions in terms of metadata rather than intent: asking "does this type implement
`IFoo`?" or "is this a concrete implementation I could instantiate?" or "what would a human call this generic
type?" each takes several lines and a couple of easy-to-get-wrong edge cases. The extension methods here answer
those questions directly, and `GetReadableTypeName` in particular turns mangled names such as
``Dictionary`2`` into something fit for a log message or an error.

The namespace's second theme is discovery and property access. `TypeLoader` and `AssemblyListBuilder` scan
assemblies for implementations of a contract, which is how plug-in and convention-based registration is usually
bootstrapped. The property helpers wrap the failure modes of dynamic access in specific exception types —
`PropertyNotFoundException`, `PropertyReadOnlyException`, `PropertyWriteOnlyException`,
`PropertyIndexerMismatchException` — so a caller can distinguish "you asked for the wrong name" from "you asked
to write to something read-only" without parsing a message. `ByValueObjectComparator` and
`ObjectHashCodeBuilder` provide structural equality for types that do not implement it themselves, which is
useful in tests and in change detection.

See the [Ploch.Common library guide](../../docs/libraries/common.md) for installation instructions and worked
examples.
