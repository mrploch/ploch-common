using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection
{
    public class DelegatingServicesBundle : IServicesBundle
    {
        private readonly ICollection<Action<IServiceCollection>> _serviceCollectionActions = new List<Action<IServiceCollection>>();

        /// <inheritdoc />
        /// <exception cref="T:System.Exception">A delegate callback throws an exception.</exception>
        public void Configure(IServiceCollection serviceCollection)
        {
            foreach (var serviceCollectionAction in _serviceCollectionActions)
            {
                serviceCollectionAction(serviceCollection);
            }
        }

        public DelegatingServicesBundle Configure(Action<IServiceCollection> serviceCollectionAction)
        {
            _serviceCollectionActions.Add(serviceCollectionAction);

            return this;
        }
    }
}