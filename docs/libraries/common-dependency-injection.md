# Ploch.Common.DependencyInjection

> Modular service registration pattern for `Microsoft.Extensions.DependencyInjection`.

## Overview

`Ploch.Common.DependencyInjection` introduces the **ServicesBundle** pattern: a structured way to organise DI registrations into cohesive, reusable units. The concept is directly analogous to [Autofac Modules](https://autofaccn.readthedocs.io/en/latest/configuration/modules.html), applied to the standard `Microsoft.Extensions.DependencyInjection` container.

Each bundle encapsulates all the `IServiceCollection` registrations for a feature or layer. Bundles can declare ordered dependencies on other bundles, ensuring that foundational registrations always run before dependent ones — without the calling code needing to know the order.

The library also provides `ScopedService<T>`, a small utility that resolves a service inside its own dedicated `IServiceScope` and disposes the scope alongside itself.

## Installation

```
dotnet add package Ploch.Common.DependencyInjection
```

The package targets `netstandard2.0` and is compatible with .NET Framework 4.6.1+, .NET Core 2.0+, and all modern .NET versions.

## Key Types

| Type | Kind | Description |
|------|------|-------------|
| `IServicesBundle` | Interface | Core contract: `Configure(IServiceCollection services)`. Also exposes an optional `Configuration` property (`IConfiguration?`) for bundles that need access to application configuration. |
| `ServicesBundle` | Abstract class | Base implementation; subclasses override `DoConfigure()` |
| `ConfigurableServicesBundle` | Abstract class | Variant that requires `IConfiguration`; subclasses override `Configure(IConfiguration)` |
| `DelegatingServicesBundle` | Concrete class | Ad-hoc bundle built from delegate actions rather than a subclass. **Warning:** inherits from `ConfigurableServicesBundle`, so omitting `IConfiguration` when registering will throw `InvalidOperationException` at runtime. |
| `ServicesBundleRegistration` | Static class | Extension methods: `AddServicesBundle(...)` on `IServiceCollection` |
| `IConfigurationConsumer` | Interface | Write-only `Configuration` setter; used internally when injecting config |
| `IOptionalConfigurationProvider` | Interface | Read-only `Configuration` getter; implemented by `IServicesBundle` |
| `IScopedService` / `IScopedService<T>` | Interfaces | Scoped service wrapper contract |
| `ScopedService` / `ScopedService<T>` | Concrete classes | Creates and owns a scope; disposes it on `Dispose()` / `DisposeAsync()` |

## Usage Examples

### Basic bundle

Define a bundle by inheriting `ServicesBundle` and implementing `DoConfigure()`:

```csharp
public class LoggingBundle : ServicesBundle
{
    public override void DoConfigure()
    {
        Services.AddSingleton<ILoggerFactory, LoggerFactory>();
        Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
    }
}
```

Register it during application startup:

```csharp
services.AddServicesBundle<LoggingBundle>();
// or with an instance
services.AddServicesBundle(new LoggingBundle());
```

### Bundle with dependencies

Declare dependencies so they are configured before the current bundle runs:

```csharp
public class ApplicationBundle : ServicesBundle
{
    protected override IEnumerable<IServicesBundle>? Dependencies { get; } =
        [new LoggingBundle(), new DatabaseBundle()];

    public override void DoConfigure()
    {
        Services.AddScoped<IApplicationService, ApplicationService>();
    }
}
```

### Configuration-aware bundle

Inherit `ConfigurableServicesBundle` when registration logic depends on `IConfiguration`:

```csharp
public class DatabaseBundle : ConfigurableServicesBundle
{
    protected override void Configure(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(connectionString));
    }
}
```

Pass configuration when registering:

```csharp
services.AddServicesBundle<DatabaseBundle>(configuration);
// or
services.AddServicesBundle(new DatabaseBundle(), configuration);
```

### Inline delegate bundle

Use `DelegatingServicesBundle` for ad-hoc registration without creating a subclass:

```csharp
var bundle = new DelegatingServicesBundle()
    .Configure((services, config) => services.AddScoped<IMyService, MyService>())
    .Configure((services, config) =>
    {
        if (config?["Feature:Enabled"] == "true")
            services.AddSingleton<IFeatureService, FeatureService>();
    });

services.AddServicesBundle(bundle, configuration);
```

### Scoped service wrapper

Use `ScopedService<T>` when a singleton needs to resolve a scoped dependency on demand:

```csharp
public class BackgroundProcessor(IServiceProvider provider)
{
    public async Task ProcessAsync(CancellationToken ct)
    {
        await using var scoped = new ScopedService<IWorkItemRepository>(provider);
        var items = await scoped.Service.GetPendingAsync(ct);
        // ...
    }
}
```

## Configuration

No additional configuration is required. The two `AddServicesBundle` extension methods on `IServiceCollection` cover both common call sites:

```csharp
// By type (requires a public parameterless constructor)
services.AddServicesBundle<TBundle>(configuration);

// By instance
services.AddServicesBundle(bundleInstance, configuration);
```

Both overloads return `IServiceCollection` for fluent chaining.

## Related Libraries

- [Ploch.Common.DependencyInjection.Autofac](common-dependency-injection-autofac.md) — Autofac container integration for ServicesBundle
- [Ploch.Common.DependencyInjection.Hosting](common-dependency-injection-hosting.md) — `IHostBuilder` extension for registering bundles during Generic Host startup
- [Ploch.Common.Extensions.Configuration](common-extensions-configuration.md) — Extension methods for binding configuration sections to options classes
