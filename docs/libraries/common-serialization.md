# Ploch.Common.Serialization

> Serialization abstractions for .NET: a common interface hierarchy covering synchronous and asynchronous JSON (or any format) serialisation.

## Overview

`Ploch.Common.Serialization` defines the contracts and base classes that decouple application code from any specific serialisation library. Instead of taking a hard dependency on `System.Text.Json` or `Newtonsoft.Json`, consumers depend only on `ISerializer` or `IAsyncSerializer`. The concrete implementation is swapped at the composition root, making serialisation strategies interchangeable without touching business logic.

The library targets `netstandard2.0`, so it is compatible with .NET Framework 4.6.1+, .NET Core, and .NET 5+.

Two parallel interface hierarchies are provided: one for synchronous string-based serialisation and one that extends it with stream-based async operations. Both hierarchies have a settings-aware variant (`ISerializer<TSettings>`, `IAsyncSerializer<TSettings>`) that allows per-call configuration without constructing a new serialiser instance.

Abstract base classes (`Serializer<TSettings, TDataJsonObject>` and `AsyncSerializer<TSettings, TDataJsonObject>`) handle the boilerplate of forwarding settings-configured overloads to the core protected methods, so implementations only need to provide the concrete serialisation logic.

## Installation

```shell
dotnet add package Ploch.Common.Serialization
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `ISerializer` | Interface | Synchronous serialise/deserialise to/from `string`. Includes two `Convert` overloads for re-mapping embedded objects. |
| `ISerializer<TSettings>` | Interface | Extends `ISerializer` with overloads accepting `Action<TSettings>?` for per-call settings configuration. |
| `IAsyncSerializer` | Interface | Extends `ISerializer` with stream-based `SerializeAsync` / `DeserializeAsync` operations. |
| `IAsyncSerializer<TSettings>` | Interface | Combines `IAsyncSerializer` and `ISerializer<TSettings>`: full async API with per-call settings support. |
| `Serializer<TSettings, TDataJsonObject>` | Abstract class | Base implementation of `ISerializer<TSettings>`. Handles settings resolution and `Convert` logic; subclasses implement the abstract `Serialize`/`Deserialize` overloads. |
| `AsyncSerializer<TSettings, TDataJsonObject>` | Abstract class | Extends `Serializer<TSettings, TDataJsonObject>` with the async stream operations from `IAsyncSerializer<TSettings>`. |

## Usage Examples

### Injecting the abstraction

Depend on the narrowest interface that satisfies the consumer's needs:

```csharp
public class ReportExporter(ISerializer serializer)
{
    public string Export(Report report) => serializer.Serialize(report);

    public Report? Import(string json) => serializer.Deserialize<Report>(json);
}
```

### Using the settings-aware interface for per-call configuration

```csharp
public class ConfigurableExporter(ISerializer<JsonSerializerOptions> serializer)
{
    public string ExportPretty(Report report) =>
        serializer.Serialize(report, opts => opts.WriteIndented = true);
}
```

### Async stream serialisation

```csharp
public class StreamingService(IAsyncSerializer serializer)
{
    public async Task WriteAsync(object obj, Stream destination, CancellationToken ct = default)
        => await serializer.SerializeAsync(destination, obj, ct);

    public async ValueTask<T?> ReadAsync<T>(Stream source, CancellationToken ct = default)
        => await serializer.DeserializeAsync<T>(source, ct);
}
```

### Converting embedded objects

The `Convert` method is useful when deserialising polymorphic JSON where an inner property arrives as an intermediate representation (e.g. `JsonElement` or `JObject`):

```csharp
object? raw = serializer.Deserialize(json, typeof(object));
MyType? typed = serializer.Convert<MyType>(raw);
```

### Implementing a custom serialiser

Inherit from `Serializer<TSettings, TDataJsonObject>` and implement the eight abstract members:

```csharp
public class MySerializer : Serializer<MyOptions, MyIntermediateObject>
{
    // Public abstract overrides
    public override string Serialize(object obj) { /* ... */ }
    public override object? Deserialize(string serializedObj, Type type) { /* ... */ }
    public override TTargetType? Deserialize<TTargetType>(string serializedObj) { /* ... */ }

    // Protected abstract overrides
    protected override string Serialize(object obj, MyOptions settings) { /* ... */ }
    protected override object? Deserialize(string serializedObj, Type type, MyOptions settings) { /* ... */ }
    protected override TTargetType? Deserialize<TTargetType>(string serializedObj, MyOptions settings) { /* ... */ }
    protected override object? DeserializeObject(MyIntermediateObject jsonObject, Type targetType) { /* ... */ }
    protected override MyOptions GetSettings(Action<MyOptions>? configuration) { /* ... */ }
}
```

For async support, inherit from `AsyncSerializer<TSettings, TDataJsonObject>` instead and add the three async overloads.

## Related Libraries

- [Ploch.Common.Serialization.SystemTextJson](common-serialization-system-text-json.md) — `System.Text.Json` implementation
- [Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection](common-serialization-system-text-json-di.md) — DI registration for the `System.Text.Json` serialiser
- [Ploch.Common.Serialization.NewtonsoftJson](common-serialization-newtonsoft-json.md) — `Newtonsoft.Json` implementation
- [Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection](common-serialization-newtonsoft-json-di.md) — DI registration for the Newtonsoft serialiser
