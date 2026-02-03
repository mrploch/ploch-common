# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Ploch.Common is a comprehensive suite of .NET utility libraries targeting `netstandard2.0` and `net8.0+`, providing extension methods, helpers, and utilities to simplify everyday development tasks.

**Solution File:** `Ploch.Common.Serialization.sln` (traditional) or `Ploch.Common.slnx` (XML-based)

## Build and Test Commands

### Building
```bash
# Build entire solution
dotnet build Ploch.Common.slnx

# Build specific project
dotnet build src/Common/Ploch.Common.csproj

# Build in Release mode
dotnet build -c Release
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/Common.Tests/Ploch.Common.Tests.csproj

# Run a single test by name
dotnet test --filter "FullyQualifiedName~MethodName_should_explain_what_it_should_do"

# Run tests matching a pattern
dotnet test --filter "FullyQualifiedName~Ploch.Common.Tests.Collections"

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"
```

### Restore and Clean
```bash
dotnet restore
dotnet clean
```

## Architecture Overview

### Library Organization

The repository contains **27+ source projects** organized in functional tiers:

**Core Foundation:**
- `Ploch.Common` - Base utility library with extension methods for strings, collections, types, paths, etc.

**Abstraction Libraries:**
- `Ploch.Common.Serialization` - Abstract serialization interfaces (ISerializer, IAsyncSerializer)
- `Ploch.Common.DependencyInjection` - DI container abstractions with ServicesBundle pattern
- `Ploch.Common.Apps.Actions.Model` - Action/command execution framework with priority-based handlers

**Concrete Implementations:**
- `Ploch.Common.Serialization.SystemTextJson` - System.Text.Json implementation
- `Ploch.Common.Serialization.NewtonsoftJson` - Newtonsoft.Json implementation
- Corresponding `.ExtensionsDependencyInjection` variants for DI registration

**Testing Infrastructure:**
- `Ploch.TestingSupport` - Base testing utilities
- `Ploch.TestingSupport.XUnit3` - XUnit 3 support with custom attributes
- `Ploch.TestingSupport.XUnit3.AutoMoq` - AutoFixture + Moq integration
- `Ploch.TestingSupport.FluentAssertions` - Custom FluentAssertions extensions

### Key Architectural Patterns

#### 1. ServicesBundle Pattern (Dependency Injection)
Modular service registration inspired by Autofac modules:

```csharp
public class MyServicesBundle : ServicesBundle
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IMyService, MyService>();
    }
}

// Usage
services.AddServicesBundle<MyServicesBundle>(configuration);
```

- Bundles can declare `Dependencies` (other bundles configured first)
- `ConfigurableServicesBundle` requires IConfiguration
- Extension methods support both type-based and instance-based registration

#### 2. Generic Serialization Abstraction
Two-tier hierarchy supporting sync and async:

- `ISerializer` - Basic sync serialization
- `ISerializer<TSettings>` - Configuration support (e.g., JsonSerializerOptions)
- `IAsyncSerializer` - Stream-based async operations
- `IAsyncSerializer<TSettings>` - Combines async + configuration

Base classes: `Serializer<TSettings, TDataJsonObject>` and `AsyncSerializer<TSettings, TDataJsonObject>`

#### 3. Modular Design Pattern
Pattern: Base implementation library + separate DI extension library

Example:
- `Common.Serialization.SystemTextJson` (core implementation)
- `Common.Serialization.SystemTextJson.ExtensionsDependencyInjection` (ServicesBundle for registration)

This separation allows using implementations without forcing DI dependencies.

#### 4. Action/Command Handler Framework
Priority-based generic handler system in `Common.Apps.Actions.Model`:

```csharp
public interface IActionHandler<TTarget, TActionInfo>
{
    int Priority { get; }
    Type ActionInfoType { get; }
    Task<ActionHandlerResult<TTarget>> ExecuteAsync(TActionInfo actionInfo, CancellationToken cancellationToken);
}
```

`ActionHandlerManager` orchestrates multiple handlers with dependency resolution.

### Important Base Classes and Namespaces

#### ArgumentChecking (Validation)
Fluent guard clause methods in `Ploch.Common.ArgumentChecking`:
- `NotNull<T>()` - Throws ArgumentNullException if null
- `NotNullOrEmpty()` - Validates strings and collections
- `RequiredNotNull<T>()` - Throws InvalidOperationException (for required state)
- `Positive<T>()` - Generic numeric validation
- `NotOutOfRange<T>()` - Enum validation

**Note:** `Common.DawnGuard` (using Dawn.Guard library) is deprecated in favor of ArgumentChecking.

#### Collections Extensions
Located in `Ploch.Common.Collections`:
- `ForEach()` - Side effects with fluent chaining
- `None()` - Inverse of `Any()`
- `Join()`, `JoinWithFinalSeparator()` - String concatenation
- `If()` - Conditional query filtering (useful for optional filters)
- `Shuffle()`, `TakeRandom()` - Randomization
- `IsNullOrEmpty()`, `Second()`, `ExceptItems()`

#### Reflection Utilities
Located in `Ploch.Common.Reflection`:
- `IsImplementing()` - Check interface implementation
- `IsConcreteImplementation<T>()` - Check for concrete (non-abstract) implementations
- `GetReadableTypeName()` - Human-readable generic type names
- `IsSimpleType()` - Primitives, strings, decimals, enums, and their nullable versions
- `TypeLoader` - Dynamic assembly scanning and type discovery

## Test Conventions

### Test Naming Convention
```csharp
[Fact]
public void MethodName_should_explain_what_it_should_do()
{
    // Arrange
    // Act
    // Assert
}
```

Use `[Theory]` whenever possible for parameterized tests.

### Testing Framework
- **Test Framework:** XUnit (version 3)
- **Assertions:** FluentAssertions
- **Mocking:** Moq
- **Test Data:** AutoFixture with AutoMoq

### Custom XUnit Attributes
- `[JsonFileData("file.json", "propertyPath")]` - Load test data from JSON files
- `[TextFileData("file.txt")]` - Read text file as test parameter
- `[TextFileLinesData("file.txt")]` - Each line as separate test case
- `[AutoMockData]` - Auto-generate mocks using AutoFixture + Moq

### Example Test Patterns
```csharp
[Theory]
[InlineData("input", "expected")]
public void Method_should_transform_input(string input, string expected)
{
    // Test implementation
}

[Theory]
[AutoMockData]
public void Service_should_call_dependency(IMyService sut, Mock<IRepository> repoMock)
{
    // AutoFixture creates sut with mocked dependencies
}

[Theory]
[JsonFileData("testdata.json", "testCases")]
public void Method_should_handle_complex_data(ComplexInput input, string expected)
{
    // Data loaded from JSON file
}
```

## Code Quality and Analysis

### Analyzers Enabled
- StyleCop (custom ruleset)
- ErrorProne.NET.CoreAnalyzers
- Philips.CodeAnalysis.DuplicateCodeAnalyzer
- PrimaryConstructorAnalyzer
- ReflectionAnalyzers
- Latest .NET analyzers (`AnalysisLevel=latest-Recommended`)

### Coding Standards
- `Nullable` reference types enabled
- `EnforceCodeStyleInBuild` enabled
- Warnings as errors for Nullable and NU1605
- Generate XML documentation for non-test projects

## SonarQube Integration

After modifying code files:
1. Disable automatic analysis: Use `toggle_automatic_analysis` tool
2. Make code changes
3. Analyze changed files: Use `analyze_file_list` tool
4. Re-enable automatic analysis: Use `toggle_automatic_analysis` tool

**Note:** Don't verify fixes using `search_sonar_issues_in_projects` immediately after changes (server lag).

## Git Workflow

- **Main branch:** `master`
- **Feature branches:** Typically named `#<issue-number>-description` (e.g., `#135-crud-endpoints`)
- Pull requests target `master` branch
- CI/CD runs on push to master and on pull requests

## CI/CD Pipeline

GitHub Actions workflow (`.github/workflows/build-dotnet.yml`):
1. Build solution
2. Run tests with code coverage
3. SonarCloud analysis
4. Publish test results and coverage reports
5. Deploy documentation to GitHub Pages (on master)
6. Publish NuGet packages to GitHub Packages

## Project Configuration

### Global Settings
Defined in `Directory.Build.props`:
- Version: `2.0.1` (prerelease builds append `-prerelease-<timestamp>`)
- Target frameworks: netstandard2.0, net8.0, net9.0 (varies by project)
- Test projects: Automatically detected (ends with "Tests")
- Package generation: Enabled for non-test projects
- XML documentation: Generated for non-test projects

### Multi-Targeting
Core library (`Ploch.Common`) targets both:
- `netstandard2.0` - Maximum compatibility
- `net8.0` - Modern .NET features

Most other libraries target `netstandard2.0` only.
