using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.Windows.Wmi;

/// <summary>
///     Provides a service bundle for registering WMI (Windows Management Instrumentation) query services.
/// </summary>
public class WmiObjectQueryServicesBundle : IServicesBundle
{
    /// <summary>
    ///     Configures the service collection by registering WMI query-related services.
    /// </summary>
    /// <param name="services">The service collection to which the WMI services will be added.</param>
    /// <remarks>
    ///     This method registers the following services:
    ///     - <see cref="IWmiObjectQueryFactory" /> as a singleton, implemented by <see cref="WmiObjectQueryFactory" />
    ///     - <see cref="IWmiConnectionFactory" /> as a singleton, implemented by <see cref="DefaultWmiConnectionFactory" />
    /// </remarks>
    public void Configure(IServiceCollection services) => services.AddSingleton<IWmiObjectQueryFactory, WmiObjectQueryFactory>()
                                                                  .AddSingleton<IWmiConnectionFactory, DefaultWmiConnectionFactory>();
}
