[![Build, Test and Analyze .NET](https://github.com/mrploch/ploch-common/actions/workflows/build-dotnet.yml/badge.svg)](https://github.com/mrploch/ploch-common/actions/workflows/build-dotnet.yml)
[![pages-build-deployment](https://github.com/mrploch/ploch-common/actions/workflows/pages/pages-build-deployment/badge.svg)](https://github.com/mrploch/ploch-common/actions/workflows/pages/pages-build-deployment)
[![Qodana](https://github.com/mrploch/ploch-common/actions/workflows/code_quality.yml/badge.svg)](https://github.com/mrploch/ploch-common/actions/workflows/code_quality.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mrploch_ploch-common&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mrploch_ploch-common)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=mrploch_ploch-common&metric=coverage)](https://sonarcloud.io/summary/new_code?id=mrploch_ploch-common)

# Ploch.Common Libraries

## Overview

A collection of utility libraries designed to simplify everyday .NET development tasks. These libraries provide reusable components, extension
methods, and helpers that reduce boilerplate code and increase productivity.

## Core Libraries

### Ploch.Common

The foundational library providing essential utilities and extensions.

#### Key Features

- **Null Safety**: Extension methods like `RequiredNotNull()` for enforcing non-null values with clear error messages
- **Extension Methods**: Useful extensions for strings, collections, and other common types
- **Reflection Helpers**: Simplified access to type information and members

#### Examples

```csharp
// Null checking with custom error messages
string value = obj.ToString().RequiredNotNull("Object string representation cannot be null");

// With formatted error message
string fieldValue = field.GetValue(null).ToString()
    .RequiredNotNull("The field {0} value cannot be null", field.Name);
```

### Ploch.Common.Reflection

Utilities for working with .NET reflection in a more convenient way.

#### Key Features

- **Type Inspection**: Methods to check interface implementation and inheritance
- **Member Access**: Extract values from properties and fields
- **Generic Type Support**: Work with open and closed generic types

#### Examples

```csharp
// Check if a type implements an interface
bool implementsInterface = type.IsImplementing(typeof(ICommand), includeDescendants: true);

// Get all public static member values from a type
var memberValues = aliasProviderType.GetMemberValues(BindingFlags.Public | BindingFlags.Static);

// Find all types implementing a generic interface
var useCaseTypes = assembly.GetTypes().Where(t => t.IsImplementing(typeof(IUseCase<,>)));
```

### Ploch.Common.AssemblyLoading

Tools for dynamic assembly scanning, type discovery, and loading.

#### Key Features

- **Type Scanning**: Find types matching specific criteria across assemblies
- **Assembly Filtering**: Include/exclude assemblies based on name patterns
- **Base Type Filtering**: Load only types implementing specific interfaces or base classes

#### Examples

```csharp
// Create a scanner that finds specific types
var typeScanner = new AppDomainTypesLoader(
    new TypeLoadingConfiguration(
        assemblyName => assemblyName.AddInclude("Ploch.*"),
        BaseTypes: [typeof(ICommand), typeof(IMessageHandler)]
    )
);

// Process assemblies and access loaded types
typeScanner.ProcessAllAssemblies();
var commandTypes = typeScanner.LoadedTypes
    .Where(t => t.IsImplementing(typeof(ICommand)));

// Simplified API
var commands = TypeLoader.Configure(cfg => cfg
    .WithBaseType<ICommand>()
    .FromAssembliesContaining("Ploch")
).LoadTypes();
```

### Ploch.Common.DependencyInjection

Extensions for working with dependency injection containers.

#### Key Features

- **Service Registration**: Simplified registration methods
- **Service Bundles**: Group related service registrations
- **Configuration Integration**: Support for configuration binding

#### Examples

```csharp
// Register services using a service bundle
services.AddServicesBundle<CommandServicesBundle>(configuration);

// Create a services bundle
public class CommandServicesBundle : ServicesBundle
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICommandHandler, DefaultCommandHandler>();
        services.AddSingleton<ICommandRegistry>();
    }
}
```

### Ploch.Common.DawnGuard

Parameter validation with detailed error reporting.

#### Key Features

- **Parameter Checking**: Validate method parameters
- **Fluent API**: Expressive validation rules
- **Custom Error Messages**: Detailed failure information

### Ploch.Serialization

Utilities for object serialization and deserialization.

#### Key Features

- **Format Support**: JSON, XML, and other formats
- **Custom Converters**: Handle complex types
- **Configuration Options**: Flexible serialization settings

### Ploch.Common.WebUI

Components and utilities for web applications.

#### Key Features

- **UI Components**: Reusable UI elements
- **Web Helpers**: Common web-related utilities
- **Framework Integration**: Works with popular web frameworks

### Ploch.TestingSupport

Tools for simplifying test development.

#### Key Features

- **Test Fixtures**: Base classes for common test scenarios
- **Mocking Helpers**: Simplify mock creation and configuration
- **Test Data Generators**: Create test data easily

### Ploch.TestingSupport.FluentAssertions

Extensions for the FluentAssertions library.

#### Key Features

- **Custom Assertions**: Domain-specific assertions
- **Readable Syntax**: Natural language assertions
- **Detailed Failure Messages**: Clear error information

### Ploch.TestingSupport.Xunit

Extensions for the XUnit testing framework.

#### Key Features

- **Test Attributes**: Custom attributes for test organization
- **Test Output**: Enhanced result reporting
- **Parameterization**: Simplified test parameterization

## Real-World Example

A location resolver that uses reflection utilities to process alias providers:

```csharp
// Configure a location resolver with alias providers
var resolver = LocationResolverConfigurator.Configure()
    .AddAliasProvider<LocationAliases>()
    .Build();

// Resolve a location using aliases
string resolvedPath = resolver.ResolveLocation("%Documents%/MyFolder");
```

The `LocationResolverConfigurator` uses reflection utilities to extract location tokens:

```csharp
public LocationResolver Build()
{
    var locationTokens = new HashSet<LocationToken>();
    
    foreach (var aliasProviderType in _aliasProviderTypes)
    {
        // Use reflection utilities to extract static members
        var values = aliasProviderType.GetMemberValues(BindingFlags.Public | BindingFlags.Static);
        
        foreach (var (key, value) in values)
        {
            locationTokens.Add(GetToken(key, value));
        }
    }
    
    return new LocationResolver(locationTokens.ToImmutableList());
}
```

```
