using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Windows.SystemApplications.Operations;

namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     Provides methods for registering services related to system applications using Windows Management Instrumentation (WMI).
/// </summary>
/// <remarks>
///     This static class is designed to simplify the registration of services required for interacting with system applications.
///     It includes methods to add services for file system abstraction, system application providers, and WMI object queries
///     to the dependency injection container.
/// </remarks>
public static class SystemApplicationServicesRegistrations
{
    /// <summary>
    ///     Registers services required for working with system applications using WMI (Windows Management Instrumentation).
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to which the services will be added.</param>
    /// <returns>The updated <see cref="IServiceCollection" /> instance.</returns>
    /// <remarks>
    ///     This method adds the following services to the dependency injection container:
    ///     <list type="bullet">
    ///         <item>
    ///             <description><see cref="IFileSystem" /> implementation for file system abstraction.</description>
    ///         </item>
    ///         <item>
    ///             <description><see cref="ISystemApplicationsProvider" /> implementation for retrieving system applications.</description>
    ///         </item>
    ///         <item>
    ///             <description>WMI object query services for querying WMI objects.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public static IServiceCollection AddWmiSystemApplicationServices(this IServiceCollection services) =>
        services.AddServicesBundle<SystemApplicationsServicesBundle>();
}
