using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Extension methods for <see cref="string" /> and related.
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    ///     Extension method that determines if a string is not <c>null</c> or empty.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns><c>true</c> if the string is not <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNotNullOrEmpty(this string? str) => !str.IsNullOrEmpty();

    /// <summary>
    ///     Extension method version of <see cref="string.IsNullOrEmpty" />.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns><c>true</c> if string is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty(this string? str) => string.IsNullOrEmpty(str);

    /// <summary>
    ///     Extension method version of <see cref="string.IsNullOrWhiteSpace" />.
    ///     Extension method version of <see cref="string.IsNullOrWhiteSpace" />.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns><c>true</c> if string is <c>null</c>, empty, or white-space; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrWhiteSpace(this string? str) => string.IsNullOrWhiteSpace(str);

    /// <summary>
    ///     Extension method that returns <c>null</c> if the given string is empty; otherwise, returns the string itself.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns><c>null</c> if the string is empty; otherwise, the original string.</returns>
    public static string? NullIfEmpty(this string? str) => str.IsNullOrEmpty() ? null : str;

    /// <summary>
    ///     Extension method that returns <c>null</c> if a string is <c>null</c>, empty,
    ///     or consists only of white-space characters; otherwise, returns the original string.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns>
    ///     The original string if it is not <c>null</c>, empty, or consists only of white-space characters;
    ///     otherwise, <c>null</c>.
    /// </returns>
    public static string? NullIfWhiteSpace(this string? str) => str.IsNullOrWhiteSpace() ? null : str;

    /// <summary>
    ///     Encodes a string as base64 string using <see cref="Encoding.UTF8" />.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>Encoded version of supplied string.</returns>
    public static string ToBase64String(this string str) => str.ToBase64String(Encoding.UTF8);

    /// <summary>
    ///     Encodes a string as base64 string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>Encoded version of supplied string.</returns>
    public static string ToBase64String(this string str, Encoding encoding)
    {
#pragma warning disable S3236
        str.NotNull(nameof(str));
#pragma warning restore S3236

        return Convert.ToBase64String(encoding.GetBytes(str));
    }

    /// <summary>
    ///     Decodes a base64 string using <see cref="Encoding.UTF8" /> encoding.
    /// </summary>
    /// <param name="str">The base64 encoded string.</param>
    /// <returns>The decoded base64 string.</returns>
    public static string FromBase64String(this string str) => str.FromBase64String(Encoding.UTF8);

    /// <summary>
    ///     Decodes a base64 string using provided encoding.
    /// </summary>
    /// <param name="str">The base64 encoded string.</param>
    /// <param name="encoding">The <paramref name="str" /> encoding.</param>
    /// <returns>The decoded base64 string.</returns>
    public static string FromBase64String(this string str, Encoding encoding)
    {
        str.NotNull(nameof(str));

        return encoding.GetString(Convert.FromBase64String(str));
    }

    /// <summary>
    ///     Compare two strings ignoring the case.
    /// </summary>
    /// <param name="str">The first string to compare.</param>
    /// <param name="other">The second string to compare.</param>
    /// <returns><c>true</c> if strings are equal ignoring case, <c>false</c> otherwise.</returns>
    public static bool EqualsIgnoreCase(this string? str, string? other)
    {
        if (str == null && other == null)
        {
            return true;
        }

        if (str == null || other == null)
        {
            return false;
        }

        return str.Equals(other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Replaces the
    ///     <param name="oldValue"></param>
    ///     with
    ///     <param name="newValue"></param>
    ///     in the string
    ///     <param name="str"></param>
    ///     if the string starts with
    ///     <param name="oldValue"></param>
    ///     .
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="oldValue">The old value to replace.</param>
    /// <param name="newValue">The new value to replace the old value with.</param>
    /// <param name="stringComparison">
    ///     The string comparison to use when checking if the <paramref name="str" /> starts with
    ///     <paramref name="oldValue" />. If not provided, then <see cref="StringComparison.InvariantCulture" /> will be used.
    /// </param>
    /// <returns>The provided string with a new value at the beginning or the original <paramref name="str" />.</returns>
    public static string ReplaceStart(this string str, string oldValue, string newValue, StringComparison stringComparison = StringComparison.InvariantCulture)
    {
        str.NotNull(nameof(str));
        newValue.NotNull(nameof(newValue));
        oldValue.NotNull(nameof(oldValue));

        if (!str.StartsWith(oldValue, stringComparison))
        {
            return str;
        }

        return newValue + str.Substring(oldValue.Length, str.Length - oldValue.Length);
    }

    /// <summary>
    ///     Converts the string representation of a number to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>
    ///     The 32-bit signed integer equivalent to the number contained in the string.
    /// </returns>
    public static int ToInt32(this string str)
    {
        str.NotNull(nameof(str));

        return int.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Tries to convert the specified string representation of a number to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <param name="result">
    ///     When this method returns, contains the 32-bit signed integer value equivalent to the number
    ///     contained in <paramref name="str" />, if the conversion succeeded; otherwise, zero. This parameter is passed
    ///     uninitialized.
    /// </param>
    /// <returns>
    ///     true if the conversion succeeded; otherwise, false.
    /// </returns>
    public static bool TryConvertToInt32(this string str, out int result) => str.TryConvertToInt32(CultureInfo.InvariantCulture, out result);

    /// <summary>
    ///     Tries to convert the specified string representation of a number to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="str" />.</param>
    /// <param name="result">
    ///     When this method returns, contains the 32-bit signed integer value equivalent to the number
    ///     contained in <paramref name="str" />, if the conversion succeeded; otherwise, zero. This parameter is passed
    ///     uninitialized.
    /// </param>
    /// <returns>
    ///     true if the conversion succeeded; otherwise, false.
    /// </returns>
    public static bool TryConvertToInt32(this string str, IFormatProvider provider, out int result)
    {
        str.NotNull(nameof(str));

        return int.TryParse(str, NumberStyles.Integer, provider, out result);
    }

    /// <summary>
    ///     Converts the string representation of a number to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <returns>
    ///     The 32-bit signed integer equivalent to the number contained in the string.
    /// </returns>
    public static long ToInt64(this string str)
    {
        str.NotNull(nameof(str));

        return long.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Tries to convert the specified string representation of a number to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <param name="result">
    ///     When this method returns, contains the 32-bit signed integer value equivalent to the number
    ///     contained in <paramref name="str" />, if the conversion succeeded; otherwise, zero. This parameter is passed
    ///     uninitialized.
    /// </param>
    /// <returns>
    ///     true if the conversion succeeded; otherwise, false.
    /// </returns>
    public static bool TryConvertToInt64(this string str, out long result) => str.TryConvertToInt64(CultureInfo.InvariantCulture, out result);

    /// <summary>
    ///     Tries to convert the specified string representation of a number to its 32-bit signed integer equivalent.
    /// </summary>
    /// <param name="str">The string to convert.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="str" />.</param>
    /// <param name="result">
    ///     When this method returns, contains the 32-bit signed integer value equivalent to the number
    ///     contained in <paramref name="str" />, if the conversion succeeded; otherwise, zero. This parameter is passed
    ///     uninitialized.
    /// </param>
    /// <returns>
    ///     true if the conversion succeeded; otherwise, false.
    /// </returns>
    public static bool TryConvertToInt64(this string str, IFormatProvider provider, out long result)
    {
        str.NotNull(nameof(str));

        return long.TryParse(str, NumberStyles.Integer, provider, out result);
    }

    /// <summary>
    ///     Converts a given string to PascalCase format, transforming delimiters and capitalization
    ///     to align with PascalCase conventions.
    /// </summary>
    /// <param name="original">
    ///     The original string to be converted. This may include spaces, underscores, or other delimiters
    ///     and mixed-case characters.
    /// </param>
    /// <returns>
    ///     A new string in PascalCase where words or segments are capitalized and joined without delimiters,
    ///     adhering to identifier-naming conventions (e.g., "pascal_case_example" becomes "PascalCaseExample").
    /// </returns>
    public static string ToPascalCase(this string original)
    {
        var invalidCharsRgx = InvalidCharsRegex();
        var whiteSpace = WhiteSpaceRegex();
        var startsWithLowerCaseChar = StartsWithLowerCaseCharRegex();
        var firstCharFollowedByUpperCasesOnly = FirstCharFollowedByUpperCasesOnlyRegex();
        var lowerCaseNextToNumber = LowerCaseNextToNumberRegex();
        var upperCaseInside = UpperCaseInsideRegex();

        // replace white spaces with undescore, then replace all invalid chars with empty string
        var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(original, "_"), string.Empty)

                                        // split by underscores
                                        .Split(['_'], StringSplitOptions.RemoveEmptyEntries)

                                        // set the first letter to uppercase
                                        .Select(w => startsWithLowerCaseChar.Replace(w, static m => m.Value.ToUpperInvariant()))

                                        // replace the second and all following upper case letters to lower if there is no next lower (ABC -> Abc)
                                        .Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, static m => m.Value.ToLowerInvariant()))

                                        // set the upper case the first lower case following a number (Ab9cd -> Ab9Cd)
                                        .Select(w => lowerCaseNextToNumber.Replace(w, static m => m.Value.ToUpperInvariant()))

                                        // lower second and next upper case letters except the last if it follows by any lower (ABcDEf -> AbcDef)
                                        .Select(w => upperCaseInside.Replace(w, static m => m.Value.ToLowerInvariant()));

        return string.Concat(pascalCase);
    }
#if NET7_0_OR_GREATER
    [GeneratedRegex("[^_a-zA-Z0-9]")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex("(?<=\\s)")]
    private static partial Regex WhiteSpaceRegex();

    [GeneratedRegex("^[a-z]")]
    private static partial Regex StartsWithLowerCaseCharRegex();

    [GeneratedRegex("(?<=[A-Z])[A-Z0-9]+$")]
    private static partial Regex FirstCharFollowedByUpperCasesOnlyRegex();

    [GeneratedRegex("(?<=[0-9])[a-z]")]
    private static partial Regex LowerCaseNextToNumberRegex();

    [GeneratedRegex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))")]
    private static partial Regex UpperCaseInsideRegex();
#else
    private static readonly Regex InvalidCharsRegexField = new("[^_a-zA-Z0-9]");

    private static Regex InvalidCharsRegex() => InvalidCharsRegexField;

    private static readonly Regex WhiteSpaceRegexField = new("(?<=\\s)");

    private static Regex WhiteSpaceRegex() => WhiteSpaceRegexField;

    private static readonly Regex StartsWithLowerCaseCharRegexField = new("^[a-z]");

    private static Regex StartsWithLowerCaseCharRegex() => StartsWithLowerCaseCharRegexField;

    private static readonly Regex FirstCharFollowedByUpperCasesOnlyRegexField = new("(?<=[A-Z])[A-Z0-9]+$");

    private static Regex FirstCharFollowedByUpperCasesOnlyRegex() => FirstCharFollowedByUpperCasesOnlyRegexField;

    private static readonly Regex LowerCaseNextToNumberRegexField = new("(?<=[0-9])[a-z]");

    private static Regex LowerCaseNextToNumberRegex() => LowerCaseNextToNumberRegexField;

    private static readonly Regex UpperCaseInsideRegexField = new("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

    private static Regex UpperCaseInsideRegex() => UpperCaseInsideRegexField;
#endif
}
