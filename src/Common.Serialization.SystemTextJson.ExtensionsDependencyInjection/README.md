# Ploch.Common.Serialization.SystemTextJson.ExtensionsDependencyInjection

## Overview

This library provides extensions for registering the
[Ploch.Common.Serialization.SystemTextJson](../Common.Serialization.SystemTextJson) in the
[IServiceCollection](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicecollection?view=dotnet-plat-ext-7.0&viewFallbackFrom=netstandard-2.0).

## Usage

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSystemTextJsonSerializer();
} 
```

Serializer can be referenced as:

- `ISerializer`
- `ISerializer<JsonSerializerOptions>`
- `IAsyncSerializer`
- `IAsyncSerializer<JsonSerializerOptions>`

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

public class MyAsyncService
{
    private readonly IAsyncSerializer _serializer;

    public MyAsyncService(IAsyncSerializer serializer)
    {
        _serializer = serializer;
    }

    public async Task DoSomethingAsync()
    {
        using FileStream createStream = File.Create("fileName.json");
        await _serializer.SerializeAsync(createStream, new { Foo = "Bar" });
        await createStream.DisposeAsync();
    }
}
```