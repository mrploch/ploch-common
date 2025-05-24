using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

public enum ServiceType
{
    [WindowsManagementObjectEnumMapping("Kernel Driver", IncludeActualEnumName = true)]
    KernelDriver,

    [WindowsManagementObjectEnumMapping("File System Driver", IncludeActualEnumName = true)]
    FileSystemDriver,

    Adapter,

    [WindowsManagementObjectEnumMapping("Recognizer Driver", IncludeActualEnumName = true)]
    RecognizerDriver,

    [WindowsManagementObjectEnumMapping("Own Process", IncludeActualEnumName = true)]
    OwnProcess,

    [WindowsManagementObjectEnumMapping("Share Process", IncludeActualEnumName = true)]
    ShareProcess,

    [WindowsManagementObjectEnumMapping("Interactive Process", IncludeActualEnumName = true)]
    InteractiveProcess,

    Unknown
}
