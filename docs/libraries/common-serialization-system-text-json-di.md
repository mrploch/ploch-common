# Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection

> DI registration helpers for wiring `SystemTextJsonSerializer` into a `Microsoft.Extensions.DependencyInjection` container.

## Overview

This library provides two registration entry points — a `ServicesBundle` class and an `IServiceCollection` extension method — for registering `SystemTextJsonSerializer` as a singleton under all four serialisation interfaces. It follows the Ploch modular DI pattern: the `ServicesBundle` is the composable unit, and the extension method is a thin convenience wrapper around it.

After registration, any of the following interfaces can be injected and will resolve to the same singleton `SystemTextJsonSerializer` instance:

- `ISerializer`
- `ISerializer<JsonSerializerOptions>`
- `IAsyncSerializer`
- `IAsyncSerializer<JsonSerializerOptions>`
- `SystemTextJsonSerializer` (direct type)

The library targets `netstandard2.0` and depends on `Microsoft.Extensions.DependencyInjection.Abstractions`.

## Installation

```shell
dotnet add package Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `SystemTextJsonSerializerServicesBundle` | Class | `ServicesBundle` subclass that registers `SystemTextJsonSerializer` and `JsonSerializerOptions` as singletons. Accepts optional `JsonSerializerOptions` in its constructor. |
| `SystemTextJsonSerializerRegistration` | Static class | Provides the `AddSystemTextJsonSerializer` extension method on `IServiceCollection`. |

## Configuration

### Using the extension method (recommended for most applications)

```csharp
// Startup / Program.cs
services.AddSystemTextJsonSerializer();
```

With custom options:

```csharp
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

services.AddSystemTextJsonSerializer(options);
```

### Using the ServicesBundle directly

Use `SystemTextJsonSerializerServicesBundle` when composing bundles or when you need to declare it as a dependency of another bundle:

```csharp
services.AddServicesBundle(new SystemTextJsonSerializerServicesBundle());
```

With custom options:

```csharp
services.AddServicesBundle(new SystemTextJsonSerializerServicesBundle(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
}));
```

As a dependency inside another `ServicesBundle`:

```csharp
public class MyAppServicesBundle : ServicesBundle
{
    protected override IEnumerable<IServicesBundle>? Dependencies =>
        [new SystemTextJsonSerializerServicesBundle()];

    public override void DoConfigure()
    {
        Services.AddScoped<IReportExporter, ReportExporter>();
    }
}
```

### Registered services after `AddSystemTextJsonSerializer`

| Service type | Implementation | Lifetime |
|---|---|---|
| `SystemTextJsonSerializer` | `SystemTextJsonSerializer` | Singleton |
| `ISerializer` | `SystemTextJsonSerializer` | Singleton |
| `ISerializer<JsonSerializerOptions>` | `SystemTextJsonSerializer` | Singleton |
| `IAsyncSerializer` | `SystemTextJsonSerializer` | Singleton |
| `IAsyncSerializer<JsonSerializerOptions>` | `SystemTextJsonSerializer` | Singleton |
| `JsonSerializerOptions` | Provided instance or `JsonSerializerOptions.Default` | Singleton |

## Usage Examples

### Injecting the abstraction

```csharp
public class ReportService(ISerializer serializer)
{
    public string Serialise(Report report) => serializer.Serialize(report);
    public Report? Deserialise(string json) => serializer.Deserialize<Report>(json);
}
```

### Injecting the settings-aware interface

```csharp
public class ConfigurableWriter(ISerializer<JsonSerializerOptions> serializer)
{
    public string WriteIndented(object obj) =>
        serializer.Serialize(obj, opts => opts.WriteIndented = true);
}
```

### Injecting the async interface

```csharp
public class StreamingHandler(IAsyncSerializer serializer)
{
    public async Task WriteAsync(object obj, Stream stream, CancellationToken ct = default)
        => await serializer.SerializeAsync(stream, obj, ct);
}
```

## Related Libraries

- [Ploch.Common.Serialization](common-serialization.md) — interface definitions
- [Ploch.Common.Serialization.SystemTextJson](common-serialization-system-text-json.md) — the serialiser this library registers
- [Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection](common-serialization-newtonsoft-json-di.md) — equivalent registration for Newtonsoft.Json
