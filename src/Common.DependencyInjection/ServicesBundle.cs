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
public abstract class ServicesBundle : IServicesBundle, IConfigurationConsumer
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
