# Ploch.Common.AppServices

> Abstractions for common cross-cutting application services, starting with user identity resolution.

## Overview

`Ploch.Common.AppServices` defines the service contracts that application code depends on for platform-independent operations. The package intentionally contains only interfaces, keeping it free of any runtime dependency (no ASP.NET Core, no MAUI, no WPF). Concrete implementations live in separate companion packages such as `Ploch.Common.AppServices.Web`.

The current release exposes a single abstraction: `IUserInfoProvider`, which returns the current user as a `ClaimsPrincipal`. This allows business logic and use-case classes to resolve the acting user without coupling to `HttpContext`, `IHttpContextAccessor`, or any other host-specific type.

Additional cross-cutting service abstractions are expected to be added to this package over time, following the same pattern of interface-only definitions with implementations in companion packages.

## Installation

```shell
dotnet add package Ploch.Common.AppServices
```

## Key Types

| Type | Kind | Description |
|---|---|---|
| `IUserInfoProvider` | Interface | `GetCurrentUserInfo()` — returns the current `ClaimsPrincipal?` or `null` if no user is authenticated. Located in the `Ploch.Common.AppServices.Security` namespace. |

## Usage Examples

### Consuming the abstraction in a use case

```csharp
public class CreateBlogPostUseCase(
    IReadWriteRepositoryAsync<BlogPost, int> repository,
    IUserInfoProvider userInfoProvider)
{
    public async Task<BlogPost> ExecuteAsync(CreateBlogPostRequest request, CancellationToken ct)
    {
        var user = userInfoProvider.GetCurrentUserInfo();
        var authorName = user?.Identity?.Name ?? "Anonymous";

        var post = new BlogPost
        {
            Title = request.Title,
            Contents = request.Contents,
            Author = authorName
        };

        return await repository.AddAsync(post, ct);
    }
}
```

### Registering a custom implementation

For environments other than ASP.NET Core (e.g. a background service with a thread-local principal), provide your own implementation:

```csharp
public class ThreadPrincipalUserInfoProvider : IUserInfoProvider
{
    public ClaimsPrincipal? GetCurrentUserInfo() => Thread.CurrentPrincipal as ClaimsPrincipal;
}

// Registration:
services.AddSingleton<IUserInfoProvider, ThreadPrincipalUserInfoProvider>();
```

## Related Libraries

- [Ploch.Common.AppServices.Web](common-appservices-web.md) — ASP.NET Core implementation of `IUserInfoProvider` via `IHttpContextAccessor`
