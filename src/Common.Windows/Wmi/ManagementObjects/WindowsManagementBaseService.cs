using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

/// <summary>
///     Represents a base class for services in the Windows Management Instrumentation (WMI) model.
/// </summary>
/// <remarks>
///     This class provides properties that describe a base service, including its state, configuration, and control capabilities.
///     It is based on the Win32_BaseService class.
/// </remarks>
[WindowsManagementClass("Win32_BaseService")]
public class WindowsManagementBaseService : CimService
{
    /// <summary>
    ///     Indicates whether the service can be paused.
    /// </summary>
    public bool AcceptPause { get; set; }

    /// <summary>
    ///     Indicates whether the service can be stopped.
    /// </summary>
    public bool AcceptStop { get; set; }

    /// <summary>
    ///     Indicates whether the service can interact with the desktop.
    /// </summary>
    public bool DesktopInteract { get; set; }

    /// <summary>
    ///     The display name of the service.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    ///     The severity of the error if the service fails to start during startup.
    /// </summary>
    public string? ErrorControl { get; set; }

    /// <summary>
    ///     The exit code returned by the service when it stops.
    /// </summary>
    public int ExitCode { get; set; }

    /// <summary>
    ///     The fully qualified path to the service executable file.
    /// </summary>
    public string? PathName { get; set; }

    /// <summary>
    ///     The service-specific error code returned by the service when it stops.
    /// </summary>
    public int ServiceSpecificExitCode { get; set; }

    /// <summary>
    ///     The type of service (e.g., Win32_OWN_PROCESS or Win32_SHARE_PROCESS).
    /// </summary>
    public ServiceType? ServiceType { get; set; }

    /// <summary>
    ///     The account name under which the service runs.
    /// </summary>
    public ServiceAccontType? StartName { get; set; }

    /// <summary>
    ///     The current state of the service (e.g., Running, Stopped, Paused).
    /// </summary>
    public ServiceState? State { get; set; }

    /// <summary>
    ///     A string that indicates the current status of the object.
    /// </summary>
    public ServiceStatus? Status { get; set; }

    /// <summary>
    ///     A unique tag identifier for the service.
    /// </summary>
    public int TagId { get; set; }
}
