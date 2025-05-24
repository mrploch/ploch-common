using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

public enum ServiceState
{
    Running,
    Stopped,

    [WindowsManagementObjectEnumMapping("Start Pending", IncludeActualEnumName = true)]
    StartPending,

    [WindowsManagementObjectEnumMapping("Stop Pending", IncludeActualEnumName = true)]
    StopPending,

    [WindowsManagementObjectEnumMapping("Continue Pending", IncludeActualEnumName = true)]
    ContinuePending,

    [WindowsManagementObjectEnumMapping("Pause Pending", IncludeActualEnumName = true)]
    PausePending,
    Paused,
    Unknown
}
