# Ploch.TestingSupport

> Base testing utilities for xUnit v3 theories: file-based data attributes, Moq helpers, and test-ordering support.

## Overview

`Ploch.TestingSupport` provides a small, focused set of helpers that cover the most common gaps in day-to-day xUnit v3 test authoring. It sits at the bottom of the testing-support stack and has no dependency on any particular xUnit test runner configuration, making it suitable as a shared utility library that more specialised packages build upon.

The three main areas it covers are:

- **File-based theory data** — `JsonFileDataAttribute`, `TextFileDataAttribute`, and `TextFileLinesDataAttribute` allow you to keep large or complex test input in files next to your test project rather than inlining it in source code.
- **Moq integration helpers** — `FluentVerifier` bridges the Moq `Verify` callback pattern with FluentAssertions, and `MockingExtensions` provides a convenience extension to retrieve the underlying `Mock<T>` from an already-mocked instance.
- **Test ordering** — `AlphabeticalOrderer` and `TestPriorityAttribute` give you control over test execution order within a class when sequence matters.

> **Note:** The README in the source directory records that this project is temporarily removed from the active solution while a migration to xUnit v3 is completed. For current xUnit v3 support, see [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md), which contains the actively maintained version of these utilities.

The library targets `netstandard2.0`.

## Installation

```shell
dotnet add package Ploch.TestingSupport
```

## Key Types

| Type | Namespace | Description |
|---|---|---|
| `JsonFileDataAttribute` | `Ploch.TestingSupport.TestData` | `[DataAttribute]` that loads theory data from a JSON file using `System.Text.Json`. Supports an optional property path to extract a named array within the document. Note: the [XUnit3 variant](testing-support-xunit3.md) uses `Newtonsoft.Json` instead. |
| `TextFileDataAttribute` | `Ploch.TestingSupport.TestData` | Abstract base class for attributes that load theory data from text files. Subclass and implement `ProcessFileData` for custom formats. |
| `TextFileLinesDataAttribute` | `Ploch.TestingSupport.TestData` | Concrete `TextFileDataAttribute` that treats each line of a text file as a separate theory row. Optionally skips empty lines. |
| `FluentVerifier` | `Ploch.TestingSupport.Moq` | Static helper with `VerifyFluentAssertion(Action)` and `VerifyFluentAssertion(Func<Task>)` — wraps FluentAssertions inside a Moq verification callback. |
| `MockingExtensions` | `Ploch.TestingSupport.Moq` | Extension method `Mock<T>()` on any mocked service instance; casts it back to the underlying `Mock<T>`. |
| `AlphabeticalOrderer` | `Ploch.TestingSupport.TestOrdering` | `ITestCaseOrderer` that sorts tests alphabetically by method name (case-insensitive). |
| `TestPriorityAttribute` | `Ploch.TestingSupport.TestOrdering` | Attribute applied to a test method to declare its numeric execution priority. |

## Usage Examples

### Loading theory data from a JSON file

The JSON file must contain either a root array or a named property containing an array. Each element of that array must itself be an array whose length matches the number of parameters on the test method.

```json
{
  "cases": [
    ["hello", "HELLO"],
    ["world", "WORLD"]
  ]
}
```

```csharp
[Theory]
[JsonFileData("testdata.json", "cases")]
public void ToUpper_should_return_uppercase_string(string input, string expected)
{
    input.ToUpper().Should().Be(expected);
}
```

To use the root array directly, omit the property name:

```csharp
[Theory]
[JsonFileData("testdata.json")]
public void Method_should_handle_data(string input, string expected) { }
```

### Loading each line of a text file as a separate theory row

```csharp
// inputs.txt:
// apple
// banana
// cherry

[Theory]
[TextFileLinesData("inputs.txt", removeEmptyEntries: true)]
public void Contains_should_find_fruit(string fruit)
{
    var list = new[] { "apple", "banana", "cherry" };
    list.Should().Contain(fruit);
}
```

### Using FluentVerifier inside a Moq callback

When you need to use FluentAssertions inside a `Moq.Verify` callback (which expects a `bool` return), wrap the assertion with `FluentVerifier.VerifyFluentAssertion`:

```csharp
mockService.Verify(s => s.Process(It.Is<Request>(r =>
    FluentVerifier.VerifyFluentAssertion(() =>
    {
        r.Name.Should().Be("expected");
        r.Value.Should().BeGreaterThan(0);
    }))));
```

### Casting an injected mock back to Mock<T>

```csharp
// Given IMyService injected as a Moq mock:
var mock = myServiceInstance.Mock();
mock.Verify(s => s.DoWork(), Times.Once);
```

### Ordering tests alphabetically

Apply the orderer to the test class with the standard xUnit attribute:

```csharp
[TestCaseOrderer(
    ordererTypeName: "Ploch.TestingSupport.TestOrdering.AlphabeticalOrderer",
    ordererAssemblyName: "Ploch.TestingSupport")]
public class MyOrderedTests
{
    [Fact] public void B_Test() { }
    [Fact] public void A_Test() { }  // runs first
}
```

## Related Libraries

- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — The actively maintained xUnit v3 variant of these utilities
- [Ploch.TestingSupport.XUnit3.AutoMoq](testing-support-xunit3-automoq.md) — AutoFixture + Moq integration built on top of the XUnit3 support
- [Ploch.TestingSupport.FluentAssertions](testing-support-fluent-assertions.md) — Custom FluentAssertions extensions
- [Ploch.TestingSupport.XUnit3.Dependencies](testing-support-xunit3-dependencies.md) — Meta-package bundling all xUnit v3 test dependencies in a single reference
