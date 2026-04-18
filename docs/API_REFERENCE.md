# Ploch.Common API Reference

Complete reference documentation for all public APIs in the Ploch.Common library suite.

## Table of Contents

- [Ploch.Common (Core)](#plochcommon-core)
  - [String Extensions](#string-extensions)
  - [Collection Extensions](#collection-extensions)
  - [Guard (Parameter Validation)](#guard-parameter-validation)
  - [IsIn Extensions](#isin-extensions)
- [Ploch.Common.Reflection](#plochcommonreflection)
  - [Type Extensions](#type-extensions)
  - [Type Loader](#type-loader)
- [Ploch.Common.IO](#plochcommonio)
  - [Path Utilities](#path-utilities)
- [Ploch.Common.Randomizers](#plochcommonrandomizers)
- [Ploch.Common.DependencyInjection](#plochcommondependencyinjection)
- [Ploch.Common.Serialization](#plochcommonserialization)

---

## Ploch.Common (Core)

### String Extensions

**Namespace**: `Ploch.Common`
**Class**: `StringExtensions`
**File**: [src/Common/StringExtensions.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/StringExtensions.cs)

#### Null/Empty Checking Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `IsNullOrEmpty()` | `bool` | Extension method version of `string.IsNullOrEmpty` |
| `IsNotNullOrEmpty()` | `bool` | Returns true if string is not null or empty |
| `IsNullOrWhiteSpace()` | `bool` | Extension method version of `string.IsNullOrWhiteSpace` |
| `NullIfEmpty()` | `string?` | Returns null if string is empty; otherwise returns the string |
| `NullIfWhiteSpace()` | `string?` | Returns null if string is null, empty, or whitespace |

#### Encoding Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ToBase64String()` | `string` | Encodes string to base64 using UTF8 |
| `ToBase64String(Encoding)` | `string` | Encodes string to base64 using specified encoding |
| `FromBase64String()` | `string` | Decodes base64 string using UTF8 |
| `FromBase64String(Encoding)` | `string` | Decodes base64 string using specified encoding |

#### Comparison Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `EqualsIgnoreCase(string?)` | `bool` | Compares strings ignoring case (OrdinalIgnoreCase) |
| `ContainsAny(params string[])` | `bool` | Checks if string contains any of the specified substrings |
| `ContainsAny(StringComparison, params string[])` | `bool` | Checks with specified comparison option |
| `ContainsAny(IEnumerable<string>)` | `bool` | Checks if string contains any substring from collection |

#### Conversion Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ToInt32()` | `int` | Converts string to 32-bit signed integer |
| `TryConvertToInt32(out int)` | `bool` | Tries to convert string to int32 |
| `TryConvertToInt32(IFormatProvider, out int)` | `bool` | Tries to convert with culture-specific formatting |
| `ToInt64()` | `long` | Converts string to 64-bit signed integer |
| `TryConvertToInt64(out long)` | `bool` | Tries to convert string to int64 |
| `TryConvertToInt64(IFormatProvider, out long)` | `bool` | Tries to convert with culture-specific formatting |

#### Manipulation Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ReplaceStart(string, string, StringComparison?)` | `string` | Replaces value at start of string if it matches |

---

### Collection Extensions

**Namespace**: `Ploch.Common.Collections`
**Class**: `EnumerableExtensions`
**File**: [src/Common/Collections/EnumerableExtensions.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/Collections/EnumerableExtensions.cs)

#### Value Checking Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `ValueIn<TValue>(params TValue[])` | `bool` | Checks if value is in the provided set |
| `ValueIn<TValue>(IEnumerable<TValue>)` | `bool` | Checks if value is in the collection |
| `ValueIn<TValue>(IEqualityComparer<TValue>?, params TValue[])` | `bool` | Checks with custom comparer |

#### Predicate Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `None<TSource>(Func<TSource, bool>)` | `bool` | Verifies that none of the items match the predicate |
| `IsEmpty<T>()` | `bool` | Determines if enumerable is empty |
| `IsNullOrEmpty<T>()` | `bool` | Checks if collection is null or empty |

#### Join Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Join<TValue>(string)` | `string` | Joins elements using separator, calling ToString on each |
| `Join<TValue, TResult>(string, Func<TValue, TResult>)` | `string` | Joins using separator with value selector |
| `JoinWithFinalSeparator<TValue>(string, string)` | `string` | Joins with different final separator |
| `JoinWithFinalSeparator<TValue, TResult>(string, string, Func<TValue, TResult>)` | `string` | Joins with selector and final separator |

#### Randomization Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Shuffle<TValue>()` | `IEnumerable<TValue>` | Randomly shuffles elements |
| `TakeRandom<TValue>(int)` | `IEnumerable<TValue>` | Takes random count of items |

#### Transformation Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `If<T>(bool, Func<IEnumerable<T>, IEnumerable<T>>)` | `IEnumerable<T>` | Conditionally applies transformation |
| `ForEach<T>(Action<T>)` | `IEnumerable<T>` | Performs action on each element |
| `ExceptItems<TItem>(params TItem[])` | `IEnumerable<TItem>` | Excludes specified items |

#### Sequence Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `Second<T>()` | `T` | Returns second element of sequence |
| `AreIntegersSequentialInOrder()` | `bool` | Checks if integers are sequential |

---

### Guard (Parameter Validation)

**Namespace**: `Ploch.Common.ArgumentChecking`
**Class**: `Guard`
**File**: [src/Common/ArgumentChecking/Guard.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/ArgumentChecking/Guard.cs)

#### Null Validation (throws ArgumentNullException)

| Method | Returns | Description |
|--------|---------|-------------|
| `NotNull<T>(string)` | `T` | Ensures reference type is not null |
| `NotNull<T>(string)` where T : struct | `T` | Ensures nullable value type has value |
| `NotNullOrEmpty(string)` | `string` | Ensures string is not null or empty |
| `NotNullOrEmpty<TEnumerable>(string)` | `TEnumerable` | Ensures collection is not null or empty |

#### Required Validation (throws InvalidOperationException)

| Method | Returns | Description |
|--------|---------|-------------|
| `RequiredNotNull<T>(string, string?)` | `T` | Ensures value is not null (InvalidOperationException) |
| `RequiredNotNullOrEmpty(string, string?)` | `string` | Ensures string is not null or empty |

#### Value Validation

| Method | Returns | Description |
|--------|---------|-------------|
| `Positive<TValue>(string)` | `TValue` | Ensures value is positive (> 0) |
| `NotOutOfRange<TEnum>(string)` | `TEnum` | Ensures enum value is defined |

#### Boolean Validation

| Method | Returns | Description |
|--------|---------|-------------|
| `RequiredTrue(string)` | `bool` | Ensures boolean is true |
| `RequiredFalse(string)` | `bool` | Ensures boolean is false |

---

### IsIn Extensions

**Namespace**: `Ploch.Common`
**Class**: `IsInExtensions`
**File**: [src/Common/IsInExtensions.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/IsInExtensions.cs)

#### In Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `In<TValue>(params TValue[])` | `bool` | Checks if value equals one of the provided values |
| `In<TValue>(IEnumerable<TValue>)` | `bool` | Checks if value is in enumerable |
| `In<TValue>(IComparer<TValue>, params TValue[])` | `bool` | Checks with custom comparer |
| `In<TValue>(IComparer<TValue>, IEnumerable<TValue>)` | `bool` | Checks enumerable with custom comparer |

#### NotIn Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `NotIn<TValue>(params TValue[])` | `bool` | Checks if value is not in set |
| `NotIn<TValue>(IEnumerable<TValue>)` | `bool` | Checks if value is not in enumerable |
| `NotIn<TValue>(IComparer<TValue>, params TValue[])` | `bool` | Checks with custom comparer |
| `NotIn<TValue>(IComparer<TValue>, IEnumerable<TValue>)` | `bool` | Checks enumerable with custom comparer |

---

## Ploch.Common.Reflection

### Type Extensions

**Namespace**: `Ploch.Common.Reflection`
**Class**: `TypeExtensions`
**File**: [src/Common/Reflection/TypeExtensions.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/Reflection/TypeExtensions.cs)

#### Type Checking Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `IsConcreteImplementation<TBaseType>()` | `bool` | Checks if type is concrete implementation of base type |
| `IsConcreteImplementation(Type)` | `bool` | Non-generic version of IsConcreteImplementation |
| `IsImplementing(Type, bool)` | `bool` | Checks if type implements interface/base type |
| `IsEnumerable()` | `bool` | Checks if type is IEnumerable |
| `IsNullable()` | `bool` | Checks if type is Nullable<T> |
| `IsSimpleType()` | `bool` | Checks if type is primitive, enum, string, decimal, or nullable of these |

#### Type Name Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetReadableTypeName()` | `string` | Gets human-readable type name including generics |

---

### Type Loader

**Namespace**: `Ploch.Common.Reflection`
**Classes**: `TypeLoader`, `TypeLoaderConfigurator`
**Files**: [src/Common/Reflection/TypeLoader.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/Reflection/TypeLoader.cs), [src/Common/Reflection/TypeLoaderConfigurator.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/Reflection/TypeLoaderConfigurator.cs)

#### Configuration Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `TypeLoader.Configure(Action<TypeLoaderConfigurator>)` | `TypeLoader` | Configures type loader with fluent API |
| `WithBaseType<T>()` | Configurator | Filters types by base type |
| `FromAssembliesContaining(string)` | Configurator | Includes assemblies matching pattern |
| `ExcludeAssemblies(string)` | Configurator | Excludes assemblies matching pattern |
| `LoadTypes()` | `IEnumerable<Type>` | Loads types matching criteria |

---

## Ploch.Common.IO

### Path Utilities

**Namespace**: `Ploch.Common.IO`
**Class**: `PathUtils`
**File**: [src/Common/IO/PathUtils.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/IO/PathUtils.cs)

#### Path Manipulation Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetDirectoryName(string)` | `string` | Gets short name of directory |
| `ToSafeFileName(string)` | `string` | Converts string to safe file name |
| `NormalizePathWithTrailingSeparator(string)` | `string` | Normalizes path ensuring trailing separator |
| `NormalizePathWithoutTrailingSeparator(string)` | `string` | Normalizes path removing trailing separator |
| `GetRelativePath(string, string)` | `string` | Creates relative path from one path to another |
| `WithExtension(string, bool, StringComparison)` | `string` | Appends or replaces file extension |
| `GetFullPathWithoutExtension(string)` | `string` | Gets full path without extension |

---

## Ploch.Common.Randomizers

**Namespace**: `Ploch.Common.Randomizers`
**Class**: `Randomizer`
**File**: [src/Common/Randomizers/Randomizer.cs](https://github.com/mrploch/ploch-common/blob/master/src/Common/Randomizers/Randomizer.cs)

### Randomizer Factory Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `GetRandomizer<TValue>()` | `IRangedRandomizer<TValue>` | Gets randomizer for specified type |
| `GetRandomizer(Type)` | `IRandomizer` | Gets randomizer for runtime type |

### Supported Types

- `string` - Random strings
- `int` - Random integers
- `DateTime` - Random dates
- `DateTimeOffset` - Random date-time offsets
- `bool` - Random booleans

### Randomizer Interfaces

| Interface | Method | Description |
|-----------|--------|-------------|
| `IRandomizer` | `GetRandom()` | Gets random value |
| `IRandomizer<T>` | `GetRandom()` | Gets typed random value |
| `IRangedRandomizer<T>` | `GetRandom(T, T)` | Gets random value in range |

---

## Ploch.Common.DependencyInjection

**Namespace**: `Ploch.Common.DependencyInjection`
**Source**: [src/Common.DependencyInjection/](https://github.com/mrploch/ploch-common/tree/master/src/Common.DependencyInjection/)

### Service Bundle

| Class | Method | Description |
|-------|--------|-------------|
| `ServicesBundle` | `ConfigureServices(IServiceCollection, IConfiguration)` | Override to configure services |
| Extensions | `AddServicesBundle<TBundle>(IConfiguration)` | Registers service bundle |

---

## Ploch.Common.Serialization

**Namespace**: `Ploch.Common.Serialization`
**Source**: [src/Common.Serialization/](https://github.com/mrploch/ploch-common/tree/master/src/Common.Serialization/)

### Serializer Interface

| Method | Returns | Description |
|--------|---------|-------------|
| `Serialize<T>(T)` | `string` | Serializes object to string |
| `Deserialize<T>(string)` | `T` | Deserializes string to object |
| `SerializeAsync<T>(T, Stream)` | `Task` | Async serialize to stream |
| `DeserializeAsync<T>(Stream)` | `Task<T>` | Async deserialize from stream |

### Implementations

- **SystemTextJsonSerializer** - Uses System.Text.Json
- **NewtonsoftJsonSerializer** - Uses Newtonsoft.Json

---

## Navigation

- [Home](https://github.com/mrploch/ploch-common#readme)
- [Getting Started](GETTING_STARTED.md)
- [Quick Reference](QUICK_REFERENCE.md)

## Source Code

Browse the source code on [GitHub](https://github.com/mrploch/ploch-common).

## Contributing

For API additions or changes, please submit a pull request to the [GitHub repository](https://github.com/mrploch/ploch-common).
