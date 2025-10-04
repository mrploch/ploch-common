using System;
using Microsoft.Extensions.Configuration;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Base class for service bundles that require configuration.
///     Provides a mechanism to register and configure services with dependency on configuration.
/// </summary>
public abstract class ConfigurableServicesBundle : ServicesBundle
{
    /// <summary>
    ///     Configures the services within the derived service bundle.
    ///     This method is typically overridden by derived classes to implement the specific
    ///     service registrations and configurations necessary for the bundle.
    /// </summary>
    public override void DoConfigure()
    {
        if (Configuration == null)
        {
            throw new InvalidOperationException($"{GetType().Name} is a configurable service bundle, but configuration was not initialized.");
        }

        Configure(Configuration.RequiredNotNull(nameof(Configuration)));
    }

    /// <summary>
    ///     Configures services with the specified configuration.
    ///     This method must be implemented by derived classes to register and configure services.
    /// </summary>
    /// <param name="configuration">The configuration to use for configuring services. Can be null.</param>
    protected abstract void Configure(IConfiguration configuration);
}
