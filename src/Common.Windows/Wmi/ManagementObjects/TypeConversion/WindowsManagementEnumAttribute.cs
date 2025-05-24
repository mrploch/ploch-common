namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <summary>
///     Specifies whether the enumeration values should be treated in a case-sensitive or case-insensitive manner
///     when being mapped or utilized in contexts like enumeration value extraction.
/// </summary>
/// <remarks>
///     This attribute can be applied to enums to designate if their string representation mappings
///     should be case-sensitive or case-insensitive.
/// </remarks>
public sealed class WindowsManagementEnumAttribute : Attribute
{
    public WindowsManagementEnumAttribute() : this(false)
    { }

    public WindowsManagementEnumAttribute(bool caseSensitive) => CaseSensitive = caseSensitive;

    public bool CaseSensitive { get; }
}
