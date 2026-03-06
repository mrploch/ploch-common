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
[AttributeUsage(AttributeTargets.Enum)]
public sealed class EnumConversionAttribute(bool caseSensitive = false) : Attribute
{
    /// <summary>
    ///     Gets a value indicating whether the enumeration values should be treated
    ///     in a case-sensitive manner when being mapped or processed.
    /// </summary>
    /// <remarks>
    ///     This property determines the case sensitivity of enumeration value mappings,
    ///     often utilized in scenarios such as string-to-enum conversions or enumeration
    ///     value extraction tasks. When set to <c>true</c>, mappings will respect the
    ///     character casing of the enumeration values.
    /// </remarks>
    public bool CaseSensitive { get; } = caseSensitive;
}
