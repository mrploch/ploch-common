# Ploch.TestingSupport.XUnit3

> xUnit v3 test helpers: file-based data attributes, Moq utilities, platform-conditional test skipping, and test ordering.

## Overview

`Ploch.TestingSupport.XUnit3` is the primary home for reusable testing infrastructure in the Ploch.Common suite. It is built specifically against xUnit v3 (`xunit.v3.extensibility.core`) and provides a richer, more consistent API than the older `Ploch.TestingSupport` package.

The library is organised into four areas:

- **File-based theory data** — `JsonFileDataAttribute`, `TextFileDataAttribute`, and `TextFileLinesDataAttribute` let you externalise large or table-driven test inputs into files, keeping test source files focused on assertions.
- **Platform-conditional tests** — `SupportedOSPlatformAttribute` dynamically skips a test at runtime when it is executed on an operating system that is not in the declared supported list. The attribute uses xUnit v3's dynamic-skip mechanism, so skipped tests are reported as skipped rather than failing.
- **Moq utilities** — `FluentVerifier` and `MockingExtensions` (mirroring those in `Ploch.TestingSupport`) make it easy to write rich assertions inside Moq callbacks.
- **Test ordering** — `AlphabeticalOrderer` and `TestPriorityAttribute` provide execution-order control within a test class.

The project targets the framework version configured by `TargetFrameworkVersion` in the solution's `Directory.Build.props` (currently net10.0 in the workspace build output).

## Installation

```shell
dotnet add package Ploch.TestingSupport.XUnit3
```

For a single reference that installs `xunit.v3`, `AutoFixture.Xunit3`, `FluentAssertions`, `Moq`, coverlet, and all associated analysers, install the companion meta-package instead:

```shell
dotnet add package Ploch.TestingSupport.XUnit3.Dependencies
```

## Key Types

| Type | Namespace | Description |
|---|---|---|
| `JsonFileDataAttribute` | `Ploch.TestingSupport.XUnit3.TestData` | `[DataAttribute]` that loads theory rows from a JSON file via Newtonsoft.Json. Supports an optional property path for nested arrays. Performs automatic type coercion to match method parameter types. |
| `TextFileDataAttribute` | `Ploch.TestingSupport.XUnit3.TestData` | Abstract base for file-based data attributes. Subclass and implement `ProcessFileData` to define how the file content maps to theory rows. |
| `TextFileLinesDataAttribute` | `Ploch.TestingSupport.XUnit3.TestData` | Splits a text file into lines; each line becomes one theory row. Optionally removes empty lines. |
| `SupportedOSPlatformAttribute` | `Ploch.TestingSupport.XUnit3` | `[BeforeAfterTestAttribute]` that dynamically skips the test when the current OS is not in the declared `SupportedOS[]` list. |
| `SupportedOS` | `Ploch.TestingSupport.XUnit3` | Enum: `FreeBSD`, `Linux`, `macOS`, `Windows`. |
| `FluentVerifier` | `Ploch.TestingSupport.XUnit3.Moq` | Static helpers: `VerifyFluentAssertion(Action)` and `VerifyFluentAssertionAsync(Func<Task>)` — evaluate FluentAssertions inside a Moq verification callback. |
| `MockingExtensions` | `Ploch.TestingSupport.XUnit3.Moq` | Extension method `Mock<T>()` on a mocked service instance; retrieves the underlying `Mock<T>`. |
| `AlphabeticalOrderer` | `Ploch.TestingSupport.XUnit3.TestOrdering` | `ITestCaseOrderer` that sorts tests by method name, case-insensitive. |
| `TestPriorityAttribute` | `Ploch.TestingSupport.XUnit3.TestOrdering` | Sealed attribute that declares a numeric execution priority for a test method. |

## Usage Examples

### JSON file data

Create a JSON file with an outer array of rows, where each row is an array of parameter values:

```json
{
  "roundTrips": [
    ["hello world", "hello world"],
    ["  trimmed  ", "trimmed"]
  ]
}
```

```csharp
[Theory]
[JsonFileData("testcases.json", "roundTrips")]
public void Trim_should_remove_whitespace(string input, string expected)
{
    input.Trim().Should().Be(expected);
}
```

The attribute uses Newtonsoft.Json for deserialisation and automatically coerces JSON values to match each method parameter's declared type, including complex object parameters via `JObject.ToObject<T>()`.

> **Migration note:** The `JsonFileDataAttribute` in the base `Ploch.TestingSupport` library uses `System.Text.Json`, while the `Ploch.TestingSupport.XUnit3` version uses `Newtonsoft.Json`. If you are migrating from `Ploch.TestingSupport` to `Ploch.TestingSupport.XUnit3`, be aware that JSON deserialisation behaviour may differ between the two libraries (e.g. case sensitivity defaults, handling of comments and trailing commas, custom converter compatibility).

### Text file theory data

```csharp
// keywords.txt contents:
// public
// private
// protected

[Theory]
[TextFileLinesData("keywords.txt", removeEmptyEntries: true)]
public void Keyword_should_be_recognised(string keyword)
{
    CSharpKeywords.IsKeyword(keyword).Should().BeTrue();
}
```

### Platform-conditional skipping

```csharp
[Fact]
[SupportedOSPlatform(SupportedOS.Linux, SupportedOS.macOS)]
public void FilePermissions_should_be_unix_style()
{
    // This test is dynamically skipped on Windows and FreeBSD.
    // On unsupported platforms, xUnit reports it as skipped rather than failing.
}
```

When the test runs on an unsupported OS, xUnit receives the `$XunitDynamicSkip$` message and marks the test as skipped with a description of the current OS.

### FluentVerifier inside Moq callbacks

```csharp
repositoryMock.Verify(r => r.SaveAsync(It.Is<Order>(o =>
    FluentVerifier.VerifyFluentAssertion(() =>
    {
        o.Status.Should().Be(OrderStatus.Confirmed);
        o.Items.Should().HaveCountGreaterThan(0);
    }))));
```

For async assertions:

```csharp
processorMock.Verify(p => p.ProcessAsync(It.Is<Payload>(pl =>
    FluentVerifier.VerifyFluentAssertionAsync(async () =>
    {
        var result = await pl.ValidateAsync();
        result.Should().BeTrue();
    }).GetAwaiter().GetResult())));
```

### Alphabetical test ordering

```csharp
[TestCaseOrderer(
    ordererTypeName: "Ploch.TestingSupport.XUnit3.TestOrdering.AlphabeticalOrderer",
    ordererAssemblyName: "Ploch.TestingSupport.XUnit3")]
public class MyOrderedTests
{
    [Fact] public void C_Third() { }
    [Fact] public void A_First() { }   // executes first
    [Fact] public void B_Second() { }
}
```

## Related Libraries

- [Ploch.TestingSupport.XUnit3.AutoMoq](testing-support-xunit3-automoq.md) — AutoFixture + AutoMoq integration that builds on this library
- [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md) — Meta-package that installs all required xUnit v3 testing dependencies
- [Ploch.TestingSupport](testing-support.md) — The older netstandard2.0 variant (partially migrated)
- [Ploch.TestingSupport.FluentAssertions](testing-support-fluent-assertions.md) — Custom FluentAssertions extensions
