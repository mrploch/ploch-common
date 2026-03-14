# Ploch.TestingSupport.FluentAssertions.IOAbstractions

> FluentAssertions extensions for asserting on collections of `System.IO.Abstractions` file system objects.

## Overview

`Ploch.TestingSupport.FluentAssertions.IOAbstractions` adds FluentAssertions-style assertion methods for working with the `TestableIO.System.IO.Abstractions` abstraction layer. When tests verify directory listings, file sets, or other file-system collections returned through `IFileSystem`, this library lets you write readable, failure-reporting assertions rather than manual LINQ comparisons.

The central type is `FileSystemInfoAssertions<TItems>`, which extends `GenericCollectionAssertions<TItems>` and adds name-based equivalence checks that handle path-separator normalisation automatically. The three `Should()` extension methods on `IEnumerable<IFileSystemInfo>`, `IEnumerable<IDirectoryInfo>`, and `IEnumerable<IFileInfo>` are the entry points.

The library targets `netstandard2.0` and has two dependencies: `FluentAssertions` and `TestableIO.System.IO.Abstractions`.

## Installation

```shell
dotnet add package Ploch.TestingSupport.FluentAssertions.IOAbstractions
```

## Key Types

| Type | Namespace | Description |
|---|---|---|
| `FileSystemInfoAssertions<TItems>` | `TestingSupport.FluentAssertions.IOAbstractions` | Extends `GenericCollectionAssertions<TItems>` for `IFileSystemInfo` collections. Provides `HaveNamesEquivalentTo`, `HaveNamesEquivalentToIgnoringCase`. |
| `FileSystemInfoEnumerableExtensions` | `TestingSupport.FluentAssertions.IOAbstractions` | Adds `Should()` to `IEnumerable<IFileSystemInfo>`, `IEnumerable<IDirectoryInfo>`, and `IEnumerable<IFileInfo>`. |

## Usage Examples

### Asserting directory contents by name (case-insensitive)

```csharp
IFileSystem fileSystem = new FileSystem(); // or a mock
var directory = fileSystem.DirectoryInfo.New("/my/project");

directory.GetDirectories()
         .Should()
         .HaveNamesEquivalentToIgnoringCase("src", "tests", "docs");
```

Trailing path separators are stripped automatically before comparison, so `"src/"` and `"src"` are treated as equal.

### Asserting with a custom equality comparer

```csharp
directory.GetFiles()
         .Should()
         .HaveNamesEquivalentTo(
             new[] { "Program.cs", "Startup.cs" },
             StringComparer.Ordinal,
             because: "the file names must match exactly");
```

### Mixed `IFileSystemInfo` collection

```csharp
IEnumerable<IFileSystemInfo> entries = directory.GetFileSystemInfos();

entries.Should()
       .HaveNamesEquivalentToIgnoringCase("README.md", "src", "tests");
```

### Using with a mocked file system

The assertions integrate directly with `MockFileSystem` from `System.IO.Abstractions.TestingHelpers`, enabling fully in-memory file system tests:

```csharp
var mockFs = new MockFileSystem(new Dictionary<string, MockFileData>
{
    { "/project/src/Program.cs", new MockFileData("// code") },
    { "/project/tests/MyTests.cs", new MockFileData("// tests") }
});

mockFs.DirectoryInfo.New("/project")
      .GetDirectories()
      .Should()
      .HaveNamesEquivalentToIgnoringCase("src", "tests");
```

## Related Libraries

- [Ploch.TestingSupport.FluentAssertions](testing-support-fluent-assertions.md) — Custom FluentAssertions extensions for strings and reflection
- [Ploch.TestingSupport.XUnit3](testing-support-xunit3.md) — xUnit v3 helpers for use alongside these assertions
