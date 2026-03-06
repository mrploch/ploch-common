# Ploch.Common Documentation

Welcome to the comprehensive documentation for **Ploch.Common** - a collection of utility libraries for .NET development.

## Quick Start

- **New to Ploch.Common?** Start with the [Getting Started Guide](GETTING_STARTED.md)
- **Need a quick reference?** Check the [Quick Reference](QUICK_REFERENCE.md)
- **Looking for examples?** See the [Main README](../README.md)
- **Need API details?** Browse the [API Reference](API_REFERENCE.md)
- **Want organized navigation?** Use the [Documentation Index](INDEX.md)

## Documentation Structure

### 📚 Main Documentation

**[README.md](../README.md)**

- Comprehensive overview of all libraries
- Detailed code examples for each feature
- Real-world usage scenarios
- Extended libraries documentation

### 🚀 Getting Started

**[Getting Started Guide](GETTING_STARTED.md)**

- Installation instructions for all platforms
- Your first steps with the library
- Common use cases and patterns
- Best practices and tips
- Troubleshooting guide

### ⚡ Quick Reference

**[Quick Reference](QUICK_REFERENCE.md)**

- Condensed API cheat sheet
- Common operations at a glance
- Code snippets for copy-paste
- Migration guide from standard .NET
- Namespace reference table

### 📖 API Reference

**[API Reference](API_REFERENCE.md)**

- Complete method signatures
- Parameter documentation
- Return types and exceptions
- Organized by namespace and class
- Links to source code

### 🗺️ Documentation Hub

**[Documentation Index](INDEX.md)**

- Comprehensive navigation guide
- Learning paths for different skill levels
- Examples organized by difficulty
- Quick answers to common questions
- Links to all documentation resources

## What's in Ploch.Common?

### Core Features

- **[String Extensions](../README.md#string-extensions)** - Null checking, parsing, encoding, comparison
- **[Collection Extensions](../README.md#collection-extensions)** - LINQ enhancements, safe operations, joining
- **[Guard Clauses](../README.md#guard-clauses-parameter-validation)** - Parameter validation with fluent API
- **[IsIn Extensions](../README.md#isin-extensions)** - SQL-like IN operator for C#
- **[Type Extensions](../README.md#type-extensions-reflection)** - Reflection utilities and type inspection
- **[Path Utilities](../README.md#path-utilities)** - Cross-platform file path operations
- **[Randomizers](../README.md#randomizers)** - Random data generation for testing

### Extended Libraries

- **Dependency Injection** - Service bundle registration and configuration
- **Serialization** - Multi-format serialization abstraction (JSON, XML)
- **Testing Support** - Test fixtures, random data, FluentAssertions extensions
- **Web UI** - Web application components and utilities

## Quick Examples

### String Operations

```csharp
using Ploch.Common;

// Null checking
if (value.IsNotNullOrEmpty())
{
    ProcessValue(value);
}

// Encoding
string encoded = text.ToBase64String();

// Comparison
if (str.EqualsIgnoreCase("hello"))
{
    // ...
}
```

### Parameter Validation

```csharp
using Ploch.Common.ArgumentChecking;

public void ProcessUser(User user, string email)
{
    user.NotNull(nameof(user));
    email.NotNullOrEmpty(nameof(email));

    // Safe to use validated parameters
}
```

### Collection Operations

```csharp
using Ploch.Common.Collections;

// Check if value is in set
if (status.ValueIn(1, 2, 3, 4))
{
    // ...
}

// Join elements
string names = users.Join(", ", u => u.Name);

// Conditional filtering
var results = query
    .If(filter.HasValue, q => q.Where(x => x.Status == filter.Value))
    .ToList();
```

### Type Inspection

```csharp
using Ploch.Common.Reflection;

// Check type implementation
if (type.IsConcreteImplementation<IService>())
{
    // Register or use type
}

// Get readable name
string name = typeof(List<int>).GetReadableTypeName();
// Returns "List<int>"
```

## Installation

```bash
# Core library
dotnet add package Ploch.Common

# Additional libraries
dotnet add package Ploch.Common.DependencyInjection
dotnet add package Ploch.Common.Serialization
dotnet add package Ploch.TestingSupport
```

## Learning Paths

### Beginner

1. [Getting Started](GETTING_STARTED.md) - Installation and basics
2. [String Extensions](../README.md#string-extensions)
3. [Guard Clauses](../README.md#guard-clauses-parameter-validation)
4. [Quick Reference](QUICK_REFERENCE.md) - Keep handy

### Intermediate

1. [Collection Extensions](../README.md#collection-extensions)
2. [IsIn Extensions](../README.md#isin-extensions)
3. [Path Utilities](../README.md#path-utilities)
4. [Real-World Examples](../README.md#real-world-examples)

### Advanced

1. [Type Extensions](../README.md#type-extensions-reflection)
2. [Type Loading](API_REFERENCE.md#type-loader)
3. [Dependency Injection](../README.md#plochcommondependencyinjection)
4. [Contribute on GitHub](https://github.com/mrploch/ploch-common)

## Design Philosophy

The Ploch.Common library suite is designed with these principles:

1. **Zero Runtime Dependencies** - Only uses .NET standard library
2. **Extension Method First** - Fluent, readable API
3. **Fail Fast** - Clear error messages with parameter validation
4. **Well Tested** - Extensive test coverage for reliability
5. **Cross-Platform** - Works on Windows, Linux, and macOS

## Finding What You Need

| I want to... | Go to... |
|--------------|----------|
| Get started quickly | [Getting Started Guide](GETTING_STARTED.md) |
| See what's available | [Main README](../README.md) |
| Find a specific method | [API Reference](API_REFERENCE.md) |
| Get a quick reminder | [Quick Reference](QUICK_REFERENCE.md) |
| Navigate all docs | [Documentation Index](INDEX.md) |
| See code examples | [Real-World Examples](../README.md#real-world-examples) |
| Learn best practices | [Getting Started - Best Practices](GETTING_STARTED.md#best-practices) |

## Test-Driven Documentation

Due to the nature of this project, **the tests serve as living documentation** - a comprehensive catalogue of usage examples.

The library comes with extensive test coverage. You can explore the tests to see more usage patterns:

- [String Extensions Tests](../tests/Ploch.Common.Tests/StringExtensionsTests.cs)
- [Collection Extensions Tests](../tests/Ploch.Common.Tests/Collections/EnumerableExtensionsTests.cs)
- [Guard Tests](../tests/Ploch.Common.Tests/ArgumentChecking/)
- [Type Extensions Tests](../tests/Ploch.Common.Tests/Reflection/TypeExtensionsTests.cs)

## Contributing

Found an error or want to improve the documentation?

1. Visit the [GitHub repository](https://github.com/mrploch/ploch-common)
2. Open an issue or submit a pull request
3. Help make the library better for everyone!

## Support

- **Documentation Issues**: [Report here](https://github.com/mrploch/ploch-common/issues/new)
- **Questions**: [GitHub Discussions](https://github.com/mrploch/ploch-common/discussions)
- **Bug Reports**: [Issue Tracker](https://github.com/mrploch/ploch-common/issues)

## Additional Resources

- [NuGet Package](https://www.nuget.org/packages/Ploch.Common)
- [Source Code](https://github.com/mrploch/ploch-common)
- [Build Status](https://github.com/mrploch/ploch-common/actions)
- [Code Quality](https://sonarcloud.io/summary/new_code?id=mrploch_ploch-common)

---

**Ready to start?** Head to the [Getting Started Guide](GETTING_STARTED.md) or jump straight to the [Main README](../README.md) for comprehensive examples.

Hopefully, you'll find something useful! 🚀
