namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides extension methods for the <see cref="CaseSensitivity" /> enum.
/// </summary>
public static class CaseSensitivityExtensions
{
    /// <summary>
    ///     Determines whether case-sensitive comparison should be used based on the CaseSensitivity value.
    /// </summary>
    /// <param name="caseSensitivity">The case sensitivity setting to evaluate.</param>
    /// <param name="caseSensitiveDefault">The default value to use when case sensitivity is unspecified.</param>
    /// <returns>
    ///     <c>true</c> if comparisons should be case-sensitive;
    ///     <c>false</c> if comparisons should be case-insensitive.
    ///     For <see cref="CaseSensitivity.Unspecified" />, returns the value of <paramref name="caseSensitiveDefault" />.
    /// </returns>
    public static bool IsCaseSensitive(this CaseSensitivity caseSensitivity, bool caseSensitiveDefault) => caseSensitivity == CaseSensitivity.Unspecified
        ? caseSensitiveDefault
        : caseSensitivity == CaseSensitivity.Sensitive;
}
