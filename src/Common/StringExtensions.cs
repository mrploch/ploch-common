using System;
using System.Globalization;
using System.Text;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Extension methods for <see cref="string" /> and related.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Extension method that determines if a string is not <c>null</c> or empty.
    /// </summary>
    /// <param name="str">The string to check.</param>
    /// <returns><c>true</c> if the string is not <c>null</c> or empty; otherwise, <c>false</c>.</returns>
#pragma warning disable SA1202
    public static bool IsNotNullOrEmpty(this string? str) => !str.IsNullOrEmpty();
#pragma warning restore SA1202

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
    ///     Encodes a string as a base64 string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>Encoded version of supplied string.</returns>
    public static string ToBase64String(this string str, Encoding encoding)
    {
#pragma warning disable S3236
        str.NotNull(nameof(str));
#pragma warning restore S3236

        return Convert.ToBase64String(encoding.NotNull(nameof(encoding)).GetBytes(str));
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

        return encoding.NotNull(nameof(encoding)).GetString(Convert.FromBase64String(str));
    }

    /// <summary>
    ///     Compare two strings ignoring the case.
    /// </summary>
    /// <param name="str">The first string to compare.</param>
    /// <param name="other">The second string to compare.</param>
    /// <returns><c>true</c> if strings are equal ignoring the case, <c>false</c> otherwise.</returns>
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
    ///     <paramref name="oldValue"></paramref>
    ///     with
    ///     <paramref name="newValue"></paramref>
    ///     in the string
    ///     <paramref name="str"></paramref>
    ///     if the string starts with
    ///     <paramref name="oldValue"></paramref>
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

        return int.Parse(str, CultureInfo.InvariantCulture);
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

        return long.Parse(str, CultureInfo.InvariantCulture);
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
}
