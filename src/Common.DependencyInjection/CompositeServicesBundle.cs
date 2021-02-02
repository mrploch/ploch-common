using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection
{

    public class CompositeServicesBundle : IServicesBundle
    {
        private readonly ICollection<IServicesBundle> _servicesBundles = new List<IServicesBundle>();

        public CompositeServicesBundle(params IServicesBundle[] servicesBundles)
        {
            foreach (var servicesBundle in servicesBundles)
            {
                AddServices(servicesBundle);
            }
        }

        public CompositeServicesBundle AddServices(IServicesBundle services)
        {
            Guard.Against.Null(services, nameof(services));
            Guard.Against.OperationCondition(_servicesBundles.Contains(services), "Provided instance of services was already added to the bundle.");
            _servicesBundles.Add(services);
            return this;
        }
        
        /// <inheritdoc />
        public void Configure(IServiceCollection serviceCollection)
        {
            foreach (var configurator in _servicesBundles)
            {
                configurator.Configure(serviceCollection);
            }
        }
    }
}