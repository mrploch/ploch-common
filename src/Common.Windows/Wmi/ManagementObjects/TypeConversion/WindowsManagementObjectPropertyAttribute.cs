using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

[AttributeUsage(AttributeTargets.Property)]
public class WindowsManagementObjectPropertyAttribute : Attribute
{
    public WindowsManagementObjectPropertyAttribute(string propertyName) => PropertyName = propertyName.NotNull();

    public string PropertyName { get; }
}
