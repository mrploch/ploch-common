namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

[AttributeUsage(AttributeTargets.Field)]
public sealed class WindowsManagementObjectEnumMappingAttribute : Attribute
{
    public WindowsManagementObjectEnumMappingAttribute() : this([null])
    { }

    public WindowsManagementObjectEnumMappingAttribute(params string?[] names) => Names = names;

    public IEnumerable<string?> Names { get; }

    public bool CaseSensitive { get; set; } = false;
}
