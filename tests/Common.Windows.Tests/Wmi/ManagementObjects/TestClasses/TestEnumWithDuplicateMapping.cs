using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

public enum TestEnumWithDuplicateMapping
{
    Value1,

    [WindowsManagementObjectEnumMapping("Duplicate")]
    Value2,

    [WindowsManagementObjectEnumMapping("Duplicate")]
    Value3
}
