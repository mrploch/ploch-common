# Ploch.Common.Web

> Swagger/OpenAPI configuration helpers for ASP.NET Core Web API projects using traditional MVC controllers.

## Overview

`Ploch.Common.Web` is a thin utility package that provides extension methods for wiring up Swagger/OpenAPI documentation in an ASP.NET Core application. It wraps Swashbuckle configuration with sensible defaults and keeps the setup in a single, reusable call pair.

The package is intentionally narrow: it does not introduce middleware beyond what Swashbuckle requires, and it does not duplicate the more advanced `OpenApiConfigurator` in `Ploch.Common.WebApi`. Use this package when you have a standard MVC controller API and want Swagger UI configured with minimal ceremony.

## Installation

> **Note:** This module does not have its own `.csproj` or NuGet package. The source files live under `src/Common.Web/` and are compiled as part of the consuming project or a parent package. Reference the source directly or use `Ploch.Common.WebApi` for a packaged alternative.

## Key Types

| Type | Kind | Description |
|---|---|---|
| `ApiSwaggerConfig` | Static class | Two extension methods: `ConfigureOpenApiContractGeneratorServices` (DI registration) and `ConfigureOpenApiContractGeneratorApp` (middleware registration). Located in namespace `Ploch.Common.Web.WebApi.Configuration`. |

## Usage Examples

### Registering Swagger services

Call `ConfigureOpenApiContractGeneratorServices` in `Program.cs` during service registration:

```csharp
builder.Services.ConfigureOpenApiContractGeneratorServices("v1", new OpenApiInfo
{
    Title = "My API",
    Version = "v1",
    Description = "My API description"
});
```

This call registers `AddEndpointsApiExplorer()`, `AddSwaggerGen(...)`, and enables Swagger annotations.

### Enabling Swagger middleware

Call `ConfigureOpenApiContractGeneratorApp` on the built application to activate the Swagger UI:

```csharp
var app = builder.Build();

app.ConfigureOpenApiContractGeneratorApp();
// Equivalent to: app.UseSwagger(); app.UseSwaggerUI();

app.Run();
```

## Related Libraries

- [Ploch.Common.WebApi](common-webapi.md) — Advanced OpenAPI configuration (`OpenApiConfigurator`) and CRUD endpoint infrastructure for Minimal API and FastEndpoints
- [Ploch.Common.NSwag](common-nswag.md) — NSwag-based name generation utilities
