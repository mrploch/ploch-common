using System;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Provides extension methods for converting string values to enumeration types.
/// </summary>
/// <remarks>
///     This class contains utility methods that simplify the conversion of string values to enum types,
///     with options for handling null values and case sensitivity.
/// </remarks>
/// <example>
///     <code lang="csharp">
/// // Example enum
/// public enum Color { Red, Green, Blue }
/// 
/// // Basic parsing with exception on failure
/// Color color1 = "Red".ParseToEnum&lt;Color&gt;();
/// Color color2 = "red".ParseToEnum&lt;Color&gt;(ignoreCase: true);
/// 
/// // This would throw an ArgumentOutOfRangeException:
/// // Color invalidColor = "Yellow".ParseToEnum&lt;Color&gt;();
/// 
/// // Safe parsing that returns null on failure
/// Color? color3 = "Blue".SafeParseToEnum&lt;Color&gt;();
/// Color? color4 = "yellow".SafeParseToEnum&lt;Color&gt;(); // Returns null
/// Color? color5 = null.SafeParseToEnum&lt;Color&gt;();     // Returns null
/// </code>
/// </example>
public static class EnumerationConverter
{
    /// <summary>
    ///     Converts a string value to the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to convert to.</typeparam>
    /// <param name="value">The string value to convert. Cannot be null or empty.</param>
    /// <param name="ignoreCase">If true, ignores case when converting the string; otherwise, exact case matching is used. Default is false.</param>
    /// <returns>The enumeration value that corresponds to the string value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the string value cannot be converted to the specified enumeration type.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the value is null or empty.</exception>
    /// <example>
    ///     <code lang="csharp">
    /// public enum Status { Active, Inactive, Pending }
    /// 
    /// // Case-sensitive parsing
    /// Status status1 = "Active".ParseToEnum&lt;Status&gt;();
    /// 
    /// // Case-insensitive parsing
    /// Status status2 = "active".ParseToEnum&lt;Status&gt;(ignoreCase: true);
    /// </code>
    /// </example>
    public static TEnum ParseToEnum<TEnum>(this string value, bool ignoreCase = false) where TEnum : struct, Enum
    {
        if (Enum.TryParse<TEnum>(value.NotNullOrEmpty(nameof(value)), ignoreCase, out var result))
        {
            return result;
        }

        throw new ArgumentOutOfRangeException(nameof(value), value, $"'{value}' is not a valid value for enum type '{typeof(TEnum).Name}'.");
    }

    /// <summary>
    ///     Converts an integer value to the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to convert to.</typeparam>
    /// <param name="enumValue">The integer value to convert to the enumeration.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information for the conversion. Default is null.</param>
    /// <returns>The enumeration value that corresponds to the integer value.</returns>
    /// <remarks>
    ///     This method converts the integer to a string and then uses the string-based ParseToEnum method.
    ///     It will throw an exception if the integer value doesn't correspond to a defined value in the enum.
    /// </remarks>
    /// <example>
    ///     <code lang="csharp">
    /// public enum Status { Active = 1, Inactive = 2, Pending = 3 }
    /// 
    /// // Convert integer to enum
    /// Status status1 = 1.ParseToEnum&lt;Status&gt;();  // Returns Status.Active
    /// Status status2 = 2.ParseToEnum&lt;Status&gt;();  // Returns Status.Inactive
    /// 
    /// // Using with integer variables
    /// int statusCode = GetStatusCodeFromDatabase();
    /// Status status = statusCode.ParseToEnum&lt;Status&gt;();
    /// 
    /// // This would throw an ArgumentOutOfRangeException:
    /// // Status invalidStatus = 99.ParseToEnum&lt;Status&gt;();
    /// </code>
    /// </example>
    public static TEnum ParseToEnum<TEnum>(this int enumValue, IFormatProvider? formatProvider = null) where TEnum : struct, Enum =>
        enumValue.ToString(formatProvider).ParseToEnum<TEnum>();

    /// <summary>
    ///     Safely attempts to convert a string value to the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to convert to.</typeparam>
    /// <param name="value">The string value to convert. Can be null or empty.</param>
    /// <param name="ignoreCase">If true, ignores case when converting the string; otherwise, exact case matching is used. Default is false.</param>
    /// <returns>The enumeration value that corresponds to the string value, or null if the conversion fails or the input is null or whitespace.</returns>
    /// <example>
    ///     <code lang="csharp">
    /// public enum Priority { Low, Medium, High }
    /// 
    /// // Successful conversion
    /// Priority? priority1 = "Medium".SafeParseToEnum&lt;Priority&gt;();  // Returns Priority.Medium
    /// 
    /// // Failed conversion returns null
    /// Priority? priority2 = "Critical".SafeParseToEnum&lt;Priority&gt;(); // Returns null
    /// 
    /// // Null input returns null
    /// Priority? priority3 = null.SafeParseToEnum&lt;Priority&gt;();       // Returns null
    /// 
    /// // Case-insensitive conversion
    /// Priority? priority4 = "high".SafeParseToEnum&lt;Priority&gt;(true); // Returns Priority.High
    /// </code>
    /// </example>
    public static TEnum? SafeParseToEnum<TEnum>(this string? value, bool ignoreCase = false) where TEnum : struct, Enum
    {
        if (value.IsNullOrWhiteSpace())
        {
            return null;
        }

        if (Enum.TryParse<TEnum>(value, ignoreCase, out var result))
        {
            return result;
        }

        return null;
    }

    /// <summary>
    ///     Safely attempts to convert a nullable integer value to the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type to convert to.</typeparam>
    /// <param name="enumValue">The nullable integer value to convert to the enumeration.</param>
    /// <param name="formatProvider">An object that supplies culture-specific formatting information for the conversion. Default is null.</param>
    /// <returns>
    ///     The enumeration value that corresponds to the integer value, or null if the input is null
    ///     or the integer value doesn't correspond to a defined value in the enum.
    /// </returns>
    /// <example>
    ///     <code lang="csharp">
    /// public enum Status { Active = 1, Inactive = 2, Pending = 3 }
    /// 
    /// // Successful conversion
    /// Status? status1 = 1.SafaParseToEnum&lt;Status&gt;();  // Returns Status.Active
    /// 
    /// // Failed conversion returns null
    /// Status? status2 = 99.SafaParseToEnum&lt;Status&gt;(); // Returns null
    /// 
    /// // Null input returns null
    /// int? nullValue = null;
    /// Status? status3 = nullValue.SafaParseToEnum&lt;Status&gt;(); // Returns null
    /// </code>
    /// </example>
    public static TEnum? SafaParseToEnum<TEnum>(this int? enumValue, IFormatProvider? formatProvider = null) where TEnum : struct, Enum =>
        enumValue?.ToString(formatProvider).ParseToEnum<TEnum>();
}
