# Ploch.Common.DependencyInjection.Autofac

> Autofac container integration for the ServicesBundle pattern.

> **This package is currently a stub.** The Autofac project file (`Plocch.Common.DependencyInjection.Autofac.csproj` — note the typo in the filename) references `Ploch.Common.DependencyInjection` and `Autofac.Extensions.DependencyInjection` but contains no additional source files beyond the project definition. The usage examples below are aspirational/planned and are not based on existing source code. Check the repository for the current state before depending on this package in production.

## Overview

`Ploch.Common.DependencyInjection.Autofac` bridges the ServicesBundle pattern from [Ploch.Common.DependencyInjection](common-dependency-injection.md) with the [Autofac](https://autofac.org/) IoC container via `Autofac.Extensions.DependencyInjection`.

When your application uses Autofac as its DI container (typically registered via `UseServiceProviderFactory(new AutofacServiceProviderFactory())`), this package enables the same `AddServicesBundle` call pattern to work against Autofac's `ContainerBuilder` / `IServiceCollection` bridge rather than the bare Microsoft DI container.

## Installation

```
dotnet add package Ploch.Common.DependencyInjection.Autofac
```

## Key Types

This package re-exports the full ServicesBundle API from `Ploch.Common.DependencyInjection` and provides Autofac-compatible registration support through the `Autofac.Extensions.DependencyInjection` bridge.

| Type | Source | Description |
|------|--------|-------------|
| `IServicesBundle` | `Ploch.Common.DependencyInjection` | Core bundle interface |
| `ServicesBundle` | `Ploch.Common.DependencyInjection` | Base class for bundles |
| `ConfigurableServicesBundle` | `Ploch.Common.DependencyInjection` | Configuration-aware base class |
| `AutofacServiceProviderFactory` | `Autofac.Extensions.DependencyInjection` | Registers Autofac as the service provider factory |

## Usage Examples

### Generic Host with Autofac and ServicesBundle

Register Autofac as the container factory in the Generic Host, then use bundles as normal:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureServices((context, services) =>
    {
        services.AddServicesBundle<ApplicationBundle>(context.Configuration);
        services.AddServicesBundle<InfrastructureBundle>(context.Configuration);
    })
    .Build();
```

Bundles are written identically to the standard ServicesBundle pattern — the Autofac package handles the container wiring transparently.

### ASP.NET Core with Autofac

```csharp
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddServicesBundle<WebApiBundle>(builder.Configuration);
```

## Configuration

No additional configuration beyond the standard ServicesBundle pattern. Ensure Autofac is registered as the service provider factory before calling `AddServicesBundle`.

## Related Libraries

- [Ploch.Common.DependencyInjection](common-dependency-injection.md) — Core ServicesBundle pattern that this package extends
- [Ploch.Common.DependencyInjection.Hosting](common-dependency-injection-hosting.md) — `IHostBuilder` extension for bundle registration
