# Ploch.Common.Serialization.SystemTextJson

> `System.Text.Json` implementation of the Ploch serialisation abstractions, with full async stream support.

## Overview

`Ploch.Common.Serialization.SystemTextJson` provides `SystemTextJsonSerializer`, a concrete implementation of `IAsyncSerializer<JsonSerializerOptions>` backed by the `System.Text.Json` library that ships with .NET. Because it implements the full interface hierarchy (`ISerializer`, `ISerializer<JsonSerializerOptions>`, `IAsyncSerializer`, `IAsyncSerializer<JsonSerializerOptions>`), it can be registered once and injected under any of those contracts.

The serialiser is constructed with a `JsonSerializerOptions` instance that acts as the default configuration. Per-call overrides can be applied without mutating the shared options, via the `Action<JsonSerializerOptions>?` overloads inherited from `ISerializer<TSettings>` and `IAsyncSerializer<TSettings>`.

The library targets `netstandard2.0` and pulls in the `System.Text.Json` NuGet package, making it available on .NET Framework and all modern .NET runtimes.

## Installation

```shell
dotnet add package Ploch.Common.Serialization.SystemTextJson
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `SystemTextJsonSerializer` | Class | Concrete serialiser backed by `System.Text.Json`. Implements `IAsyncSerializer<JsonSerializerOptions>` (and all narrower interfaces in the hierarchy). |

## Usage Examples

### Direct instantiation with default options

```csharp
var serializer = new SystemTextJsonSerializer();

string json = serializer.Serialize(new { Name = "Alice", Age = 30 });
MyDto? dto = serializer.Deserialize<MyDto>(json);
```

### Instantiation with custom options

```csharp
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};

var serializer = new SystemTextJsonSerializer(options);
```

### Per-call settings override

The settings-aware overloads apply the configuration action on top of the instance's options for that call:

```csharp
ISerializer<JsonSerializerOptions> serializer = new SystemTextJsonSerializer();

// Serialise with indented output for this call only
string pretty = serializer.Serialize(report, opts => opts.WriteIndented = true);
```

### Async stream serialisation

```csharp
IAsyncSerializer serializer = new SystemTextJsonSerializer();

// Write to a stream
await using var stream = File.OpenWrite("data.json");
await serializer.SerializeAsync(stream, myObject, cancellationToken);

// Read from a stream
await using var readStream = File.OpenRead("data.json");
MyObject? result = await serializer.DeserializeAsync<MyObject>(readStream, cancellationToken);
```

### Converting embedded `JsonElement` values

When a property in a deserialised object is typed as `object`, `System.Text.Json` returns a `JsonElement`. The `Convert` method handles re-mapping it to a concrete type:

```csharp
ISerializer serializer = new SystemTextJsonSerializer();

// 'raw' will be a JsonElement when the target type is object
object? raw = serializer.Deserialize("{\"count\":42}", typeof(object));
MyModel? typed = serializer.Convert<MyModel>(raw);
```

## Configuration

For dependency injection registration see [Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection](common-serialization-system-text-json-di.md).

When constructing `SystemTextJsonSerializer` manually, pass a configured `JsonSerializerOptions`:

```csharp
var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

var serializer = new SystemTextJsonSerializer(options);
```

> Note: the `GetSettings` implementation mutates the shared `_options` instance by applying the configuration action and setting `UnknownTypeHandling = JsonElement`. Avoid sharing a `JsonSerializerOptions` instance across multiple serialisers if per-call mutations could interfere.

## Related Libraries

- [Ploch.Common.Serialization](common-serialization.md) — abstractions this library implements
- [Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection](common-serialization-system-text-json-di.md) — DI registration
- [Ploch.Common.Serialization.NewtonsoftJson](common-serialization-newtonsoft-json.md) — alternative Newtonsoft.Json implementation
