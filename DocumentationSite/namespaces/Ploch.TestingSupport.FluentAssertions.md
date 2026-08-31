---
uid: Ploch.TestingSupport.FluentAssertions
summary: *content
---

### Overview

`Ploch.TestingSupport.FluentAssertions` extends FluentAssertions at the points where its defaults do not match
how these libraries are tested. The most consequential piece is
`NullEmptyCollectionEquivalencyStep`: an equivalency step that treats a null collection and an empty collection
as equivalent. Round-tripping an object through serialisation, mapping or persistence routinely turns one into
the other, and without this step a structural comparison fails on a difference that carries no meaning for the
assertion being made.

The rest of the namespace adds vocabulary rather than semantics. `PropertyInfoCollectionAssertions` and its
extension entry point give reflection-heavy tests a way to assert over a set of `PropertyInfo` values without
projecting them to names first, and `StringAssertionExtensions` adds the string checks that come up often
enough to be worth naming. All of it is opt-in — importing the namespace makes the extensions visible, and the
equivalency step is registered explicitly where it is wanted.

See the [Ploch.TestingSupport.FluentAssertions library guide](../../docs/libraries/testing-support-fluent-assertions.md)
for installation instructions and worked examples.
