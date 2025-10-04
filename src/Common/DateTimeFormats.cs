namespace Ploch.Common;

/// <summary>
///     The <c>DateTimeFormats</c> class provides a collection of standardized date and time format strings
///     for use in formatting or parsing date and time values in various formats.
/// </summary>
/// <remarks>
///     This class includes both general date-time formats and specific date-only formats,
///     making it useful for scenarios where consistency in date and time string representation is required.
/// </remarks>
public class DateTimeFormats
{
    /// <summary>
    ///     A constant string representing the date-time format "yyyyMMddHHmmss".
    /// </summary>
    /// <remarks>
    ///     This format is used to represent a full date and time as a continuous sequence
    ///     of numeric characters without any separators. The components are in the following order:
    ///     Year (4 digits), Month (2 digits), Day (2 digits), Hour (2 digits - 24-hour format),
    ///     Minute (2 digits), and Second (2 digits).
    /// </remarks>
    /// <example>
    ///     For example, a date-time value of <c>2023-10-15 14:30:45</c> will be represented as <c>20231015143045</c>
    ///     when formatted using this string.
    /// </example>
    public const string YearMonthDayHourMinuteSecondNumbersOnly = "yyyyMMddHHmmss";

    /// <summary>
    ///     A constant string representing the date-time format "yyyy-MM-dd-HH-mm-ss".
    /// </summary>
    /// <remarks>
    ///     This format is used to represent a full date and time with dashes separating the components.
    ///     The components are in the following order:
    ///     Year (4 digits), Month (2 digits), Day (2 digits), Hour (2 digits - 24-hour format),
    ///     Minute (2 digits), and Second (2 digits).
    /// </remarks>
    /// <example>
    ///     For example, a date-time value of <c>2023-10-15 14:30:45</c> will be represented as <c>2023-10-15-14-30-45</c>
    ///     when formatted using this string.
    /// </example>
    public const string YearMonthDayHourMinuteSecondNumbersWithDashes = "yyyy-MM-dd-HH-mm-ss";

    /// <summary>
    ///     A constant string representing the date-time format "yyyy-MM-dd HH:mm:ss".
    /// </summary>
    /// <remarks>
    ///     This format is used to represent a full date and time with dashed separators between the
    ///     year, month, and day, and colons separating the hour, minute, and second components.
    ///     The components are in the following order:
    ///     Year (4 digits), Month (2 digits), Day (2 digits), Hour (2 digits - 24-hour format),
    ///     Minute (2 digits), and Second (2 digits).
    /// </remarks>
    /// <example>
    ///     For example, a date-time value of <c>2023-10-15 14:30:45</c> will be represented as <c>"2023-10-15 14:30:45"</c>
    ///     when formatted using this string.
    /// </example>
    public const string YearMonthDayHourMinuteSecondNumbersWithDashesAndColons = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    ///     The <c>DateOnly</c> class provides a collection of standardized date-only format strings
    ///     for use in formatting or parsing date values in various formats.
    /// </summary>
    /// <remarks>
    ///     This class focuses on date-only representations and is designed to ensure consistency when working with
    ///     date strings in both compact and human-readable formats. It includes formats both with and without delimiters.
    /// </remarks>
    public static class DateOnly
    {
        /// <summary>
        ///     A constant string representing the date format "yyyyMMdd".
        /// </summary>
        /// <remarks>
        ///     This format is used to represent a date as a continuous sequence of numeric characters
        ///     without any separators. The components are in the following order:
        ///     Year (4 digits), Month (2 digits), and Day (2 digits).
        /// </remarks>
        /// <example>
        ///     For example, a date value of <c>2023-10-15</c> will be represented as <c>20231015</c>
        ///     when formatted using this string.
        /// </example>
        public const string YearMonthDayNumbersOnly = "yyyyMMdd";

        /// <summary>
        ///     A constant string representing the date format "yyyy-MM-dd".
        /// </summary>
        /// <remarks>
        ///     This format is used to represent a date in a human-readable form with dashes as separators.
        ///     The components are in the following order: Year (4 digits), Month (2 digits), and Day (2 digits),
        ///     separated by dashes (<c>-</c>).
        /// </remarks>
        /// <example>
        ///     For example, a date value of <c>2023-10-15</c> will be represented as <c>"2023-10-15"</c>
        ///     when formatted using this string.
        /// </example>
        public const string YearMonthDayNumbersWithDashes = "yyyy-MM-dd";
    }
}
