namespace Ploch.Common;

/// <summary>
///     Provides extension methods for parsing strings into different data types with nullability support.
/// </summary>
public static class StringParsingExtensions
{
    /// <summary>
    ///     Attempts to parse the specified string as a boolean value.
    /// </summary>
    /// <param name="str">The string to parse. Can be <c>null</c> or whitespace.</param>
    /// <returns>
    ///     A nullable boolean that represents the parsed value. Returns <c>true</c> or <c>false</c> if parsing is successful.
    ///     Returns <c>null</c> if the string is <c>null</c>, whitespace, or cannot be parsed as a valid boolean value.
    /// </returns>
    public static bool? ParseToBool(this string? str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return null;
        }

        return bool.TryParse(str, out var result) ? result : null;
    }

    /// <summary>
    ///     Attempts to parse the specified string as a 32-bit integer value.
    /// </summary>
    /// <param name="str">The string to parse. Can be <c>null</c> or whitespace.</param>
    /// <returns>
    ///     A nullable 32-bit integer that represents the parsed value. Returns a valid integer if parsing is successful.
    ///     Returns <c>null</c> if the string is <c>null</c>, whitespace, or cannot be parsed as a valid 32-bit integer value.
    /// </returns>
    public static int? ParseToInt32(this string? str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return null;
        }

        return int.TryParse(str, out var result) ? result : null;
    }

    /// <summary>
    ///     Attempts to parse the specified string as a 64-bit signed integer.
    /// </summary>
    /// <param name="str">The string to parse. Can be <c>null</c>, empty, or consist of only whitespace.</param>
    /// <returns>
    ///     A nullable <c>long</c> representing the parsed value. Returns the parsed 64-bit integer if parsing is successful.
    ///     Returns <c>null</c> if the string is <c>null</c>, whitespace, or cannot be parsed as a valid 64-bit integer (e.g., due to invalid format or overflow).
    /// </returns>
    public static long? ParseToInt64(this string? str)
    {
        if (str.IsNullOrWhiteSpace())
        {
            return null;
        }

        return long.TryParse(str, out var result) ? result : null;
    }
}
