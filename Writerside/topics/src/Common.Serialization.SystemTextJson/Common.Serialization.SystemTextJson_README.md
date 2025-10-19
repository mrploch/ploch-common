# Ploch.Common.Serialization.SystemTextJson

## Overview

`Ploch.Common.Serialization.SystemTextJson` package provides a concrete implementation of the `ISerializer` interface
for
the [System.Text.Json](https://www.nuget.org/packages/System.Text.Json) serializer.

To create a concrete instance of the serializer, you can use the `SystemTextJsonSerializer` class:

```csharp
var serializer = new NewtonsoftJsonSerializer();
var json = _serializer.Serialize(new { Foo = "Bar" });
```
