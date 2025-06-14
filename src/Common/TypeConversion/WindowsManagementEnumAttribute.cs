using System;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Specifies whether the enumeration values should be treated in a case-sensitive or case-insensitive manner
///     when being mapped or utilized in contexts like enumeration value extraction.
/// </summary>
/// <remarks>
///     This attribute can be applied to enums to designate if their string representation mappings
///     should be case-sensitive or case-insensitive.
/// </remarks>
public sealed class WindowsManagementEnumAttribute(bool caseSensitive = false) : Attribute
{
    public bool CaseSensitive { get; } = caseSensitive;
}
