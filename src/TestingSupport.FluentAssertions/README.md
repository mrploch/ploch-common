# Ploch.TestingSupport.FluentAssertions

## Overview

Library provides extension methods and equivalency helpers for the [FluentAssertions](https://fluentassertions.com/)
library.

## Features

### `NullEmptyCollectionEquivalencyStep`

A custom [`IEquivalencyStep`](https://fluentassertions.com/objectgraphs/#equivalency) that treats a `null` collection as
equivalent to an empty collection (and vice versa) during `BeEquivalentTo` comparisons.

**Why this is useful:** Object graphs often have collection properties that one side leaves `null` while the other
initialises to an empty collection. A typical example is EF Core: a navigation collection that was not eager-loaded via
`Include()` remains `null`, whereas an in-memory test entity usually initialises it to `new List<T>()`. Without this
step, FluentAssertions reports a false negative.

The step only intercedes when one side is `null` and the other is an empty `IEnumerable` (strings are explicitly
excluded — a `null` string is **not** treated as equivalent to `""`). All other cases pass through to the rest of the
equivalency pipeline, preserving configured options such as `DateTimeOffset` tolerance and cyclic-reference handling.

Usage:

```csharp
using Ploch.TestingSupport.FluentAssertions;

actual.Should().BeEquivalentTo(expected,
    options => options.Using(new NullEmptyCollectionEquivalencyStep()));
```

### `StringAssertionExtensions.ContainAllEquivalentOf`

Asserts that a string contains all of the specified values, ignoring case.

```csharp
"Hello World".Should().ContainAllEquivalentOf("hello", "WORLD");
```

### `PropertyInfoCollectionExtensions.ContainProperty` / `ContainProperties`

Assert that a `PropertyInfo` collection contains a property (or properties) with matching names and that those
properties can be read from a given source object.

```csharp
typeof(MyClass).GetProperties().Should().ContainProperties(new[] { "Id", "Name" }, instance);
```
