# Ploch.Common.Serialization.NewtonsoftJson

## Overview

`Ploch.Common.Serialization.NewtonsoftJson` package provides a concrete implementation of the `ISerializer` interface
for
the [Newtonsoft Json.NET](https://www.newtonsoft.com/json) serializer.

To create a concrete instance of the serializer, you can use the `NewtonsoftJsonSerializer` class:

```csharp
var serializer = new NewtonsoftJsonSerializer();
```

To register this serializer with the dependency injection container:

```csharp
ser
```