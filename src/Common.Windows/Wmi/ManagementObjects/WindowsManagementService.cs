using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

/// <summary>
///     Represents a Windows service in the Windows Management Instrumentation (WMI) model.
/// </summary>
/// <remarks>
///     This class provides properties that describe a Windows service, including its process ID, startup configuration, and operational state.
///     It is based on the Win32_Service class.
/// </remarks>
[WindowsManagementClass("Win32_Service")]
public class WindowsManagementService : WindowsManagementBaseService
{
    /// <summary>
    ///     The current checkpoint value of the service during a pending start, stop, pause, or continue operation.
    /// </summary>
    public int CheckPoint { get; set; }

    /// <summary>
    ///     Indicates whether the service is configured for delayed auto-start.
    /// </summary>
    public bool DelayedAutoStart { get; set; }

    /// <summary>
    ///     The process identifier of the service.
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    ///     The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds.
    /// </summary>
    public int WaitHint { get; set; }
}
