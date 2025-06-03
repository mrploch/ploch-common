using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

public enum TestEnumWithMappings2
{
    [WindowsManagementObjectEnumMapping("Value 1", IncludeActualEnumName = true)]
    Value1,

    [WindowsManagementObjectEnumMapping("Value 2", IncludeActualEnumName = true)]
    Value2,

    [WindowsManagementObjectEnumMapping]
    Value3MappedToNull
}
