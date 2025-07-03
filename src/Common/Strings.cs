using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Provides a collection of commonly used string constants.
/// </summary>
public static class Strings
{
    /// <summary>
    ///     Represents a space character as a string (" ").
    /// </summary>
    public static readonly string Space = Spaces(1);

    /// <summary>
    ///     Represents an underscore character as a string ("_").
    /// </summary>
    public static readonly string Underscore = Underscores(1);

    /// <summary>
    ///     Represents a dash/hyphen character as a string ("-").
    /// </summary>
    public static readonly string Dash = Dashes(1);

    /// <summary>
    ///     Represents a dot/period character as a string (".").
    /// </summary>
    public static readonly string Dot = Dots(1);

    /// <summary>
    ///     Generates a string consisting of the specified number of space (' ') characters.
    /// </summary>
    /// <param name="count">The number of space characters to include in the string. Must be non-negative.</param>
    /// <returns>A string containing the specified number of space characters.</returns>
    public static string Spaces(int count) => new(Chars.Space, count.Positive(nameof(count)));

    /// <summary>
    ///     Generates a string consisting of the specified number of underscore ('_') characters.
    /// </summary>
    /// <param name="count">The number of underscore characters to include in the string. Must be non-negative.</param>
    /// <returns>A string containing the specified number of underscore characters.</returns>
    public static string Underscores(int count) => new(Chars.Underscore, count.Positive(nameof(count)));

    /// <summary>
    ///     Generates a string consisting of the specified number of dash ('-') characters.
    /// </summary>
    /// <param name="count">The number of dash characters to include in the string. Must be non-negative.</param>
    /// <returns>A string containing the specified number of dash characters.</returns>
    public static string Dashes(int count) => new(Chars.Dash, count.Positive(nameof(count)));

    /// <summary>
    ///     Generates a string consisting of the specified number of dot/period ('.') characters.
    /// </summary>
    /// <param name="count">The number of dot/period characters to include in the string. Must be non-negative.</param>
    /// <returns>A string containing the specified number of dot/period characters.</returns>
    public static string Dots(int count) => new(Chars.Dot, count.Positive(nameof(count)));
}
