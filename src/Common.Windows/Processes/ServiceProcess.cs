using System.Diagnostics;
using System.ServiceProcess;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.Processes;

/// <summary>
///     Represents a Windows service process, combining information from both the Service Controller API and Windows Management Instrumentation (WMI).
/// </summary>
public class ServiceProcess
{
    /// <summary>
    ///     Gets or initializes the Windows Management Service object that provides WMI-based information about the service.
    /// </summary>
    public required WindowsManagementService Service { get; init; }

    /// <summary>
    ///     Gets or initializes the ServiceController object that provides access to service control operations.
    /// </summary>
    public required ServiceController ServiceController { get; init; }

    /// <summary>
    ///     Gets or sets the Process object associated with the running service, if available.
    ///     May be null if the service is not running or the process information cannot be retrieved.
    /// </summary>
    public Process? Process { get; set; }

    /// <summary>
    ///     Gets or sets the WMI-based process information associated with the service.
    ///     May be null if the service is not running or the WMI process information cannot be retrieved.
    /// </summary>
    public WindowsManagementProcess? WmiProcess { get; set; }
}
