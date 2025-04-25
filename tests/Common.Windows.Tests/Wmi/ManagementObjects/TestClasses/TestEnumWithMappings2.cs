using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

public enum TestEnumWithMappings2
{
    [WindowsManagementObjectEnumMapping("Value 1", "value1")]
    Value1,

    [WindowsManagementObjectEnumMapping("Value 2", "value2")]
    Value2,

    [WindowsManagementObjectEnumMapping]
    Value3MappedToNull
}
