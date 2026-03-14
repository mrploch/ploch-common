# Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection

> DI registration helpers for wiring `NewtonsoftJsonObjectSerializer` into a `Microsoft.Extensions.DependencyInjection` container.

## Overview

This library provides an `IServiceCollection` extension method for registering `NewtonsoftJsonObjectSerializer` as a singleton under the standard serialisation interfaces. It is the DI counterpart to `Ploch.Common.Serialization.NewtonsoftJson` and follows the same registration pattern as the `System.Text.Json` DI package.

After registration the following interfaces resolve to the same singleton `NewtonsoftJsonObjectSerializer` instance:

- `ISerializer`
- `ISerializer<JsonSerializerSettings>`

> Note: `IAsyncSerializer` is **not** registered because `NewtonsoftJsonObjectSerializer` does not implement stream-based async serialisation. If `IAsyncSerializer` injection is required, use the `System.Text.Json` DI package instead.

The library targets `netstandard2.0` and depends on `Microsoft.Extensions.DependencyInjection.Abstractions`.

## Installation

```shell
dotnet add package Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `NewtonsoftJsonSerializerRegistration` | Static class | Provides the `AddNewtonsoftJsonSerializer` extension method on `IServiceCollection`. |

## Configuration

### Using the extension method

```csharp
// Startup / Program.cs
services.AddNewtonsoftJsonSerializer();
```

With custom settings:

```csharp
var settings = new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

services.AddNewtonsoftJsonSerializer(settings);
```

### Registered services after `AddNewtonsoftJsonSerializer`

| Service type | Implementation | Lifetime |
|---|---|---|
| `ISerializer` | `NewtonsoftJsonObjectSerializer` | Singleton |
| `ISerializer<JsonSerializerSettings>` | `NewtonsoftJsonObjectSerializer` | Singleton |

The serialiser instance is constructed eagerly at registration time with the supplied `JsonSerializerSettings` (or a default instance if none is provided), then registered as a singleton for both interfaces.

> **Note:** Unlike the System.Text.Json DI package, this package does not provide a `ServicesBundle` subclass. To use it as a dependency within another bundle, call `AddNewtonsoftJsonSerializer()` on the `Services` collection inside the bundle's `DoConfigure()` method.

## Usage Examples

### Injecting the base abstraction

```csharp
public class ReportService(ISerializer serializer)
{
    public string Serialise(Report report) => serializer.Serialize(report);
    public Report? Deserialise(string json) => serializer.Deserialize<Report>(json);
}
```

### Injecting the settings-aware interface

```csharp
public class ConfigurableWriter(ISerializer<JsonSerializerSettings> serializer)
{
    public string WriteIndented(object obj) =>
        serializer.Serialize(obj, s => s.Formatting = Formatting.Indented);
}
```

## Related Libraries

- [Ploch.Common.Serialization](common-serialization.md) — interface definitions
- [Ploch.Common.Serialization.NewtonsoftJson](common-serialization-newtonsoft-json.md) — the serialiser this library registers
- [Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection](common-serialization-system-text-json-di.md) — equivalent registration for `System.Text.Json`, which also covers `IAsyncSerializer`
