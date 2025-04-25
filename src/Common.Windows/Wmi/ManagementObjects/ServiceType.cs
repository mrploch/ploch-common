using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

public enum ServiceType
{
    [WindowsManagementObjectEnumMapping("Own Process")]
    OwnProcess,

    [WindowsManagementObjectEnumMapping("Share Process")]
    ShareProcess,
    Unknown
}
