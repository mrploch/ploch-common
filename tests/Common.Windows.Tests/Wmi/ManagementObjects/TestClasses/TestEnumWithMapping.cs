using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

public enum TestEnumWithMapping
{
    [WindowsManagementObjectEnumMapping("ValueWithOneMappedName1")]
    ValueWithOneMappedName,

    [WindowsManagementObjectEnumMapping("ValueWithMultipleMappedNames1", "ValueWithMultipleMappedNames2", null)]
    ValueWithMultipleMappedNames,
    ValueWithoutMappingAttribute
}
