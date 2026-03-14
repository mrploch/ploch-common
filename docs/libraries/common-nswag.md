# Ploch.Common.NSwag

> A custom NSwag `IOperationNameGenerator` that produces clean, tag-based operation names for generated API clients.

## Overview

`Ploch.Common.NSwag` contains `CustomNameGenrator`, an implementation of NSwag's `IOperationNameGenerator` interface. When NSwag generates C# or TypeScript client code from an OpenAPI document, it needs to produce a unique method name for each operation. The default generator often creates verbose or collision-prone names; `CustomNameGenrator` derives a PascalCase name from the HTTP method and the first OpenAPI tag on the operation (typically the controller or entity name).

The `SupportsMultipleClients` property is `false` by default, meaning all operations are emitted into a single monolithic client class rather than split across per-tag clients.

> **Note:** despite living in the `Ploch.Common.NSwag` package, the class is currently in the `Ploch.Lists.Api.WebApi` namespace. This may be moved to `Ploch.Common.NSwag` in a future release.

## Installation

```shell
dotnet add package Ploch.Common.NSwag
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `CustomNameGenrator` | Class | Implements `IOperationNameGenerator`. `GetClientName` returns `operation.Tags[0]`; `GetOperationName` returns `{httpMethod}{Tags[0]}` converted to PascalCase. |

> Note: the class name `CustomNameGenrator` reflects the current source spelling.

## Usage Examples

### Wiring up the generator in a NSwag code generation pipeline

```csharp
var document = await OpenApiDocument.FromUrlAsync("https://myapi/swagger/v1/swagger.json");

var settings = new CSharpClientGeneratorSettings
{
    OperationNameGenerator = new CustomNameGenrator()
};

var generator = new CSharpClientGenerator(document, settings);
var code = generator.GenerateFile();
File.WriteAllText("ApiClient.cs", code);
```

### Expected output

Given an operation `GET /api/blogpost/{id}` tagged `BlogPost`, the generator produces:

- Client name: `BlogPost`
- Operation name: `GetBlogPost`

## Related Libraries

- [Ploch.Common.Web](common-web.md) — Swagger/OpenAPI configuration for ASP.NET Core hosts
- [Ploch.Common.WebApi](common-webapi.md) — Server-side OpenAPI configuration and CRUD endpoint infrastructure
