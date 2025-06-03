using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

[WindowsManagementClass("Win32_TestClass")]
public class WmiTestClass
{
    [WindowsManagementObjectProperty("Name")]
    public string? Name { get; set; }

    [WindowsManagementObjectProperty("IntValue")]
    public int IntValue { get; set; }

    public DateTime DateTimeValue { get; set; }

    public DateTime? NullableDateTimeValue { get; set; }

    public DateTimeOffset DateTimeOffsetValue { get; set; }

    public DateTimeOffset? NullableDateTimeOffsetValue { get; set; }

    [WindowsManagementObjectProperty("TestStringValue")]
    public string? StringPropertyWithDifferentName { get; set; }

    public string? TestPropertyWithoutAttribute { get; set; }

    [WindowsManagementObjectProperty("TestEnumValue1")]
    public TestEnumWithMappings2? TestEnumValue { get; set; }
}
