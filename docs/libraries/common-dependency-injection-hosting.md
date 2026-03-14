# Ploch.Common.DependencyInjection.Hosting

> `IHostBuilder` extension for registering ServicesBundles during Generic Host startup.

## Overview

`Ploch.Common.DependencyInjection.Hosting` adds a single extension method on `IHostBuilder` that wires a ServicesBundle into the Generic Host's service configuration pipeline. It eliminates the need to unwrap the `HostBuilderContext` manually to access `IConfiguration` when using the [ServicesBundle pattern](common-dependency-injection.md).

This is the idiomatic integration point when configuring a .NET Generic Host (`Host.CreateDefaultBuilder`) or an ASP.NET Core application using the pre-`WebApplication` API.

## Installation

```
dotnet add package Ploch.Common.DependencyInjection.Hosting
```

The package targets `net8.0`. It depends on `Ploch.Common.DependencyInjection` and `Microsoft.Extensions.Hosting.Abstractions`.

## Key Types

| Type | Description |
|------|-------------|
| `HostBuilderBundleRegistrationExtensions` | Provides `AddServicesBundle<TBundle>(this IHostBuilder)` |

## Usage Examples

### Generic Host startup

```csharp
var host = Host.CreateDefaultBuilder(args)
    .AddServicesBundle<ApplicationBundle>()
    .AddServicesBundle<InfrastructureBundle>()
    .Build();

await host.RunAsync();
```

The extension method calls `ConfigureServices((context, services) => services.AddServicesBundle<TBundle>(context.Configuration))` internally, so `context.Configuration` is automatically threaded through to any `ConfigurableServicesBundle` that needs it.

### Combining with manual service registration

You can mix `AddServicesBundle` calls with standard `ConfigureServices` calls freely:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .AddServicesBundle<CoreBundle>()
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<MyWorker>();
    })
    .AddServicesBundle<MessagingBundle>()
    .Build();
```

### Bundle definition (for reference)

The bundle itself is written using the standard `ServicesBundle` or `ConfigurableServicesBundle` base classes:

```csharp
public class InfrastructureBundle : ConfigurableServicesBundle
{
    protected override void Configure(IConfiguration configuration)
    {
        Services.AddSingleton<IEmailSender, SmtpEmailSender>();
        Services.AddSingleton(new SmtpSettings(
            configuration["Smtp:Host"]!,
            int.Parse(configuration["Smtp:Port"]!)));
    }
}
```

## Configuration

No additional configuration. The `IConfiguration` instance is sourced automatically from the `HostBuilderContext` at the point `ConfigureServices` runs.

## Method Signature

```csharp
public static IHostBuilder AddServicesBundle<TBundle>(this IHostBuilder hostBuilder)
    where TBundle : IServicesBundle, new();
```

`TBundle` must have a public parameterless constructor, consistent with the type-based overload in `ServicesBundleRegistration`.

## Related Libraries

- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — Core ServicesBundle pattern, `ServicesBundle`, `ConfigurableServicesBundle`, and `IServiceCollection` extensions
- [Ploch.Common.DependencyInjection.Autofac](common-dependency-injection-autofac.md) — Autofac container integration
