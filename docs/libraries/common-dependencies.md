# Ploch.Common.Dependencies

> Shared meta-package that pulls in common JetBrains annotation dependencies.

## Overview

`Ploch.Common.Dependencies` is a thin meta-package (no source code) that centralises a shared set of package references for use across the Ploch.Common library suite. At present its sole dependency is `JetBrains.Annotations`, which provides attributes such as `[NotNull]`, `[CanBeNull]`, `[Pure]`, and ReSharper / Rider code-analysis hints.

Referencing this package instead of individual annotation packages ensures every library in the suite gets a consistent version of shared infrastructure references without each project having to declare them individually.

The package targets `net10.0`.

## Installation

```
dotnet add package Ploch.Common.Dependencies
```

## Contents

| Dependency | Purpose |
|------------|---------|
| `JetBrains.Annotations` | Compile-time attributes for nullability, purity, and code-flow hints recognised by ReSharper and JetBrains Rider |

## Usage

Add the package reference to projects that need JetBrains annotation attributes:

```xml
<PackageReference Include="Ploch.Common.Dependencies" />
```

No additional code is required. The transitive `JetBrains.Annotations` package is included automatically.

### Common JetBrains annotation attributes

```csharp
using JetBrains.Annotations;

public class ExampleService
{
    [Pure]
    public string FormatName([NotNull] string firstName, [NotNull] string lastName)
        => $"{firstName} {lastName}";

    [CanBeNull]
    public string? FindById(int id) => _store.TryGetValue(id, out var value) ? value : null;
}
```

## Related Libraries

- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — Core DI abstractions library in the same suite
