---
title: Untitled doc
---
<SwmSnippet path="/src/Common.DependencyInjection/ServicesBundle.cs" line="1">

---

&nbsp;

```c#
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Collections;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Abstract base class for organizing service registrations into logical bundles.
/// </summary>
/// <remarks>
///     Provides a template for registering services with the dependency injection container.
///     Derived classes can specify dependencies on other service bundles and must implement
///     the <see cref="DoConfigure" /> method to register their specific services.
/// </remarks>
public abstract class ServicesBundle : IServicesBundle
{
    /// <summary>
    ///     Gets the service collection for registering services.
    /// </summary>
    /// <remarks>
    ///     This property is set during the <see cref="Configure" /> method execution
    ///     and can be used by derived classes to register services in <see cref="DoConfigure" />.
    /// </remarks>
    protected IServiceCollection Services { get; private set; } = null!;

    /// <summary>
    ///     Gets the collection of service bundles that this bundle depends on.
    /// </summary>
    /// <remarks>
    ///     Dependencies are configured before this bundle's <see cref="DoConfigure" /> method is called.
    ///     Override this property to specify dependencies on other service bundles.
    /// </remarks>
    protected virtual IEnumerable<IServicesBundle>? Dependencies { get; }

    public IConfiguration? Configuration { get; set; }

    /// <summary>
    ///     Configures the service collection by first configuring dependencies and then calling <see cref="DoConfigure" />.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    public void Configure(IServiceCollection services)
    {
        Services = services;

        Dependencies?.ForEach(dependency => services.AddServicesBundle(dependency, Configuration));

        DoConfigure();
    }

    /// <summary>
    ///     When implemented in a derived class, registers services with the dependency injection container.
    /// </summary>
    /// <remarks>
    ///     This method is called after all dependencies have been configured.
    ///     Use the <see cref="Services" /> property to register services.
    /// </remarks>
    public abstract void DoConfigure();
}

```

---

</SwmSnippet>

<SwmSnippet path="/src/Common.DependencyInjection/IServicesBundle.cs" line="1">

---

&nbsp;

```c#
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Represents a collection of services that should be added to <see cref="IServiceCollection" />.
/// </summary>
/// <remarks>
///     Helps grouping service registrations together.
///     <para>
///         Same concept as
///         <see href="https://autofaccn.readthedocs.io/en/latest/configuration/modules.html">Autofac Modules</see>.
///     </para>
/// </remarks>
public interface IServicesBundle
{
    IConfiguration? Configuration { get; }

    /// <summary>
    ///     Configures a <c>IServiceCollection</c> instance.
    /// </summary>
    /// <remarks>
    ///     Implementations of this method should add all required services for this particular Services Bundle.
    ///     Method will be executed when a service provider is built.
    /// </remarks>
    /// <param name="services">A service collection.</param>
    void Configure(IServiceCollection services);
}
```

---

</SwmSnippet>

<SwmMeta version="3.0.0" repo-id="Z2l0aHViJTNBJTNBcGxvY2gtY29tbW9uJTNBJTNBbXJwbG9jaA==" repo-name="ploch-common"><sup>Powered by [Swimm](https://app.swimm.io/)</sup></SwmMeta>
