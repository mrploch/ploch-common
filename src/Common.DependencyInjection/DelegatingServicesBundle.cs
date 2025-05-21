using System;
using System.Collections.Generic;
using Dawn;
using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.DependencyInjection;

public class DelegatingServicesBundle : IServicesBundle
{
    private readonly ICollection<Action<IServiceCollection>> _serviceCollectionActions = new List<Action<IServiceCollection>>();

    /// <inheritdoc />
    /// <exception cref="Exception">A delegate callback throws an exception.</exception>
    public void Configure(IServiceCollection serviceCollection)
    {
        Guard.Argument(serviceCollection, nameof(serviceCollection)).NotNull();

        foreach (var serviceCollectionAction in _serviceCollectionActions)
        {
            serviceCollectionAction(serviceCollection);
        }
    }

    public DelegatingServicesBundle Configure(Action<IServiceCollection> serviceCollectionAction)
    {
        Guard.Argument(serviceCollectionAction, nameof(serviceCollectionAction)).NotNull();

        _serviceCollectionActions.Add(serviceCollectionAction);

        return this;
    }
}