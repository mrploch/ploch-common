using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

public enum ServiceStatus
{
    OK,
    Error,
    Degraded,
    Unknown,

    [WindowsManagementObjectEnumMapping("Pred Fail")]
    PredFail,
    Starting,
    Stopping,
    Service,
    Stressed,
    NonRecover,
    NoContact,

    [WindowsManagementObjectEnumMapping("Lost Comm")]
    LostComm
}
