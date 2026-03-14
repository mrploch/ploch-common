# Ploch.Common

> Core .NET utility library providing extension methods, guard clauses, reflection helpers, pattern matching, and randomisation utilities for everyday development tasks.

## Overview

`Ploch.Common` is the foundational library in the Ploch.Common suite. It targets both `netstandard2.0` and `net8.0`, making it suitable for the widest possible range of .NET projects from legacy frameworks to the latest runtimes.

The library is organised into functional namespaces rather than a single flat API surface. Each namespace addresses a specific concern: string manipulation, collection operations, argument checking, reflection, path handling, pattern matching, and random value generation. These utilities reduce boilerplate and consolidate common patterns across codebases.

Callers should be aware that on .NET 7 and later, guard clause methods use `CallerArgumentExpressionAttribute` to capture parameter names automatically, removing the need to pass string literals. On .NET Standard 2.0 and .NET 6, parameter names must be supplied explicitly.

## Installation

```bash
dotnet add package Ploch.Common
```

## Key Types

### Argument Checking (`Ploch.Common.ArgumentChecking`)

| Type | Description |
|------|-------------|
| `Guard` | Static partial class providing extension-method guard clauses for validating method arguments and enforcing preconditions. |

### Strings (`Ploch.Common` namespace)

| Type | Description |
|------|-------------|
| `StringExtensions` | Extension methods for `string`: null/empty checks, base64 encoding, case-insensitive comparison, prefix replacement, numeric parsing, and substring containment. |
| `StringParsingExtensions` | Nullable-returning parse helpers: `ParseToBool`, `ParseToInt32`, `ParseToInt64`. |
| `StringBuilderExtensions` | Extension methods for `StringBuilder`. |
| `Strings` | Static constants and factory methods for common string fragments: `Space`, `Dash`, `Underscore`, `Dot`, plus `Spaces(n)`, `Dashes(n)`, etc. |
| `Chars` | Character constants mirroring `Strings`. |

### Collections (`Ploch.Common.Collections`)

| Type | Description |
|------|-------------|
| `EnumerableExtensions` | Extensions for `IEnumerable<T>`: `ForEach`, `None`, `Join`, `JoinWithFinalSeparator`, `Shuffle`, `TakeRandom`, `If` (conditional query), `IsEmpty`, `IsNullOrEmpty`, `Second`, `ExceptItems`, `ValueIn`, `AreIntegersSequentialInOrder`. |
| `CollectionExtensions` | Extensions for `ICollection<T>`: `AddMany` with configurable `DuplicateHandling`, `Add` (key/value pair), `AddIfNotNull`. |
| `QueryableExtensions` | `If` (conditional query) for `IQueryable<T>`, preserving composability with LINQ to Entities. |
| `ArrayExtensions` | Array-specific extension methods. |
| `DictionaryExtensions` | Dictionary-specific extension methods. |
| `DuplicateHandling` | Enum controlling behaviour when adding duplicates: `Throw`, `Ignore`, `Overwrite`. |

### Value Membership (`Ploch.Common`)

| Type | Description |
|------|-------------|
| `IsInExtensions` | `In` and `NotIn` extension methods for checking whether a value is a member of a set, with optional custom `IComparer<T>`. |
| `IfNullHelpers` | `OrIfNull` and `OrIfNullOrEmpty` for concise null-coalescing. Note: `OrIfNullOrEmpty` currently delegates to `OrIfNull` and only checks for null, not emptiness. |

### Reflection (`Ploch.Common.Reflection`)

| Type | Description |
|------|-------------|
| `TypeExtensions` | `IsConcreteImplementation`, `IsImplementing`, `IsEnumerable`, `IsNullable`, `IsSimpleType`, `GetReadableTypeName`. |
| `TypeLoader` | Fluent type-discovery engine: load types from assemblies matching base-type constraints, assembly name globs, and type name globs. |
| `TypeLoaderConfigurator` / `ITypeLoaderConfigurator` | Builder interface for configuring `TypeLoader`. |
| `ObjectReflectionExtensions` | `GetFieldValue`, `GetFieldValues`, `GetMemberValues` — access field and property values including non-public and static members. |
| `PropertyHelpers` | Utilities for working with `PropertyInfo`. |
| `ByValueObjectComparator` / `ByValueObjectComparer` | Reflection-based value-equality comparison for objects. |
| `ObjectHashCodeBuilder` | Builds `GetHashCode` implementations from object properties via reflection. |
| `AssemblyExtensions` / `AssemblyListBuilder` | Helpers for working with `Assembly` instances. |

### LINQ and Expressions (`Ploch.Common.Linq`)

| Type | Description |
|------|-------------|
| `ExpressionExtensions` | `GetMemberName` — extract a member or method name string from a strongly-typed lambda expression. `GetProperty` — retrieve an `IOwnedPropertyInfo` from a property selector expression. |
| `IOwnedPropertyInfo<TType, TProperty>` / `OwnedPropertyInfo<TType, TProperty>` | Combines `PropertyInfo` with the owning object instance. |

### IO and Paths (`Ploch.Common.IO`)

| Type | Description |
|------|-------------|
| `PathUtils` | `GetDirectoryName`, `ToSafeFileName`, `NormalizePathWithTrailingSeparator`, `NormalizePathWithoutTrailingSeparator`, `GetRelativePath`, `WithExtension`, `GetFullPathWithoutExtension`. Also exposed as extension method `WithExtension` on `string`. |
| `StreamExtensions` | Extension methods for `Stream`. |
| `CommandLineParser` / `CommandLineInfo` | Basic command-line argument parsing utilities. |

### Pattern Matching (`Ploch.Common.Matchers`)

| Type | Description |
|------|-------------|
| `IMatcher<T>` | Single-method interface `bool IsMatch(T? value)`. |
| `IStringMatcher` | Specialisation of `IMatcher<string>`. |
| `GlobEvaluator` | Evaluates strings against include/exclude glob patterns using `Microsoft.Extensions.FileSystemGlobbing.Matcher`. Configurable null/empty match results and string comparison. |
| `RegexListEvaluator` | Evaluates strings against a list of regular expressions. |
| `PropertyMatcher` | Matches objects by reflected property values. |
| `GlobMatcherExtensions` | Extension helpers for configuring `Matcher` with include/exclude patterns. |

### Randomisation (`Ploch.Common.Randomizers`)

| Type | Description |
|------|-------------|
| `IRandomizer` | Base interface: `GetRandomValue(min, max)` and `GetRandomValue()`. |
| `IRandomizer<TValue>` | Generic variant returning typed values. |
| `IRangedRandomizer<TValue>` | Adds typed min/max overload. |
| `Randomizer` | Static factory: `GetRandomizer<TValue>()` and `GetRandomizer(Type)`. Supports `string`, `int`, `DateTime`, `DateTimeOffset`, `bool`. |
| `StringRandomizer`, `IntRandomizer`, `DateTimeRandomizer`, `DateTimeOffsetRandomizer`, `BooleanRandomizer` | Concrete randomiser implementations. |
| `ThreadSafeRandom` | Thread-safe `Random` wrapper. |

### Enumerations

| Type | Description |
|------|-------------|
| `EnumHelper` | `GetEnumEntries<TEnum>()` and `GetFlags<TEnum>()` for iterating all values or active flags of an enum. |
| `EnumerationConverter` | Extension methods for converting strings and integers to enum values: `ParseToEnum<TEnum>()` (throws on failure), `SafeParseToEnum<TEnum>()` (returns `null` on failure), and integer overloads. |

### Type Conversion (`Ploch.Common.TypeConversion`)

| Type | Description |
|------|-------------|
| `ITypeConverter` | Core converter interface with `Order`, `CanHandle(value, targetType)`, `CanHandleSourceType`, `CanHandleTargetType`, and `ConvertValue`. Lower `Order` values are tried first. |
| `ITypeConverter<TSourceType, TTargetType>` | Generic variant combining `ISourceTypeConverter<TSourceType>` and `ITargetTypeConverter<TTargetType>` with strongly-typed conversion methods. |
| `ISourceTypeConverter<TSourceType>` | Specialisation of `ITypeConverter` scoped to a single source type. |
| `ITargetTypeConverter<TTargetType>` | Specialisation of `ITypeConverter` scoped to a single target type, with `ConvertValueToTargetType`. |
| `TypeConverter` | Abstract base class implementing `ITypeConverter` with configurable null handling, supported source/target type collections, and ordering. |
| `SingleSourceTargetTypeConverter<TSource, TTarget>` | Abstract base for converters between a single source and target type pair, with support for derived-type handling and null source values. |
| `ObjectConverter` | Static class that builds typed objects from `ISourceObject` property bags using an ordered converter pipeline. |
| `ObjectPropertyAttribute` | Attribute for remapping a source property name to a differently-named target property during object conversion. |
| `EnumConverter` | Converts string values to nullable enum types using a field-name mapping cache (`EnumerationFieldValueCache`). |
| `TypeConversionException` | Exception thrown when a type conversion fails. |
| `TypeConverterHelper` | Static helper methods for type compatibility checks and combining type sets. |

### Date and Time

| Type | Description |
|------|-------------|
| `DateTimeExtensions` | `ToEpochSeconds`, `ToDateTime` (from epoch seconds, with automatic millis detection), nullable overloads. |
| `DateTimeFormats` | Common format string constants. |

### Cryptography (`Ploch.Common.Cryptography`)

| Type | Description |
|------|-------------|
| `Hashing` | `ToHashString(HashAlgorithm)` and `ToMD5HashString()` extension methods on `Stream`. |

### Other Utilities

| Type | Description |
|------|-------------|
| `StopwatchUtil` | Convenience methods for timing code execution. |
| `ComparisonUtils` | Generic comparison helpers. |
| `ContentSizes` | Constants for common content size thresholds. |
| `ObjectCloningHelpers` | Deep-clone utilities. |
| `OperatingSystemExtensions` | Extensions on `OperatingSystem`. |
| `EnvironmentUtilities` / `EnvironmentVariables` | Helpers for reading environment state. |
| `AssemblyInformation` / `AssemblyInformationProvider` | Reads version and metadata from assemblies. |

## Usage Examples

### Guard Clauses

```csharp
using Ploch.Common.ArgumentChecking;

public void Process(string name, IEnumerable<int> items, MyEnum status)
{
    // Throws ArgumentNullException if null; ArgumentException if empty
    name.NotNullOrEmpty();

    // Throws ArgumentNullException if null
    var safeItems = items.NotNull();

    // Throws ArgumentOutOfRangeException if value is not defined in the enum
    status.NotOutOfRange();

    // Throws ArgumentOutOfRangeException if value is not positive
    var count = items.Count();
    count.Positive();
}

// RequiredNotNull throws InvalidOperationException (used for state checks, not argument checks)
var config = _configuration.RequiredNotNull();
```

### String Extensions

```csharp
using Ploch.Common;

// Null/empty checks as extension methods
string? input = GetInput();
if (input.IsNullOrEmpty()) return;
if (input.IsNullOrWhiteSpace()) return;

// Returns null if empty, enabling null-conditional chaining
var normalised = input.NullIfEmpty();

// Case-insensitive comparison
if (input.EqualsIgnoreCase("admin")) { }

// Contains any of a set of substrings
if (input.ContainsAny("error", "fail", "exception")) { }
if (input.ContainsAny(StringComparison.OrdinalIgnoreCase, "Error", "FAIL")) { }

// Base64 encoding/decoding
var encoded = "hello world".ToBase64String();
var decoded = encoded.FromBase64String();

// Numeric parsing
int value = "42".ToInt32();
if ("99".TryConvertToInt32(out var n)) { }
```

### Collection Extensions

```csharp
using Ploch.Common.Collections;

var items = new List<string> { "a", "b", "c" };

// Side-effect iteration returning the same enumerable (fluent)
items.ForEach(Console.WriteLine);

// Inverse of Any
bool hasNoErrors = items.None(x => x.StartsWith("err"));

// Join with separator
string result = items.Join(", ");                     // "a, b, c"
string natural = items.JoinWithFinalSeparator(", ", " and ");  // "a, b and c"

// Conditional query composition (useful for optional filters)
var query = GetCars()
    .OrderBy(x => x.Created)
    .If(createdAfter.HasValue, x => x.Where(y => y.Created > createdAfter!.Value))
    .If(first.HasValue, x => x.Take(first!.Value));

// Shuffle and random sampling
var shuffled = items.Shuffle();
var sample = items.TakeRandom(2);

// Null/empty check
if (items.IsNullOrEmpty()) { }

// Collection membership with AddMany
var set = new HashSet<string>();
set.AddMany(new[] { "x", "y" }, DuplicateHandling.Ignore);
```

### Value Membership

```csharp
using Ploch.Common;

// In / NotIn - readable membership check
var day = DayOfWeek.Saturday;
if (day.In(DayOfWeek.Saturday, DayOfWeek.Sunday))
{
    Console.WriteLine("Weekend");
}

string status = "active";
if (status.NotIn("archived", "deleted")) { }
```

### Reflection and Type Discovery

```csharp
using Ploch.Common.Reflection;

// Type checks
typeof(MyService).IsConcreteImplementation<IMyService>();  // true
typeof(string).IsSimpleType();                              // true
typeof(List<int>).IsEnumerable();                          // true
typeof(List<string>).GetReadableTypeName();                 // "List<String>"

// TypeLoader — discover all concrete IHandler implementations in two assemblies
var loader = TypeLoader.Configure(cfg => cfg
    .WithBaseType<IHandler>()
    .WithAssemblyGlob(m => m.AddInclude("MyApp.*")));

loader.LoadTypes(typeof(MyApp.Startup))
      .LoadTypes(typeof(MyPlugin.Entry));

foreach (var handlerType in loader.LoadedTypes)
{
    Console.WriteLine(handlerType.FullName);
}
```

### Path Utilities

```csharp
using Ploch.Common.IO;

// Normalise paths
string withSep = PathUtils.NormalizePathWithTrailingSeparator(@"C:\projects\myapp");
// "C:\projects\myapp\"

// Safe file name (replaces illegal characters with '_')
string safe = PathUtils.ToSafeFileName("Report: Q1/2024.xlsx");
// "Report_ Q1_2024.xlsx"

// File extension manipulation
string newPath = "/data/report.txt".WithExtension(".md");
// "/data/report.md"

string relPath = PathUtils.GetRelativePath(@"C:\base\", @"C:\base\sub\file.txt");
// "sub\file.txt"
```

### Pattern Matching

```csharp
using Ploch.Common.Matchers;

// Glob-based string matching with include/exclude patterns
var evaluator = new GlobEvaluator(
    includes: new[] { "**/*.cs" },
    excludes: new[] { "**/obj/**", "**/bin/**" });

evaluator.IsMatch("src/MyProject/Class.cs");   // true
evaluator.IsMatch("src/MyProject/obj/Temp.cs"); // false
```

### Randomisers

```csharp
using Ploch.Common.Randomizers;

var intRandomizer = Randomizer.GetRandomizer<int>();
int value = intRandomizer.GetRandomValue(1, 100);

var dateRandomizer = Randomizer.GetRandomizer<DateTime>();
DateTime date = dateRandomizer.GetRandomValue(DateTime.Today.AddYears(-1), DateTime.Today);
```

### Date/Time

```csharp
using Ploch.Common;

long epochSeconds = DateTime.UtcNow.ToEpochSeconds();
DateTime restored = epochSeconds.ToDateTime();

// Handles both epoch seconds and epoch milliseconds automatically
DateTime fromMillis = 1700000000000L.ToDateTime();
```

## Related Libraries

- [Ploch.Common.Net9](common-net9.md) — .NET 9/10-specific extensions, including `AppDomainTypesLoader` for application-domain-wide type discovery.
- [Ploch.Common.DataAnnotations](common-data-annotations.md) — Data annotation attributes for model validation.
- [Ploch.Common.ObjectBuilder](common-object-builder.md) — Object construction from loosely typed property bags.
- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — `ServicesBundle` pattern for modular DI registration.
- [Ploch.Common.Serialization](common-serialization.md) — Abstract serialisation interfaces and implementations.
