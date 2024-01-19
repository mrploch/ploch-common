using System;
using System.Text;
using Dawn;

namespace Ploch.Common;

/// <summary>
///     Extension methods for <see cref="string" /> and related.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Extension method version of <see cref="string.IsNullOrEmpty" />.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns><c>true</c> if string is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    ///     Encodes a string as base64 string using <see cref="Encoding.UTF8" />.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <returns>Encoded version of supplied string.</returns>
    public static string ToBase64String(this string str)
    {
        return ToBase64String(str, Encoding.UTF8);
    }

    /// <summary>
    ///     Encodes a string as base64 string.
    /// </summary>
    /// <param name="str">The string.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>Encoded version of supplied string.</returns>
    public static string ToBase64String(this string str, Encoding encoding)
    {
        Guard.Argument(str, nameof(str)).NotNull();

        return Convert.ToBase64String(encoding.GetBytes(str));
    }

    /// <summary>
    ///     Decodes a base64 string using <see cref="Encoding.UTF8" /> encoding.
    /// </summary>
    /// <param name="str">The base64 encoded string.</param>
    /// <returns>The decoded base64 string.</returns>
    public static string FromBase64String(this string str)
    {
        return FromBase64String(str, Encoding.UTF8);
    }

    /// <summary>
    ///     Decodes a base64 string using provided encoding.
    /// </summary>
    /// <param name="str">The base64 encoded string.</param>
    /// <param name="encoding">The <paramref name="str" /> encoding.</param>
    /// <returns>The decoded base64 string.</returns>
    public static string FromBase64String(this string str, Encoding encoding)
    {
        Guard.Argument(str, nameof(str)).NotNull();

        return encoding.GetString(Convert.FromBase64String(str));
    }

    /// <summary>
    ///     Compare two strings ignoring case.
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
        Guard.Argument(str, nameof(str)).NotNull();
        Guard.Argument(newValue, nameof(newValue)).NotNull();
        Guard.Argument(oldValue, nameof(oldValue)).NotNull();

        if (!str.StartsWith(oldValue, stringComparison))
        {
            return str;
        }

        return newValue + str.Substring(oldValue.Length, str.Length - oldValue.Length);
    }
}