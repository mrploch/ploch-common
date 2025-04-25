using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

[WindowsManagementClass("Win32_TestClass")]
public class WmiTestClass
{
    [WindowsManagementObjectProperty("Name")]
    public string? Name { get; set; }

    [WindowsManagementObjectProperty("IntValue")]
    public int IntValue { get; set; }

    [WindowsManagementObjectDateTimeProperty("DateTimeValue", DateTimeKind.Utc)]
    public DateTime DateTimeValue { get; set; }

    [WindowsManagementObjectProperty("TestStringValue")]
    public string? StringPropertyWithDifferentName { get; set; }

    public string? TestPropertyWithoutAttribute { get; set; }

    public TestEnum? TestEnumValue { get; set; }
}
