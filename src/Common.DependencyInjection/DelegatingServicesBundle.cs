using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     A services bundle that delegates service configuration to a collection of action delegates.
/// </summary>
/// <remarks>
///     This class provides a flexible way to configure services by allowing multiple configuration
///     actions to be registered and executed during the configuration phase. Each action receives
///     both the service collection and configuration instance, enabling dynamic service registration
///     based on configuration values.
///     <para>
///         This is useful when you need to build service configurations programmatically or when
///         you want to compose service registrations from multiple sources without creating
///         separate bundle classes.
///     </para>
/// </remarks>
/// <example>
///     <code language="csharp">
/// var bundle = new DelegatingServicesBundle()
///     .Configure((services, config) => services.AddScoped&lt;IMyService, MyService&gt;())
///     .Configure((services, config) => services.AddSingleton&lt;IAnotherService, AnotherService&gt;());
///
/// services.AddServicesBundle(bundle, configuration);
/// </code>
/// </example>
public class DelegatingServicesBundle : ConfigurableServicesBundle
{
    private readonly List<Action<(IServiceCollection services, IConfiguration? configuration)>> _serviceCollectionActions = new();

    /// <summary>
    ///     Adds a service configuration action to be executed during the configuration phase.
    /// </summary>
    /// <param name="serviceCollectionAction">
    ///     An action that configures services. The action receives a tuple containing the service collection
    ///     and the configuration instance (which may be null).
    /// </param>
    /// <returns>
    ///     The current <see cref="DelegatingServicesBundle" /> instance to enable method chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="serviceCollectionAction" /> is <c>null</c>.
    /// </exception>
    /// <example>
    ///     <code>
    /// bundle.Configure((services, config) =>
    /// {
    ///     services.AddScoped&lt;IRepository, DatabaseRepository&gt;();
    ///     if (config?["UseCache"] == "true")
    ///     {
    ///         services.AddSingleton&lt;ICacheService, RedisCacheService&gt;();
    ///     }
    /// });
    /// </code>
    /// </example>
    public DelegatingServicesBundle Configure(Action<(IServiceCollection services, IConfiguration? configuration)> serviceCollectionAction)
    {
        serviceCollectionAction.NotNull(nameof(serviceCollectionAction));

        _serviceCollectionActions.Add(serviceCollectionAction);

        return this;
    }

    /// <summary>
    ///     Executes all registered service configuration actions to configure the service collection.
    /// </summary>
    /// <param name="configuration">
    ///     The configuration instance to pass to each configuration action. May be <c>null</c>.
    /// </param>
    /// <remarks>
    ///     This method iterates through all previously registered configuration actions (added via
    ///     <see cref="Configure(Action{ValueTuple{IServiceCollection, IConfiguration}})" />) and executes
    ///     them in the order they were added. Each action receives both the service collection and
    ///     the provided configuration instance.
    ///     <para>
    ///         If any configuration action throws an exception, the configuration process will be
    ///         interrupted and the exception will propagate to the caller.
    ///     </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when the <see cref="ConfigurableServicesBundle.Services" /> property is <c>null</c>.
    /// </exception>
    /// <exception cref="Exception">
    ///     Thrown when any of the registered configuration action delegates throws an exception.
    /// </exception>
    /// <inheritdoc />
    protected override void Configure(IConfiguration configuration)
    {
        Services.NotNull(nameof(Services));

        foreach (var serviceCollectionAction in _serviceCollectionActions)
        {
            serviceCollectionAction((Services, configuration));
        }
    }
}
