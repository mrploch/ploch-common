using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

[WindowsManagementEnum(false)]
public enum TestEnumWithCaseInsensitiveConversion
{
    Value1,
    Value2
}
