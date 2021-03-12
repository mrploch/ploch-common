using Dawn;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection
{
    /// <summary>
    /// Represents a collection of services bundles to register.
    /// </summary>
    /// <remarks>
    /// Implementation of the <see cref="Ploch.Common.DependencyInjection.IServicesBundle" /> which contains
    /// a collection of other bundles.
    /// </remarks>
    /// <seealso cref="Ploch.Common.DependencyInjection.IServicesBundle" />
    public class CompositeServicesBundle : IServicesBundle
    {
        private readonly ICollection<IServicesBundle> _servicesBundles = new List<IServicesBundle>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeServicesBundle"/> class.
        /// </summary>
        /// <param name="servicesBundles">Bundles used to initialize the <c>CompositeServicesBundle</c>.</param>
        public CompositeServicesBundle([NotNull] params IServicesBundle[] servicesBundles)
        {
            Guard.Argument(servicesBundles, nameof(servicesBundles)).NotNull();

            foreach (var servicesBundle in servicesBundles)
            {
                AddServices(servicesBundle);
            }
        }

        /// <exception cref="T:System.ArgumentNullException"><paramref name="services" /> value is <c>null</c>.</exception>
        /// <exception cref="T:System.InvalidOperationException"><paramref name="services"/> was already added.</exception>
        public CompositeServicesBundle AddServices(IServicesBundle services)
        {
            Guard.Argument(services, nameof(services)).NotNull();
            Guard.Operation(!_servicesBundles.Contains(services));

            _servicesBundles.Add(services);
            return this;
        }

        /// <inheritdoc />
        /// <exception cref="T:System.ArgumentNullException"><paramref name="serviceCollection" /> value is <c>null</c>.</exception>
        public void Configure([NotNull] IServiceCollection serviceCollection)
        {
            Guard.Argument(serviceCollection, nameof(serviceCollection)).NotNull();

            foreach (var configurator in _servicesBundles)
            {
                configurator.Configure(serviceCollection);
            }
        }
    }
}