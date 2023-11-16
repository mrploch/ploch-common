# Ploch.Common.Serialization

## Overview

`Ploch.Common.Serialization` library provides a common interface for serialization and deserialization of objects.

Library also provides conversion of embedded `object` types.

## Rationale

The main reason for this library is to provide a common interface for serialization and deserialization of objects using
different serializers.

For example, if you want to switch from `Newtonsoft.Json` to `System.Text.Json` or `YamlDotNet` you would have to
re-implement all the serialization and deserialization logic. This library provides a way to avoid it.

Reasons for switching serializers may vary, but one of the examples is the support in `System.Text.Json` for a feature
that was not previously supported.

At the moment, the following serializers support is implemented:

- [Newtonsoft Json.NET](https://www.newtonsoft.com/json) in the
  
  - [Newtonsoft Json.NET](https://www.newtonsoft.com/json) in the
    [Ploch.Common.Serialization.NewtonsoftJson](../Common.Serialization.NewtonsoftJson/README.md) package,
  
  - [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json) in the
    [Ploch.Common.Serialization.SystemTextJson](../Common.Serialization.SystemTextJson/README.md) package,
- [System.Text.Json](https://docs.microsoft.com/en-us/dotnet/api/system.text.json) in the
  [Ploch.Common.Serialization.SystemTextJson](../Common.Serialization.SystemTextJson/README.md) package,

## Usage

Usage is quite straightforward:

You create an instance of the concrete serializer, optionally passing the settings, then call its methods:

```csharp
var serializer = new ...;

var serialized = serializer.Serialize(yourObject);

var deserialized = serializer.Deserialize<YourType>(serialized);
```

To convert an embedded object type, you call the `Convert` method on the serializer instance.

For example:

```csharp
// You have a type with an object property:
public record YourType(string SomeStrProperty, object SomeObjectProperty);

// You know that this type should be deserialized to a specific type:

var deserializedData = serializer.Convert<YourConcreteTypeOfObject>(deserialized.SomeObjectProperty);
```