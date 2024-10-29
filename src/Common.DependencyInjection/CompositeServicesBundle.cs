using System;
using System.Collections.Generic;
using Dawn;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection;

/// <summary>
///     Represents a collection of services bundles to register.
/// </summary>
/// <remarks>
///     Implementation of the <see cref="Ploch.Common.DependencyInjection.IServicesBundle" /> which contains
///     a collection of other bundles.
/// </remarks>
/// <seealso cref="IServicesBundle" />
public class CompositeServicesBundle : IServicesBundle
{
    private readonly ICollection<IServicesBundle> _servicesBundles = new List<IServicesBundle>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="CompositeServicesBundle" /> class.
    /// </summary>
    /// <param name="servicesBundles">Bundles used to initialize the <c>CompositeServicesBundle</c>.</param>
    public CompositeServicesBundle(params IServicesBundle[] servicesBundles)
    {
        Guard.Argument(servicesBundles, nameof(servicesBundles)).NotNull();

        foreach (var servicesBundle in servicesBundles)
        {
            AddServices(servicesBundle);
        }
    }

    /// <summary>
    ///     Configures a <c>IServiceCollection</c> instance using the bundles stored in this bundle.
    /// </summary>
    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="serviceCollection" /> value is <c>null</c>.</exception>
    public void Configure(IServiceCollection serviceCollection)
    {
        Guard.Argument(serviceCollection, nameof(serviceCollection)).NotNull();

        foreach (var configurator in _servicesBundles)
        {
            configurator.Configure(serviceCollection);
        }
    }

    /// <summary>
    ///     Adds a services bundle to this bundle.
    /// </summary>
    /// <param name="services">Services bundle to add.</param>
    /// <exception cref="ArgumentNullException"><paramref name="services" /> value is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="services" /> was already added.</exception>
    public CompositeServicesBundle AddServices(IServicesBundle services)
    {
        Guard.Argument(services, nameof(services)).NotNull();
        Guard.Operation(!_servicesBundles.Contains(services));

        _servicesBundles.Add(services);

        return this;
    }
}