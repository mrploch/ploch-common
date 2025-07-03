using System.IO.Abstractions;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Windows.DependencyInjection;
using Ploch.Common.Windows.SystemApplications.Operations;
using Ploch.Common.Windows.Wmi;

namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     A services bundle that registers Windows system application related services in the dependency injection container.
///     This bundle provides access to system applications, services management, and registry services.
/// </summary>
[SupportedOSPlatform("windows")]
public class SystemApplicationsServicesBundle : ConfigurableServicesBundle
{
    /// <summary>
    ///     Configures the service collection by registering all required services for Windows system applications functionality.
    /// </summary>
    /// <param name="services">
    ///     The service collection to configure. This collection will be populated with system application
    ///     related services, including WMI query services, system application matchers, file system access, system applications
    ///     providers, service management, and registry services.
    /// </param>
    /// <remarks>
    ///     This method registers the following services:
    ///     - WMI object query services
    ///     - System application matchers
    ///     - File system abstraction
    ///     - System applications provider
    ///     - Service start mode management
    ///     - Windows registry services lister
    /// </remarks>
    protected override void Configure(IConfiguration? configuration)
    {
        Services.AddServicesBundle<WmiObjectQueryServicesBundle>(configuration)
                .AddServicesBundle<SystemApplicationMatchersServicesBundle>(configuration)
                .AddServicesBundle<SystemApplicationActionsServicesBundle>(configuration)
                .AddServicesBundle<RegistryServiceListerBundle>(configuration)
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<ISystemApplicationsProvider, WmiSystemApplicationsProvider>()
                .AddSingleton<IServiceStartModeUpdater, RegistryStartModeUpdater>()
                .AddSingleton<IServicesLister, WindowsRegistryServicesLister>();
    }
}
