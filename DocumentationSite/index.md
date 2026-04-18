# Ploch.Common — .NET Utility Libraries

A suite of .NET utility libraries targeting `netstandard2.0` and `net8.0+`, providing extension methods, helpers, and abstractions that simplify everyday development.

> [!NOTE]
> Full per-library documentation is available in the [docs folder](https://github.com/mrploch/ploch-common/tree/master/docs) of the repository. Integration with this site is tracked in [issue #190](https://github.com/mrploch/ploch-common/issues/190).

## Install

```bash
dotnet add package Ploch.Common
```

Packages are published to [NuGet.org](https://www.nuget.org/profiles/mrploch); prerelease builds go to GitHub Packages on every push to `master`.

## What's included

### Core foundation

- **`Ploch.Common`** — string, collection, type, path, enum, and reflection extensions; guard clauses; randomisers.
- **`Ploch.Common.Net9`** — `.NET 9+`-specific helpers (`AppDomainTypesLoader`).
- **`Ploch.Common.DataAnnotations`** — custom validation attributes (e.g. `RequiredNotDefaultDate`).
- **`Ploch.Common.ObjectBuilder`** — WMI/COM object-to-POCO conversion pipeline.
- **`Ploch.Common.Ardalis.Result`** — HTTP status-code mapping for `Ardalis.Result`.

### Serialisation

Abstract `ISerializer` / `IAsyncSerializer` interfaces plus implementations:

- **`Ploch.Common.Serialization.SystemTextJson`** (sync + async)
- **`Ploch.Common.Serialization.NewtonsoftJson`** (sync)
- Both ship with `.ExtensionsDependencyInjection` companions for DI registration.

### Dependency injection

- **`Ploch.Common.DependencyInjection`** — the `ServicesBundle` pattern for modular, ordered DI registration.
- **`Ploch.Common.DependencyInjection.Autofac`** — Autofac container integration.
- **`Ploch.Common.DependencyInjection.Hosting`** — `IHostBuilder` integration.
- **`Ploch.Common.Extensions.Configuration`** — configuration section binding helpers.

### Web, API, and applications

- **`Ploch.Common.WebApi`** — generic CRUD endpoint builders (EF Core + AutoMapper + FastEndpoints).
- **`Ploch.Common.Web`** — Swagger/OpenAPI configuration helpers.
- **`Ploch.Common.WebUI`** — Razor/MVC page utilities.
- **`Ploch.Common.AppServices`** + **`.Web`** — application service abstractions and HTTP context providers.
- **`Ploch.Common.Apps`** — priority-based action/command handler framework.
- **`Ploch.Common.NSwag`** — NSwag operation-name generator.
- **`Ploch.Common.Maui`** — MAUI MVVM bases, view discovery, font management.

### Testing support

- **`Ploch.TestingSupport`** — base test utilities, data attributes, `FluentVerifier`.
- **`Ploch.TestingSupport.XUnit3`** — xUnit v3 integration with custom data attributes.
- **`Ploch.TestingSupport.XUnit3.AutoMoq`** — AutoFixture + Moq integration.
- **`Ploch.TestingSupport.FluentAssertions`** (+ `.IOAbstractions`) — custom FluentAssertions extensions.

## Navigating the docs

- [**API**](api/index.md) — auto-generated reference for every public type.
- [**Articles**](articles/intro.md) — worked examples and usage patterns.
- [**README** on GitHub](https://github.com/mrploch/ploch-common#readme) — comprehensive code examples and real-world scenarios.

## Links

- [GitHub repository](https://github.com/mrploch/ploch-common)
- [Issue tracker](https://github.com/mrploch/ploch-common/issues)
- [NuGet packages](https://www.nuget.org/profiles/mrploch)
