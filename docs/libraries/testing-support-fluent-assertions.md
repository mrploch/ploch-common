# Ploch.TestingSupport.FluentAssertions

> Custom FluentAssertions extensions for string and reflection assertions.

## Overview

`Ploch.TestingSupport.FluentAssertions` extends the [FluentAssertions](https://fluentassertions.com/) library with assertion methods that are not part of the standard FluentAssertions distribution. It currently provides two areas of functionality:

- **String assertions** — `ContainAllEquivalentOf` asserts that a string contains every one of a supplied list of substrings, using a case-insensitive comparison. This fills a gap in the standard `StringAssertions` API, which only exposes `ContainEquivalentOf` for a single expected substring at a time.
- **Reflection / property assertions** — `PropertyInfoCollectionAssertions` (exposed via `PropertyInfoCollectionExtensions.Should()`) adds `ContainProperty` and `ContainProperties` assertions on `IEnumerable<PropertyInfo>` collections. These are useful when verifying that a type exposes specific properties, for example in tests for mapping configurations or serialisation contracts.

The library targets `netstandard2.0` and depends on `FluentAssertions`, `Ploch.Common`, and `TestableIO.System.IO.Abstractions`.

## Installation

```shell
dotnet add package Ploch.TestingSupport.FluentAssertions
```

## Key Types

| Type | Description |
|---|---|
| `StringAssertionExtensions` | Static class. Adds `ContainAllEquivalentOf(params string?[] values)` and its `IEnumerable<string?>` overload to `StringAssertions`. |
| `PropertyInfoCollectionExtensions` | Static class. Adds `Should()` to `IEnumerable<PropertyInfo>`, returning a `PropertyInfoCollectionAssertions`. |
| `PropertyInfoCollectionAssertions` | Extends `GenericCollectionAssertions<PropertyInfo>` with `ContainProperty(string, object)` and `ContainProperties(string[], object)`. |

## Usage Examples

### Asserting a string contains multiple substrings (case-insensitive)

```csharp
var message = "Order confirmed for customer John, total: 99.99";

message.Should().ContainAllEquivalentOf("ORDER", "customer", "John", "99.99");
```

Failure output identifies which substrings were missing:

```
Expected string "Order confirmed for customer John, total: 99.99"
to contain the strings ignoring case: ["shipping", "discount"]
but ["shipping", "discount"] was not found.
```

You can also pass an enumerable and supply a `because` clause:

```csharp
var requiredTokens = new[] { "status", "active" };

responseBody.Should().ContainAllEquivalentOf(
    requiredTokens,
    because: "the API response must include status information");
```

### Asserting property presence on a type

`ContainProperty` and `ContainProperties` work on the result of reflecting a type's `GetProperties()` call. They look up the actual `PropertyInfo` on the supplied `sourceObj`'s type to ensure that the property name resolves correctly.

```csharp
var dto = new UserDto { Name = "Alice", Email = "alice@example.com" };
var properties = typeof(UserDto).GetProperties();

properties.Should().ContainProperty("Name", dto);
properties.Should().ContainProperties(new[] { "Name", "Email" }, dto);
```

## Related Libraries

- [Ploch.TestingSupport.FluentAssertions.IOAbstractions](testing-support-fluent-assertions-io.md) — Extends FluentAssertions for `System.IO.Abstractions` file system collections
- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — xUnit v3 integration that pairs naturally with these assertion extensions
