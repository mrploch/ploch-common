# Ploch.Common.Extensions.Configuration

> Extension methods for binding configuration sections to strongly-typed options classes.

## Overview

`Ploch.Common.Extensions.Configuration` provides two `IServiceCollection` extension methods that simplify the `IOptions<T>` / `Configure<T>` registration pattern from `Microsoft.Extensions.Options`. Instead of repeating the section name as a magic string at every call site, you either place a `[ConfigurationSection]` attribute on the options class or derive the section name from the class name automatically.

The package targets `net8.0`.

## Installation

```
dotnet add package Ploch.Common.Extensions.Configuration
```

## Key Types

| Type | Kind | Description |
|------|------|-------------|
| `ConfigurationSectionAttribute` | Attribute | Decorates an options class with an explicit configuration section name |
| `ConfigurationOptionsExtensions` | Static class | Extension methods `AddConfigurationSection<T>` and `AddConfigurationOptions<TMain, TSub>` on `IServiceCollection` |

## Usage Examples

### Binding a top-level section by class name

Without the attribute, the section name defaults to the class name. Given the following `appsettings.json`:

```json
{
  "SmtpSettings": {
    "Host": "mail.example.com",
    "Port": 587
  }
}
```

Register and consume `IOptions<SmtpSettings>`:

```csharp
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}

// Startup / Program.cs
services.AddConfigurationSection<SmtpSettings>(configuration);

// Consumer
public class EmailSender(IOptions<SmtpSettings> options) { ... }
```

### Using `[ConfigurationSection]` to override the section name

When the class name does not match the JSON key, apply the attribute:

```csharp
[ConfigurationSection("Smtp")]
public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
}

// Maps to the "Smtp" key rather than "SmtpSettings"
services.AddConfigurationSection<SmtpSettings>(configuration);
```

### Binding a nested sub-section

`AddConfigurationOptions` binds a child property of a parent section using a strongly-typed lambda — no string literals for the sub-section path:

```json
{
  "Application": {
    "Database": {
      "ConnectionString": "Data Source=app.db"
    }
  }
}
```

```csharp
public class ApplicationSettings
{
    public DatabaseSettings Database { get; set; } = new();
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; } = string.Empty;
}

services.AddConfigurationOptions<ApplicationSettings, DatabaseSettings>(
    configuration,
    app => app.Database);
```

The section is resolved as `Application:Database`, and `IOptions<DatabaseSettings>` is registered in the container.

## Method Signatures

```csharp
// Top-level section binding
public static IServiceCollection AddConfigurationSection<TSection>(
    this IServiceCollection services,
    IConfiguration configuration)
    where TSection : class, new();

// Nested sub-section binding
public static IServiceCollection AddConfigurationOptions<TMainSection, TSubSection>(
    this IServiceCollection services,
    IConfiguration configuration,
    Expression<Func<TMainSection, TSubSection>> subSectionProperty)
    where TSubSection : class;
```

## Section Name Resolution

`AddConfigurationSection<TSection>` resolves the section name in this order:

1. The `SectionName` from a `[ConfigurationSection("...")]` attribute on `TSection`, if present.
2. The class name of `TSection` as a fallback.

## Related Libraries

- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — ServicesBundle pattern for organising DI registrations; can use `AddConfigurationSection` inside a `ConfigurableServicesBundle`
