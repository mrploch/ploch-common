namespace Ploch.Common.TypeConversion;

/// <summary>
///     Specifies whether string comparisons should be case-sensitive, case-insensitive, or use an unspecified case sensitivity.
/// </summary>
public enum CaseSensitivity
{
    /// <summary>
    ///     Specifies that string comparisons should be case-insensitive.
    /// </summary>
    Insensitive,

    /// <summary>
    ///     Specifies that string comparisons should be case-sensitive.
    /// </summary>
    Sensitive,

    /// <summary>
    ///     Specifies that the case sensitivity for string comparisons is unspecified or should use a default behavior.
    /// </summary>
    Unspecified
}
