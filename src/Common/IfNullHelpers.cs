namespace Ploch.Common;

/// <summary>
///     Provides extension methods for handling null values.
/// </summary>
public static class IfNullHelpers
{
    /// <summary>
    ///     Returns the specified value if it is not null; otherwise, returns the default value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to check for null.</param>
    /// <param name="defaultValue">The default value to return if the specified value is null.</param>
    /// <returns>The specified value if it is not null; otherwise, the default value.</returns>
    public static TValue OrIfNull<TValue>(this TValue? value, TValue defaultValue) => value ?? defaultValue;

    /// <summary>
    ///     Returns the specified enumerable if it is not null; otherwise, returns the default enumerable.
    /// </summary>
    /// <typeparam name="TEnumerable">The type of the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check for null.</param>
    /// <param name="defaultValue">The default enumerable to return if the specified enumerable is null.</param>
    /// <returns>The specified enumerable if it is not null; otherwise, the default enumerable.</returns>
    public static TEnumerable OrIfNullOrEmpty<TEnumerable>(this TEnumerable enumerable, TEnumerable defaultValue) => enumerable.OrIfNull(defaultValue);
}
