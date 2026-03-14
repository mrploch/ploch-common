# Ploch.Common.AppServices.Web

> ASP.NET Core implementation of the `IUserInfoProvider` abstraction from `Ploch.Common.AppServices`.

## Overview

`Ploch.Common.AppServices.Web` provides the ASP.NET Core-specific implementation of the `IUserInfoProvider` interface defined in `Ploch.Common.AppServices`. It resolves the current user from the active `HttpContext` via `IHttpContextAccessor`.

The package is intentionally thin — a single implementation class and a single DI registration extension method — so application code that depends only on `IUserInfoProvider` remains portable across host environments.

## Installation

```shell
dotnet add package Ploch.Common.AppServices.Web
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `HttpContextUserInfoProvider` | Class | Implements `IUserInfoProvider` (from `Ploch.Common.AppServices.Security`). Returns `httpContext.User` from the current `IHttpContextAccessor`. |
| `ServiceCollectionRegistrations` | Static class | `AddUserInfoProvider()` extension method — registers `HttpContextUserInfoProvider` as a singleton `IUserInfoProvider`. Does **not** register `IHttpContextAccessor` itself. |

## Usage Examples

### Registering the provider

```csharp
// Program.cs
builder.Services.AddUserInfoProvider();
```

This maps `IUserInfoProvider` (from `Ploch.Common.AppServices.Security`) to `HttpContextUserInfoProvider` as a singleton. Note: this method does **not** register `IHttpContextAccessor` — it assumes it is already available. If you encounter a missing registration, add it explicitly (see Configuration section below).

### Consuming via the abstraction

After registration, inject `IUserInfoProvider` anywhere in the application without referencing ASP.NET Core types directly:

```csharp
public class AuditService(IUserInfoProvider userInfoProvider, IAuditRepository repository)
{
    public async Task LogAsync(string action, CancellationToken ct)
    {
        var user = userInfoProvider.GetCurrentUserInfo();
        var entry = new AuditEntry
        {
            Action = action,
            PerformedBy = user?.Identity?.Name ?? "system",
            Timestamp = DateTimeOffset.UtcNow
        };
        await repository.AddAsync(entry, ct);
    }
}
```

## Configuration

No additional configuration is needed. `IHttpContextAccessor` is made available by the ASP.NET Core framework when calling `AddHttpContextAccessor()`, which most minimal API and MVC setups already include. If you encounter a missing registration, add it explicitly:

```csharp
builder.Services.AddHttpContextAccessor();
builder.Services.AddUserInfoProvider();
```

## Related Libraries

- [Ploch.Common.AppServices](common-appservices.md) — `IUserInfoProvider` interface and the rationale for the abstraction
