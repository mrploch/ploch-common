using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;

[WindowsManagementEnum(true)]
public enum TestEnumWithCaseSensitiveMatching
{
    Value1,
    Value2
}
