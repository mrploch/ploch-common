# Ploch.Common.Serialization.NewtonsoftJson

> `Newtonsoft.Json` implementation of the Ploch serialisation abstractions.

## Overview

`Ploch.Common.Serialization.NewtonsoftJson` provides `NewtonsoftJsonObjectSerializer`, a concrete implementation of `ISerializer<JsonSerializerSettings>` backed by the well-established `Newtonsoft.Json` library. It is the right choice when an application already depends on Newtonsoft.Json, requires its richer feature set (custom converters, `JsonPath` querying, reference handling, etc.), or needs compatibility with legacy code that produces Newtonsoft-formatted JSON.

The serialiser is constructed with a `JsonSerializerSettings` instance that serves as the default configuration. Per-call overrides can be supplied via the `Action<JsonSerializerSettings>?` overloads inherited from `ISerializer<TSettings>`.

Unlike the `SystemTextJsonSerializer`, `NewtonsoftJsonObjectSerializer` does not implement `IAsyncSerializer` — Newtonsoft.Json does not expose native stream-based async serialisation. If async stream support is required, use the `System.Text.Json` variant instead.

The library targets `netstandard2.0`.

## Installation

```shell
dotnet add package Ploch.Common.Serialization.NewtonsoftJson
```

> Note: the published NuGet package ID uses the legacy name `Ploch.Common.Serialiation.NewtonsoftJson` (missing the `z` — "Serialiation" instead of "Serialization"). Verify the exact name in your package source when adding the reference.

## Key Types

| Type | Kind | Description |
|---|---|---|
| `NewtonsoftJsonObjectSerializer` | Class | Concrete serialiser backed by `Newtonsoft.Json`. Implements `ISerializer<JsonSerializerSettings>` (and the base `ISerializer`). The intermediate object type used by `Convert` is `JObject`. |

## Usage Examples

### Direct instantiation with default settings

```csharp
var serializer = new NewtonsoftJsonObjectSerializer();

string json = serializer.Serialize(new { Name = "Alice", Age = 30 });
MyDto? dto = serializer.Deserialize<MyDto>(json);
```

### Instantiation with custom settings

```csharp
var settings = new JsonSerializerSettings
{
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore,
    ContractResolver = new CamelCasePropertyNamesContractResolver()
};

var serializer = new NewtonsoftJsonObjectSerializer(settings);
```

### Per-call settings override

The settings-aware overloads apply the configuration action on top of the instance's settings for that call:

```csharp
ISerializer<JsonSerializerSettings> serializer = new NewtonsoftJsonObjectSerializer();

string compact = serializer.Serialize(report, s => s.Formatting = Formatting.None);
string indented = serializer.Serialize(report, s => s.Formatting = Formatting.Indented);
```

### Deserialisation by runtime type

```csharp
ISerializer serializer = new NewtonsoftJsonObjectSerializer();

Type targetType = typeof(MyDto);
object? result = serializer.Deserialize(json, targetType);
```

### Converting embedded `JObject` values

When a property in a deserialised object is typed as `object`, Newtonsoft.Json returns a `JObject`. The `Convert` method re-maps it to a concrete type using `JObject.ToObject`:

```csharp
ISerializer serializer = new NewtonsoftJsonObjectSerializer();

object? raw = serializer.Deserialize(json, typeof(object));
MyModel? typed = serializer.Convert<MyModel>(raw);
```

## Related Libraries

- [Ploch.Common.Serialization](common-serialization.md) — abstractions this library implements
- [Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection](common-serialization-newtonsoft-json-di.md) — DI registration
- [Ploch.Common.Serialization.SystemTextJson](common-serialization-system-text-json.md) — alternative `System.Text.Json` implementation with async stream support
