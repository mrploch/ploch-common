---
uid: Ploch.TestingSupport.XUnit3
summary: *content
---

### Overview

`Ploch.TestingSupport.XUnit3` is the xUnit v3 line of the testing-support utilities, and the one to use for new
tests. Its centre of gravity is theory data. `JsonFileDataAttribute`, `TextFileDataAttribute` and
`TextFileLinesDataAttribute` (in the `TestData` child namespace) source `[Theory]` cases from files rather than
from `[InlineData]`, which keeps large, structured, or frequently edited fixtures out of the test source and
lets a JSON file be reviewed, diffed and reused on its own terms.

Alongside that sit the cross-platform and mocking helpers. `SupportedOSPlatformAttribute` with the
`SupportedOS` enum skips a test on operating systems where it cannot meaningfully run, so a platform-specific
test reports as skipped rather than failing the suite. `FluentVerifier` and `MockingExtensions` (in the `Moq`
child namespace) make Moq verification read fluently and let you recover the `Mock<T>` behind an already-mocked
instance. AutoFixture integration lives in the separate `Ploch.TestingSupport.XUnit3.AutoMoq` package, whose
`AutoMockData` attribute generates a subject under test with its dependencies already mocked.

See the [Ploch.TestingSupport.XUnit3 library guide](../../docs/libraries/testing-support-xunit3.md) for
installation instructions and worked examples.
