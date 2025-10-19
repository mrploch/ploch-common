# Ploch.Common.Serialization.NewtonsoftJson.ExtensionsDependencyInjection

## Overview

This library provides extensions for registering the
[Ploch.Common.Serialization.NewtonsoftJson](../Common.Serialization.NewtonsoftJson) in the
[IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-7.0&viewFallbackFrom=netstandard-2.0).

## Usage

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddNewtonsoftJsonSerializer();
} 
```

Serializer can be referenced as:

- `ISerializer`
- `ISerializer<JsonSerializerSettings>`

```csharp
public class MyService
{
    private readonly ISerializer _serializer;

    public MyService(ISerializer serializer)
    {
        _serializer = serializer;
    }

    public void DoSomething()
    {
        var json = _serializer.Serialize(new { Foo = "Bar" });
    }
}
```