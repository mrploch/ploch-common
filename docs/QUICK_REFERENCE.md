# Ploch.Common Quick Reference Guide

A condensed reference for commonly used extensions and utilities in the Ploch.Common library.

## String Extensions

```csharp
using Ploch.Common;

// Null/Empty checks
str.IsNullOrEmpty()
str.IsNotNullOrEmpty()
str.IsNullOrWhiteSpace()
str.NullIfEmpty()           // Returns null if empty
str.NullIfWhiteSpace()      // Returns null if whitespace

// Conversions
str.ToInt32()
str.ToInt64()
str.TryConvertToInt32(out int result)
str.ToBase64String()
str.FromBase64String()

// Comparisons
str.EqualsIgnoreCase(other)
str.ContainsAny("str1", "str2", "str3")
str.ContainsAny(StringComparison.OrdinalIgnoreCase, "STR1")

// Manipulation
str.ReplaceStart(oldValue, newValue)
```

## Collection Extensions

```csharp
using Ploch.Common.Collections;

// Value checking
value.ValueIn(1, 2, 3)
value.ValueIn(collection)
value.ValueIn(comparer, val1, val2)

// Collection operations
collection.None(predicate)          // Opposite of Any
collection.IsEmpty()
collection.IsNullOrEmpty()
collection.Second()
collection.ExceptItems(item1, item2)

// Joining
collection.Join(", ")
collection.Join(", ", x => x.Name)
collection.JoinWithFinalSeparator(", ", " and ")

// Random operations
collection.Shuffle()
collection.TakeRandom(count)

// Conditional operations
collection.If(condition, query => query.Where(...))
collection.ForEach(item => DoSomething(item))

// Validation
numbers.AreIntegersSequentialInOrder()
```

## Guard Clauses

```csharp
using Ploch.Common.ArgumentChecking;

// Null validation (throws ArgumentNullException)
arg.NotNull(nameof(arg))
str.NotNullOrEmpty(nameof(str))
collection.NotNullOrEmpty(nameof(collection))

// Required validation (throws InvalidOperationException)
value.RequiredNotNull(nameof(value))
value.RequiredNotNull(nameof(value), "Custom message")
str.RequiredNotNullOrEmpty(nameof(str))
str.RequiredNotNullOrEmpty(nameof(str), "Custom message")

// Value validation
number.Positive(nameof(number))
enumValue.NotOutOfRange(nameof(enumValue))

// Boolean validation
condition.RequiredTrue("Error message")
condition.RequiredFalse("Error message")
```

## IsIn Extensions

```csharp
using Ploch.Common;

// IN operator (returns true if value is in collection)
value.In(1, 2, 3)
value.In(collection)
value.In(comparer, val1, val2)

// NOT IN operator
value.NotIn(1, 2, 3)
value.NotIn(collection)
value.NotIn(comparer, val1, val2)
```

## Type Extensions (Reflection)

```csharp
using Ploch.Common.Reflection;

// Type checking
type.IsImplementing(typeof(IInterface))
type.IsImplementing(typeof(IGeneric<>), concreteOnly: true)
type.IsConcreteImplementation(typeof(IInterface))
type.IsConcreteImplementation<IInterface>()

// Type properties
type.IsEnumerable()
type.IsNullable()
type.IsSimpleType()

// Type names
type.GetReadableTypeName()  // e.g., "List<int>", "Dictionary<string, User>"
```

## Path Utilities

```csharp
using Ploch.Common.IO;

// Directory operations
dirPath.GetDirectoryName()

// Path manipulation
PathUtils.ToSafeFileName(fileName)
PathUtils.NormalizePathWithTrailingSeparator(path)
PathUtils.NormalizePathWithoutTrailingSeparator(path)
PathUtils.GetRelativePath(fromPath, toPath)

// Extension operations
path.WithExtension(".txt")
path.WithExtension(".md", replaceExistingExtension: true)
PathUtils.GetFullPathWithoutExtension(path)
```

## Randomizers

```csharp
using Ploch.Common.Randomizers;

// Get randomizer for specific type
var randomizer = Randomizer.GetRandomizer<T>();
var value = randomizer.GetRandom();

// Supported types
Randomizer.GetRandomizer<string>()
Randomizer.GetRandomizer<int>()
Randomizer.GetRandomizer<DateTime>()
Randomizer.GetRandomizer<DateTimeOffset>()
Randomizer.GetRandomizer<bool>()

// With ranges
var rangedRandomizer = (IRangedRandomizer<int>)randomizer;
var value = rangedRandomizer.GetRandom(min, max);

// Using runtime type
var randomizer = Randomizer.GetRandomizer(typeof(int));
```

## Common Patterns

### Input Validation Pattern

```csharp
public void ProcessData(string input, List<Item> items, Status status)
{
    // Validate parameters
    input.NotNullOrEmpty(nameof(input));
    items.NotNullOrEmpty(nameof(items));
    status.NotOutOfRange(nameof(status));

    // Validate business rules
    if (!status.In(Status.Active, Status.Pending))
    {
        throw new ArgumentException("Invalid status");
    }
}
```

### Conditional Query Building

```csharp
var results = baseQuery
    .If(filter1.HasValue, q => q.Where(x => x.Field1 == filter1.Value))
    .If(filter2.HasValue, q => q.Where(x => x.Field2 == filter2.Value))
    .If(maxResults.HasValue, q => q.Take(maxResults.Value))
    .ToList();
```

### Safe String Operations

```csharp
string result = input
    .NullIfWhiteSpace()        // Convert whitespace to null
    ?.Trim()                    // Trim if not null
    .RequiredNotNull("input")  // Ensure not null
    .ToLowerInvariant();       // Convert to lowercase
```

### Collection Processing

```csharp
var summary = items
    .Where(x => x.IsActive)
    .OrderBy(x => x.Priority)
    .ForEach(x => x.Process())
    .Join(", ", x => x.Name);
```

### Type Discovery

```csharp
var types = TypeLoader.Configure(cfg => cfg
    .WithBaseType<IService>()
    .FromAssembliesContaining("MyApp")
    .ExcludeAssemblies("*.Tests")
).LoadTypes();

foreach (var type in types.Where(t => t.IsConcreteImplementation<IService>()))
{
    // Register or process type
}
```

## Cheat Sheet: Most Common Operations

| Operation | Code |
|-----------|------|
| Check string not empty | `str.IsNotNullOrEmpty()` |
| Validate parameter | `param.NotNull(nameof(param))` |
| Check value in set | `value.In(1, 2, 3)` |
| Join collection | `items.Join(", ")` |
| Safe file name | `PathUtils.ToSafeFileName(name)` |
| Type implements interface | `type.IsConcreteImplementation<IService>()` |
| Random value | `Randomizer.GetRandomizer<T>().GetRandom()` |
| Conditional filter | `query.If(condition, q => q.Where(...))` |
| None match predicate | `items.None(x => x.IsInvalid)` |
| Empty collection check | `collection.IsNullOrEmpty()` |

## Extension Method Namespaces

| Functionality | Namespace |
|--------------|-----------|
| String extensions | `Ploch.Common` |
| Collection extensions | `Ploch.Common.Collections` |
| Guard clauses | `Ploch.Common.ArgumentChecking` |
| IsIn extensions | `Ploch.Common` |
| Type extensions | `Ploch.Common.Reflection` |
| Path utilities | `Ploch.Common.IO` |
| Randomizers | `Ploch.Common.Randomizers` |
| DI extensions | `Ploch.Common.DependencyInjection` |
| Serialization | `Ploch.Common.Serialization` |
| Testing support | `Ploch.TestingSupport` |

## Tips and Best Practices

1. **Always use Guard clauses at method entry points** to fail fast with clear error messages
2. **Prefer `NotNull` for ArgumentNullException** and `RequiredNotNull` for InvalidOperationException
3. **Use `In`/`ValueIn`** instead of multiple OR conditions for cleaner code
4. **Chain `If` extensions** for building conditional LINQ queries
5. **Use `IsConcreteImplementation`** when discovering types to avoid abstract classes
6. **Leverage `Join` extensions** instead of string.Join with LINQ projections
7. **Use `PathUtils`** for cross-platform path operations
8. **Use Randomizers** in tests to generate varied test data

## Migration from Standard .NET

| Standard .NET | Ploch.Common |
|--------------|--------------|
| `string.IsNullOrEmpty(str)` | `str.IsNullOrEmpty()` |
| `if (x == 1 \|\| x == 2 \|\| x == 3)` | `if (x.In(1, 2, 3))` |
| `if (arg == null) throw new ArgumentNullException(...)` | `arg.NotNull(nameof(arg))` |
| `string.Join(", ", items.Select(x => x.Name))` | `items.Join(", ", x => x.Name)` |
| `!items.Any(predicate)` | `items.None(predicate)` |
| `typeof(IService).IsAssignableFrom(type)` | `type.IsImplementing(typeof(IService))` |

## Getting Help

- Full documentation: [README.md](https://github.com/mrploch/ploch-common#readme)
- Source code: [GitHub Repository](https://github.com/mrploch/ploch-common)
- Report issues: [GitHub Issues](https://github.com/mrploch/ploch-common/issues)
