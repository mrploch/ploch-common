# Current-User Information — `IUserInfoProvider`

`Ploch.Common.AppServices` contains a single, deliberately small abstraction:

```csharp
namespace Ploch.Common.AppServices.Security;

public interface IUserInfoProvider
{
    ClaimsPrincipal? GetCurrentUserInfo();
}
```

One method, one return type, no dependencies beyond `System.Security.Claims`. Everything interesting
about it is in what it *does not* depend on.

---

## The problem it solves

Application code frequently needs to know who is acting: to stamp `CreatedBy` on an entity, to scope a
query to the caller's tenant, to decide whether an operation is permitted, to attach a user identifier
to an audit record.

In an ASP.NET Core application the answer lives on `HttpContext.User`, and the usual way to reach it
from outside a controller is `IHttpContextAccessor`:

```csharp
// The common approach — and the problem with it.
public sealed class AuditingSaveInterceptor(IHttpContextAccessor httpContextAccessor)
{
    private string? CurrentUserId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
```

That one constructor parameter carries a large amount of baggage:

- **It drags ASP.NET Core into the layer.** `IHttpContextAccessor` lives in
  `Microsoft.AspNetCore.Http.Abstractions`. A domain, application or data-access project that takes
  this dependency now references the web stack, and the compiler will happily let the next person
  reach for `HttpContext.Request` from inside a repository.
- **It only works inside a request.** A background worker, a hosted service, a scheduled job, a
  message-queue consumer, a CLI tool and a MAUI application all have a current user; none of them has
  an `HttpContext`. Code written against `IHttpContextAccessor` silently degrades to "no user" in every
  one of those hosts.
- **It is awkward to test.** Faking a current user means constructing a `DefaultHttpContext`, assigning
  a `ClaimsPrincipal` to it, and mocking the accessor to return it — three lines of ceremony,
  repeated in every test class, to express one fact.
- **It states the mechanism, not the intent.** `IHttpContextAccessor` says "I need the HTTP context";
  the code actually needs "the current user". Naming the intent is what makes the dependency
  substitutable.

`IUserInfoProvider` names the intent. `ClaimsPrincipal` is in the base class library, so the
abstraction costs nothing to depend on from anywhere.

---

## Using it

Inject the interface and ask for the principal:

```csharp
using System.Security.Claims;
using Ploch.Common.AppServices.Security;

public sealed class AuditStampingService(IUserInfoProvider userInfoProvider, TimeProvider timeProvider)
{
    public void Stamp(IHasAuditProperties entity)
    {
        var user = userInfoProvider.GetCurrentUserInfo();
        var now = timeProvider.GetUtcNow();

        var userName = user?.Identity?.Name ?? "system";

        if (entity.CreatedTime is null)
        {
            entity.CreatedTime = now;
            entity.CreatedBy = userName;
        }

        entity.ModifiedTime = now;
        entity.LastModifiedBy = userName;
    }
}
```

Two properties of the contract shape how you consume it:

**The return value is nullable, and so is everything under it.** `null` means "there is no current
user" — an anonymous request, a background job, a start-up path. Even when a principal is returned it
may be unauthenticated, so check before trusting it:

```csharp
var user = userInfoProvider.GetCurrentUserInfo();

if (user?.Identity?.IsAuthenticated != true)
{
    return Result.Unauthorized();
}

var tenantId = user.FindFirstValue("tenant_id")
               ?? throw new InvalidOperationException("The authenticated principal carries no tenant claim.");
```

**The method is called, not cached.** Resolve the principal at the point of use rather than in a
constructor — an implementation backed by a request context returns different values across the
lifetime of a singleton, and capturing the first answer is a cross-request data-leak bug in a service
whose lifetime you may not control.

A small extension over the interface keeps the claim-reading in one place:

```csharp
public static class UserInfoProviderExtensions
{
    public static string? GetUserId(this IUserInfoProvider provider) =>
        provider.GetCurrentUserInfo()?.FindFirstValue(ClaimTypes.NameIdentifier);

    public static bool IsInRole(this IUserInfoProvider provider, string role) =>
        provider.GetCurrentUserInfo()?.IsInRole(role) ?? false;
}
```

---

## The web implementation

`Ploch.Common.AppServices.Web` supplies the ASP.NET Core implementation and its registration:

```csharp
namespace Ploch.Common.AppServices.Web;

public class HttpContextUserInfoProvider(IHttpContextAccessor httpContextAccessor) : IUserInfoProvider
{
    public ClaimsPrincipal? GetCurrentUserInfo() => httpContextAccessor.HttpContext?.User;
}
```

That is the whole class. The value is not in the code — it is in the fact that this is the **only**
type in the solution that has to know about `HttpContext`.

Registration:

```csharp
using Ploch.Common.AppServices.Web;

builder.Services.AddHttpContextAccessor();
builder.Services.AddUserInfoProvider();
```

`AddUserInfoProvider` registers `IUserInfoProvider` as a **singleton** bound to
`HttpContextUserInfoProvider`. That is correct here because `IHttpContextAccessor` resolves the
context from an `AsyncLocal` at each call, so the singleton returns the right principal per request —
but it is also the reason the "call, do not cache" rule above matters.

`AddHttpContextAccessor()` is a prerequisite: `HttpContextUserInfoProvider` cannot be constructed
without `IHttpContextAccessor` in the container. Register both.

---

## Writing another implementation

The interface exists so that other hosts can supply their own. Each of the following is a handful of
lines, and none of them requires a change to the consuming code.

### A background worker resolving the user a job was queued for

```csharp
public sealed class JobContextUserInfoProvider(IJobExecutionContext jobContext) : IUserInfoProvider
{
    public ClaimsPrincipal? GetCurrentUserInfo()
    {
        if (jobContext.Current is not { QueuedByUserId: { } userId, QueuedByUserName: { } userName })
        {
            return null;
        }

        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName)
            ],
            authenticationType: "BackgroundJob");

        return new ClaimsPrincipal(identity);
    }
}
```

The `authenticationType` argument matters: a `ClaimsIdentity` constructed **without** one reports
`IsAuthenticated == false`, so passing it is what makes the synthesised principal behave like a real
one.

### A desktop or MAUI application using the signed-in Windows user

```csharp
public sealed class WindowsIdentityUserInfoProvider : IUserInfoProvider
{
    [SupportedOSPlatform("windows")]
    public ClaimsPrincipal? GetCurrentUserInfo() => new WindowsPrincipal(WindowsIdentity.GetCurrent());
}
```

### A service account for system-initiated work

Migrations, seeding and scheduled maintenance all write audit columns. Rather than special-casing
`null` at every site, register a provider that returns a well-known system principal:

```csharp
public sealed class SystemUserInfoProvider : IUserInfoProvider
{
    private static readonly ClaimsPrincipal SystemPrincipal =
        new(new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, "system"),
                new Claim(ClaimTypes.Name, "System")
            ],
            authenticationType: "System"));

    public ClaimsPrincipal? GetCurrentUserInfo() => SystemPrincipal;
}
```

```csharp
// In a migration or seeding host:
services.AddSingleton<IUserInfoProvider, SystemUserInfoProvider>();
```

### Falling back between sources

Because the abstraction is one method, composing implementations is trivial — useful in an application
that both serves requests and runs hosted services in the same process:

```csharp
public sealed class FallbackUserInfoProvider(IEnumerable<IUserInfoProvider> providers) : IUserInfoProvider
{
    public ClaimsPrincipal? GetCurrentUserInfo() =>
        providers.Select(provider => provider.GetCurrentUserInfo())
                 .FirstOrDefault(principal => principal?.Identity?.IsAuthenticated == true);
}
```

---

## Testing against it

This is where the abstraction pays for itself most visibly. Supplying a current user is one line:

```csharp
[Theory]
[AutoMockData]
public void Stamp_should_record_the_current_user_as_the_creator(
    [Frozen] Mock<IUserInfoProvider> userInfoProvider,
    AuditStampingService sut)
{
    var principal = new ClaimsPrincipal(
        new ClaimsIdentity([ new Claim(ClaimTypes.Name, "alice") ], authenticationType: "Test"));

    userInfoProvider.Setup(provider => provider.GetCurrentUserInfo()).Returns(principal);

    var entity = new Order();

    sut.Stamp(entity);

    entity.CreatedBy.Should().Be("alice");
}
```

And the anonymous path — the one that is easy to forget and expensive to get wrong — is equally cheap
to cover:

```csharp
[Theory]
[AutoMockData]
public void Stamp_should_fall_back_to_system_when_there_is_no_current_user(
    [Frozen] Mock<IUserInfoProvider> userInfoProvider,
    AuditStampingService sut)
{
    userInfoProvider.Setup(provider => provider.GetCurrentUserInfo()).Returns((ClaimsPrincipal?)null);

    var entity = new Order();

    sut.Stamp(entity);

    entity.CreatedBy.Should().Be("system");
}
```

The `[AutoMockData]` and `[Frozen]` attributes come from
[`Ploch.TestingSupport.XUnit3.AutoMoq`](testing-support.md).

Compare that with the `IHttpContextAccessor` version, which needs a `DefaultHttpContext`, a principal
assigned to its `User` property, and a mock returning the context — for the same single fact.

---

## Where the boundary should sit

| Code | Depends on |
|---|---|
| Domain and application services, repositories, audit interceptors, background workers | `IUserInfoProvider` |
| Controllers, endpoints, middleware, Razor pages | `HttpContext.User` directly — they are already in the web layer, and the indirection buys nothing |
| Composition root (`Program.cs`) | `AddUserInfoProvider()`, or a host-specific implementation |

The rule of thumb: if the class would still make sense in a console host, it should take
`IUserInfoProvider`.

`IUserInfoProvider` is not an authorisation system. It reports who the caller is; it does not decide
what they may do. Keep policy in ASP.NET Core authorisation policies, or in explicit domain rules that
*consume* the principal this provides.

## See also

- [Testing support](testing-support.md)
- [`Ploch.Common.AppServices.Security` API reference](../api/Ploch.Common.AppServices.Security.html)
- [`Ploch.Common.AppServices.Web` API reference](../api/Ploch.Common.AppServices.Web.html)
