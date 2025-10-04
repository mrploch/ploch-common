# Ploch.Common Documentation

## Overview

**Ploch.Common** is a collection of utility libraries that I was creating for my daily development needs.

Some time ago I decided to publish it. It still needs a lot of documentation because barely any exists, but thecode is
well-tested.

Hopefully, you'll find something useful there :)

## Libraries

### Ploch.Common

[Ploch.Common](../src/Common/README.md) contains extensions methods and utility classes for various core classes of
.NET.

One of the goals of the design was to no introduce any exteranl runtime dependencies. Only pure .NET is used.

#### Installation

```powershell
dotnet add package Ploch.Common
```

The best way of getting to know the library is looking at the tests.

`Ploch.Common` comes with an extensive number of tests and pretty good coverage ratio.
Due to a nature of this project, tests are a type of tutorial - a big catalogue of usage examples.

Even though the tests seem like a best approach to learn capabilities of this package, we decided to provide a rundown
over the main namespaces.

##### Ploch.Common types

The root namespace `Ploch.Common` contains several utility types and extension methods designed to simplify common .NET
development tasks. Below is a rundown of the main types, usage examples, and links to relevant tests in GitHub.

### 1. `StringExtensions`

Provides extension methods for string manipulation, such as `IsNullOrEmpty`, `IsNullOrWhiteSpace`, and more.

You can find more usage examples in the [StringExtensions tests](../tests/Ploch.Common.Tests/StringExtensionsTests.cs).

### 2. `EnumerableExtensions`

Provides extension methods for working with `IEnumerable<T>`, such as `ForEach`, `IsNullOrEmpty`, and more.
**Usage Example:**
**Usage Example:**
You can find more usage examples in
the [EnumerableExtensions tests](../tests/Ploch.Common.Tests/EnumerableExtensionsTests.cs).

### 3. `DateTimeExtensions`

Provides extension methods for `DateTime` and `DateTimeOffset`, such as date calculations, formatting, and range checks.

**Usage Example:**