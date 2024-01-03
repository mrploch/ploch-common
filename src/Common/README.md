# Project Ploch.Common

## Overview

This library contains a set of extension methods and utilities for core .NET types.

This includes:

- enumerables: `IEnumerable<T>`, `ICollection<T>`, `IQueryable<T>` and others,
- expression trees
- `System.String`
- enums
- environment utilities

It's delivered as a .NET Standard 2.0 package allowing usage in both the .NET Core and the legacy
.NET Framework.

## Usage

The library is available as a [NuGet package](https://www.nuget.org/packages/Ploch.Common/).

```powershell
dotnet add package Ploch.Common
```

The API reference is available [here](https://github.ploch.dev/ploch-common/).

### Examples

The best place to look for examples is
the [library unit tests project](https://github.com/mrploch/ploch-common/tree/master/src/Common.Tests).

### Enumerables

A few examples of the enumerable extensions:

```csharp
  var items = new[] { 1, 2, 3};
  var result = items.JoinWithFinalSeparator(", ", " and ");
  
    // result will be "1, 2 and 3"
  
  var items = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
  var result = items.TakeRandom(3);
  // result will be 3 random items from the collection
  var result = items.Shuffle();
  // result will be the collection with items in random order
  
  var result = 10.ValueIn(items);
  // result will be true
```

More in 