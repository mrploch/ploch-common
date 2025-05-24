using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     Provides mechanisms to retrieve and manage information about system applications
///     such as services and processes on Windows platforms using WMI (Windows Management Instrumentation).
/// </summary>
/// <remarks>
///     The class leverages <see cref="IWmiObjectQueryFactory" /> to execute WMI queries and fetch
///     information about system resources. It implements the <see cref="ISystemApplicationsProvider" />
///     interface, offering the ability to obtain detailed data on system services and processes.
/// </remarks>
/// <param name="wmiObjectQueryFactory">The factory for creating WMI object queries.</param>
/// <param name="logger">The logger for recording diagnostic information.</param>
public class WmiSystemApplicationsProvider(IWmiObjectQueryFactory wmiObjectQueryFactory, ILogger<WmiSystemApplicationsProvider> logger)
    : ISystemApplicationsProvider
{
    /// <summary>
    ///     Retrieves a collection of system services available on the Windows platform using WMI.
    /// </summary>
    /// <returns>
    ///     A collection of <see cref="ServiceInfo" /> objects, each containing detailed information
    ///     about a system service, including its name and display name.
    /// </returns>
    [SupportedOSPlatform("windows")]
    public IEnumerable<ServiceInfo> GetServices()
    {
        using var wmiQuery = wmiObjectQueryFactory.Create();

        var services = wmiQuery.GetAll<WindowsManagementService>();

        var results = new List<ServiceInfo>();
        foreach (var service in services)
        {
            logger.LogInformation("Processing service {ServiceName} - {DisplayName}", service.Name, service.DisplayName);

            var serviceInfo = ServiceInfoBuilder.Create(service);
            results.Add(serviceInfo);
        }

        return results;
    }

    /// <summary>
    ///     Retrieves a collection of running processes on the Windows platform using WMI.
    /// </summary>
    /// <returns>
    ///     A collection of <see cref="ProcessInfo" /> objects, each containing detailed information
    ///     about a running process, including its ID, name, display name, and command line information.
    /// </returns>
    public IEnumerable<ProcessInfo> GetProcesses()
    {
        using var wmiQuery = wmiObjectQueryFactory.Create();

        var processes = wmiQuery.GetAll<WindowsManagementProcess>();

        var results = new List<ProcessInfo>();

        foreach (var process in processes)
        {
            var processInfo = ProcessInfoBuilder.Create(process);

            results.Add(processInfo);
        }

        return results;
    }
}
