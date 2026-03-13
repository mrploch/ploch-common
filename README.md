[![NuGet](https://img.shields.io/nuget/v/Ploch.Common.svg)](https://www.nuget.org/packages/Ploch.Common)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Ploch.Common.svg)](https://www.nuget.org/packages/Ploch.Common)
[![Build, Test and Analyze .NET](https://github.com/mrploch/ploch-common/actions/workflows/build-dotnet.yml/badge.svg)](https://github.com/mrploch/ploch-common/actions/workflows/build-dotnet.yml)
[![pages-build-deployment](https://github.com/mrploch/ploch-common/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/mrploch/ploch-common/actions/workflows/pages/pages-build-deployment)
[![Qodana](https://github.com/mrploch/ploch-common/actions/workflows/code_quality.yml/badge.svg)](https://github.com/mrploch/ploch-common/actions/workflows/code_quality.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mrploch_ploch-common&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mrploch_ploch-common)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=mrploch_ploch-common&metric=coverage)](https://sonarcloud.io/summary/new_code?id=mrploch_ploch-common)

# Ploch.Common Libraries

A comprehensive collection of .NET utility libraries providing extension methods, helpers, and utilities to simplify everyday development tasks and reduce boilerplate code.

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Versioning and Releases](#versioning-and-releases)
- [Core Library (Ploch.Common)](#core-library-plochcommon)
  - [String Extensions](#string-extensions)
  - [Collection Extensions](#collection-extensions)
  - [Guard Clauses (Parameter Validation)](#guard-clauses-parameter-validation)
  - [IsIn Extensions](#isin-extensions)
  - [Type Extensions (Reflection)](#type-extensions-reflection)
  - [Path Utilities](#path-utilities)
  - [Randomizers](#randomizers)
- [Extended Libraries](#extended-libraries)
- [Real-World Examples](#real-world-examples)

## Overview

Ploch.Common is a suite of .NET libraries targeting `netstandard2.0` and `net8.0+`, offering:

- **Rich Extension Methods** for strings, collections, types, and more
- **Parameter Validation** with fluent Guard APIs
- **Reflection Utilities** for type inspection and manipulation
- **I/O Helpers** for path operations
- **Testing Support** with random data generators
- **Dependency Injection** extensions
- **Serialization** abstractions (JSON, XML)
- And much more...

## Installation

```bash
# Core library
dotnet add package Ploch.Common

# Additional libraries
dotnet add package Ploch.Common.DependencyInjection
dotnet add package Ploch.Common.Serialization
dotnet add package Ploch.Common.Serialization.SystemTextJson
dotnet add package Ploch.TestingSupport
```

## Versioning and Releases

This project uses [Nerdbank.GitVersioning (NBGV)](https://github.com/dotnet/Nerdbank.GitVersioning) for automatic
version computation from `version.json` and git commit history.

- **Stable releases** are published to [NuGet.org](https://www.nuget.org/profiles/mrploch) via the Release workflow
- **Prerelease packages** are published to GitHub Packages on every push to `master`
- **SourceLink** is enabled — step into library source code during debugging
- **Symbol packages** (`.snupkg`) are published to the NuGet symbol server

### Cutting a Release

1. Go to **Actions** → **Release** workflow → **Run workflow**
2. Enter the release version (e.g. `3.0`) — the patch version is computed by NBGV
3. Optionally enter the next development version (defaults to minor increment)
4. The workflow builds, tests, publishes to NuGet.org, creates a GitHub Release, and bumps the version

### Checking the Current Version Locally

```bash
dotnet tool restore
dotnet nbgv get-version
```

## Core Library (Ploch.Common)

### String Extensions

Convenient extension methods for common string operations.

#### Null/Empty Checking

```csharp
using Ploch.Common;

string? value = GetValue();

// Check if string is null or empty
if (value.IsNullOrEmpty())
{
    // Handle empty string
}

// Check if string is NOT null or empty
if (value.IsNotNullOrEmpty())
{
    ProcessValue(value);
}

// Check for whitespace
if (value.IsNullOrWhiteSpace())
{
    // Handle whitespace-only string
}

// Convert empty strings to null
string? result = "".NullIfEmpty(); // Returns null
string? result2 = "   ".NullIfWhiteSpace(); // Returns null
```

#### String Conversion and Encoding

```csharp
using Ploch.Common;

// Parse to integers
string number = "42";
int value = number.ToInt32(); // Returns 42
long bigValue = number.ToInt64(); // Returns 42L

// Safe conversion with TryConvert
if (number.TryConvertToInt32(out int result))
{
    Console.WriteLine($"Parsed: {result}");
}

// Base64 encoding/decoding
string original = "Hello, World!";
string encoded = original.ToBase64String(); // Encodes to base64
string decoded = encoded.FromBase64String(); // Decodes back
```

#### String Comparison and Manipulation

```csharp
using Ploch.Common;

// Case-insensitive comparison
string a = "Hello";
string b = "hello";
bool isEqual = a.EqualsIgnoreCase(b); // Returns true

// Replace at start of string
string path = "/api/users";
string newPath = path.ReplaceStart("/api", "/v2/api"); // Returns "/v2/api/users"

// Check if string contains any of the provided values
string text = "Hello World";
bool found = text.ContainsAny("World", "Universe", "Galaxy"); // Returns true
bool found2 = text.ContainsAny(StringComparison.OrdinalIgnoreCase, "WORLD"); // Returns true
```

### Collection Extensions

Powerful extensions for working with IEnumerable, arrays, and collections.

#### Value Checking (SQL-like IN operator)

```csharp
using Ploch.Common.Collections;

int status = 2;

// Check if value is in a set (like SQL IN)
if (status.ValueIn(1, 2, 3, 4))
{
    // Status is one of the valid values
}

string role = "Admin";
if (role.ValueIn("Admin", "Manager", "Supervisor"))
{
    // User has elevated permissions
}

// With custom comparer
if (name.ValueIn(StringComparer.OrdinalIgnoreCase, "admin", "manager"))
{
    // Case-insensitive check
}
```

#### Collection Operations

```csharp
using Ploch.Common.Collections;

var numbers = new[] { 1, 2, 3, 4, 5 };

// Check if none match a predicate (opposite of Any)
bool noneNegative = numbers.None(n => n < 0); // Returns true

// Join elements into a string
string result = numbers.Join(", "); // Returns "1, 2, 3, 4, 5"

// Join with custom selector
var users = GetUsers();
string names = users.Join(", ", u => u.Name);

// Join with different final separator
var items = new[] { "apples", "oranges", "bananas" };
string list = items.JoinWithFinalSeparator(", ", " and ");
// Returns "apples, oranges and bananas"

// Check if collection is empty
bool isEmpty = numbers.IsEmpty(); // Returns false
bool isNullOrEmpty = ((int[])null).IsNullOrEmpty(); // Returns true

// Get second element
int second = numbers.Second(); // Returns 2

// Exclude specific items
var filtered = numbers.ExceptItems(3, 5); // Returns [1, 2, 4]
```

#### Advanced Collection Operations

```csharp
using Ploch.Common.Collections;

var items = GetItems();

// Conditional filtering with If
var query = items
    .OrderBy(x => x.Created)
    .If(createdAfter.HasValue, q => q.Where(x => x.Created > createdAfter.Value))
    .If(first.HasValue, q => q.Take(first.Value));

// ForEach operation
items.ForEach(item => Console.WriteLine(item.Name));

// Shuffle collection randomly
var shuffled = items.Shuffle();

// Take random items
var randomItems = items.TakeRandom(5);

// Check if integers are sequential
var numbers = new[] { 1, 2, 3, 4, 5 };
bool sequential = numbers.AreIntegersSequentialInOrder(); // Returns true
```

### Guard Clauses (Parameter Validation)

Fluent API for parameter validation with clear error messages.

#### Null Checking

```csharp
using Ploch.Common.ArgumentChecking;

public void ProcessUser(User user, string email)
{
    // Throws ArgumentNullException if null
    user.NotNull(nameof(user));
    email.NotNull(nameof(email));

    // For reference types - throws InvalidOperationException if null
    var name = user.Name.RequiredNotNull(nameof(user.Name));

    // With custom error message
    var address = user.Address.RequiredNotNull(
        nameof(user.Address),
        "User {0} must have an address"
    );
}
```

#### String and Collection Validation

```csharp
using Ploch.Common.ArgumentChecking;

public void CreateAccount(string username, string password, List<string> roles)
{
    // Validate string is not null or empty
    username.NotNullOrEmpty(nameof(username));
    password.NotNullOrEmpty(nameof(password));

    // Validate collection is not null or empty
    roles.NotNullOrEmpty(nameof(roles));

    // RequiredNotNullOrEmpty throws InvalidOperationException
    var validatedName = username.RequiredNotNullOrEmpty(
        nameof(username),
        "Username {0} cannot be empty"
    );
}
```

#### Value Validation

```csharp
using Ploch.Common.ArgumentChecking;

public void SetTimeout(int milliseconds, OrderStatus status)
{
    // Ensure value is positive
    milliseconds.Positive(nameof(milliseconds));

    // Ensure enum value is defined
    status.NotOutOfRange(nameof(status));

    // Boolean conditions
    var isValid = ValidateInput();
    isValid.RequiredTrue("Input validation failed");
}
```

### IsIn Extensions

Check if a value exists in a collection (similar to SQL IN operator).

```csharp
using Ploch.Common;

int statusCode = GetStatusCode();

// Check if value is in collection
if (statusCode.In(200, 201, 204))
{
    // Success status codes
}

// Check if NOT in collection
if (statusCode.NotIn(400, 401, 403, 404))
{
    // Not a client error
}

// Works with enumerables
var validCodes = new List<int> { 200, 201, 202 };
if (statusCode.In(validCodes))
{
    // Valid status
}

// With custom comparer
if (name.In(StringComparer.OrdinalIgnoreCase, "admin", "root"))
{
    // Case-insensitive check
}
```

### Type Extensions (Reflection)

Utilities for type inspection and reflection operations.

#### Type Checking

```csharp
using Ploch.Common.Reflection;

Type type = typeof(MyService);

// Check if type implements an interface
bool implements = type.IsImplementing(typeof(IService));

// Check if it's a concrete implementation (not abstract/interface)
bool isConcrete = type.IsConcreteImplementation(typeof(IService));

// Generic version
bool implementsGeneric = type.IsConcreteImplementation<IService>();

// Check for generic interfaces
bool hasGeneric = type.IsImplementing(typeof(IRepository<>), concreteOnly: true);

// Check if type is enumerable
bool isEnumerable = type.IsEnumerable();

// Check if type is nullable
bool isNullable = typeof(int?).IsNullable(); // Returns true

// Check if type is "simple" (primitive, string, decimal, enum, or nullable of these)
bool isSimple = typeof(int).IsSimpleType(); // Returns true
bool isSimple2 = typeof(DateTime).IsSimpleType(); // Returns true
```

#### Type Name Generation

```csharp
using Ploch.Common.Reflection;

// Get readable type names for generics
Type listType = typeof(List<int>);
string name = listType.GetReadableTypeName(); // Returns "List<int>"

Type dictType = typeof(Dictionary<string, List<User>>);
string dictName = dictType.GetReadableTypeName();
// Returns "Dictionary<string, List<User>>"

// Works with arrays
Type arrayType = typeof(int[]);
string arrayName = arrayType.GetReadableTypeName(); // Returns "int[]"
```

### Path Utilities

File path manipulation and validation utilities.

```csharp
using Ploch.Common.IO;

// Get directory name
string dirPath = "/usr/local/bin";
string name = dirPath.GetDirectoryName(); // Returns "bin"

// Convert to safe file name
string input = "My Document: Version 1.0";
string safe = PathUtils.ToSafeFileName(input);
// Returns "My Document_ Version 1_0"

// Normalize paths
string path = "/some/path/";
string normalized = PathUtils.NormalizePathWithTrailingSeparator(path);
// Returns "/some/path/" with proper separators

string withoutTrailing = PathUtils.NormalizePathWithoutTrailingSeparator(path);
// Returns "/some/path"

// Get relative path
string from = "/home/user/projects";
string to = "/home/user/documents/file.txt";
string relative = PathUtils.GetRelativePath(from, to);
// Returns "../documents/file.txt"

// Manage file extensions
string file = "/path/to/file.txt";
string withNewExt = file.WithExtension(".md");
// Returns "/path/to/file.md"

string withoutExt = PathUtils.GetFullPathWithoutExtension(file);
// Returns "/path/to/file"
```

### Randomizers

Generate random data for testing purposes.

```csharp
using Ploch.Common.Randomizers;

// Get randomizer for a specific type
var stringRandomizer = Randomizer.GetRandomizer<string>();
string randomString = stringRandomizer.GetRandom();

var intRandomizer = Randomizer.GetRandomizer<int>();
int randomInt = intRandomizer.GetRandom();

// With ranges
var rangedInt = (IRangedRandomizer<int>)intRandomizer;
int value = rangedInt.GetRandom(min: 1, max: 100);

var dateRandomizer = Randomizer.GetRandomizer<DateTime>();
DateTime randomDate = dateRandomizer.GetRandom();

// Boolean randomizer
var boolRandomizer = Randomizer.GetRandomizer<bool>();
bool randomBool = boolRandomizer.GetRandom();

// Using runtime type
Type type = typeof(int);
var randomizer = Randomizer.GetRandomizer(type);
object randomValue = randomizer.GetRandom();
```

## Extended Libraries

### Ploch.Common.DependencyInjection

Extensions for working with dependency injection containers.

```csharp
using Ploch.Common.DependencyInjection;

// Register services using a service bundle
services.AddServicesBundle<CommandServicesBundle>(configuration);

// Create a services bundle
public class CommandServicesBundle : ServicesBundle
{
    public override void ConfigureServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<ICommandHandler, DefaultCommandHandler>();
        services.AddSingleton<ICommandRegistry, CommandRegistry>();

        // Bind configuration
        services.Configure<CommandOptions>(
            configuration.GetSection("Commands")
        );
    }
}
```

### Ploch.Common.Serialization

Abstraction layer for serialization with multiple format support.

```csharp
using Ploch.Common.Serialization;

// System.Text.Json
using Ploch.Common.Serialization.SystemTextJson;

var serializer = new SystemTextJsonSerializer();

// Serialize
var user = new User { Name = "John", Email = "john@example.com" };
string json = serializer.Serialize(user);

// Deserialize
var deserializedUser = serializer.Deserialize<User>(json);

// Async operations
await serializer.SerializeAsync(user, stream);
var user2 = await serializer.DeserializeAsync<User>(stream);

// Newtonsoft.Json
using Ploch.Common.Serialization.NewtonsoftJson;

var newtonsoftSerializer = new NewtonsoftJsonSerializer();
string json2 = newtonsoftSerializer.Serialize(user);
```

### Ploch.Common.Reflection (Assembly Loading)

Tools for dynamic assembly scanning and type discovery.

```csharp
using Ploch.Common.Reflection;

// Configure type loader
var commands = TypeLoader.Configure(cfg => cfg
    .WithBaseType<ICommand>()
    .FromAssembliesContaining("MyApp")
    .ExcludeAssemblies("*.Tests")
).LoadTypes();

// Manual configuration
var typeScanner = new AppDomainTypesLoader(
    new TypeLoadingConfiguration(
        assemblyName => assemblyName.AddInclude("MyApp.*"),
        BaseTypes: [typeof(ICommand), typeof(IMessageHandler)]
    )
);

typeScanner.ProcessAllAssemblies();
var loadedTypes = typeScanner.LoadedTypes
    .Where(t => t.IsConcreteImplementation<ICommand>());
```

### Ploch.TestingSupport

Testing utilities and random data generators.

```csharp
using Ploch.TestingSupport;

// Base test class with common functionality
public class MyTests : TestFixtureBase
{
    [Fact]
    public void TestMethod()
    {
        // Use built-in test helpers
        var randomData = GenerateRandomTestData();
        // ...
    }
}
```

### Ploch.TestingSupport.FluentAssertions

Custom assertions for FluentAssertions.

```csharp
using Ploch.TestingSupport.FluentAssertions;

// Custom domain-specific assertions
result.Should().BeSuccessful();
user.Should().HaveValidEmail();
collection.Should().ContainItemsMatching(predicate);
```

## Real-World Examples

### Example 1: Input Validation

```csharp
using Ploch.Common;
using Ploch.Common.ArgumentChecking;

public class UserService
{
    public User CreateUser(string email, string password, UserRole role)
    {
        // Validate parameters
        email.NotNullOrEmpty(nameof(email));
        password.NotNullOrEmpty(nameof(password));
        role.NotOutOfRange(nameof(role));

        // Additional validation
        if (!email.ContainsAny("@"))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }

        // Ensure valid role
        if (!role.In(UserRole.Admin, UserRole.User, UserRole.Guest))
        {
            throw new ArgumentException("Invalid role", nameof(role));
        }

        return new User { Email = email, Role = role };
    }
}
```

### Example 2: Configuration with Type Discovery

```csharp
using Ploch.Common.Reflection;
using Ploch.Common.DependencyInjection;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        // Discover and register all command handlers
        var handlers = TypeLoader.Configure(cfg => cfg
            .WithBaseType<ICommandHandler>()
            .FromAssembliesContaining("MyApp.Commands")
        ).LoadTypes();

        foreach (var handlerType in handlers)
        {
            if (handlerType.IsConcreteImplementation<ICommandHandler>())
            {
                services.AddTransient(typeof(ICommandHandler), handlerType);
            }
        }

        return services;
    }
}
```

### Example 3: Safe Path Operations

```csharp
using Ploch.Common.IO;
using Ploch.Common.ArgumentChecking;

public class FileManager
{
    private readonly string _basePath;

    public FileManager(string basePath)
    {
        _basePath = basePath.NotNullOrEmpty(nameof(basePath));
    }

    public string CreateSafeFilePath(string fileName, string extension)
    {
        fileName.NotNullOrEmpty(nameof(fileName));

        // Create safe file name
        string safeFileName = PathUtils.ToSafeFileName(fileName);

        // Add extension
        string fileWithExt = safeFileName.WithExtension(extension);

        // Create full path
        string fullPath = Path.Combine(_basePath, fileWithExt);

        // Normalize
        return PathUtils.NormalizePathWithoutTrailingSeparator(fullPath);
    }

    public string GetRelativeDocumentPath(string documentPath)
    {
        documentPath.NotNullOrEmpty(nameof(documentPath));

        return PathUtils.GetRelativePath(_basePath, documentPath);
    }
}
```

### Example 4: Data Processing with Collections

```csharp
using Ploch.Common.Collections;

public class DataProcessor
{
    public ProcessResult ProcessRecords(
        IEnumerable<Record> records,
        DateTime? afterDate,
        RecordStatus? status,
        int? maxResults)
    {
        // Validate input
        if (records.IsNullOrEmpty())
        {
            return ProcessResult.Empty;
        }

        // Build query with conditional filters
        var query = records
            .OrderBy(r => r.CreatedDate)
            .If(afterDate.HasValue, q => q.Where(r => r.CreatedDate > afterDate.Value))
            .If(status.HasValue, q => q.Where(r => r.Status == status.Value))
            .If(maxResults.HasValue, q => q.Take(maxResults.Value));

        var processed = query
            .ForEach(r => r.MarkAsProcessed())
            .ToList();

        // Generate summary
        var statuses = processed.Join(", ", r => r.Status);

        return new ProcessResult
        {
            Count = processed.Count,
            Summary = $"Processed {processed.Count} records with statuses: {statuses}"
        };
    }
}
```

### Example 5: Type-Safe Configuration

```csharp
using Ploch.Common;
using Ploch.Common.ArgumentChecking;

public class AppConfiguration
{
    public static T GetRequiredConfig<T>(
        IConfiguration configuration,
        string sectionName)
    {
        sectionName.NotNullOrEmpty(nameof(sectionName));
        configuration.NotNull(nameof(configuration));

        var section = configuration.GetSection(sectionName);
        var config = section.Get<T>();

        return config.RequiredNotNull(
            nameof(config),
            $"Configuration section '{sectionName}' is required but was not found"
        );
    }
}
```

## License

[Your License Here]

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/mrploch/ploch-common).
