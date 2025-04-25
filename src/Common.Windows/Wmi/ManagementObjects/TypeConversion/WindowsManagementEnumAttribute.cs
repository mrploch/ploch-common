namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public sealed class WindowsManagementEnumAttribute : Attribute
{
    public WindowsManagementEnumAttribute() : this(false)
    { }

    public WindowsManagementEnumAttribute(bool caseSensitive) => CaseSensitive = caseSensitive;

    public bool CaseSensitive { get; }
}
