---
uid: Ploch.TestingSupport
summary: *content
---

### Overview

`Ploch.TestingSupport` is the root namespace of the testing-support stack — the utilities that close the most
common gaps in day-to-day test authoring, so that each solution does not grow its own private copy of them.
Three concerns are covered: keeping bulky or awkward theory data in files beside the test project instead of
inlined in source (`Ploch.TestingSupport.TestData`), bridging Moq's `Verify` callback style with fluent
assertions (`Ploch.TestingSupport.Moq`), and controlling execution order within a class where sequence
genuinely matters (`Ploch.TestingSupport.TestOrdering`).

The namespace itself is a container: the types live in its children, and the xUnit v3 versions of these
utilities live under <xref:Ploch.TestingSupport.XUnit3>, which is the actively maintained line. Assertion
helpers are separate again, in <xref:Ploch.TestingSupport.FluentAssertions>. Everything here is intended for
test projects only and is not part of any production package's dependency graph.

See the [Ploch.TestingSupport library guide](../../docs/libraries/testing-support.md) for installation
instructions and worked examples.
