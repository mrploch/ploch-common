namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public class WindowsManagementObjectDateTimePropertyAttribute : WindowsManagementObjectPropertyAttribute
{
    public WindowsManagementObjectDateTimePropertyAttribute(string propertyName, DateTimeKind dateTimeKind) : base(propertyName) => DateTimeKind = dateTimeKind;

    public DateTimeKind DateTimeKind { get; }
}
