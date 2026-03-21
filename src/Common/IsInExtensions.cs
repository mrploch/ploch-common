using System.Collections.Generic;
using System.Linq;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Class IsInExtensions.
/// </summary>
public static class IsInExtensions
{
    /// <summary>
    ///     Checks if the <paramref name="value" /> is equal to one of the <paramref name="values" /> provided.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is equal to one of the <paramref name="values" />, <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool In<TValue>(this TValue value, params TValue[] values) => value.In((IEnumerable<TValue>)values);

    /// <summary>
    ///     Checks if the <paramref name="value" /> is equal to one of the <paramref name="values" /> provided.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is equal to one of the <paramref name="values" />, <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool In<TValue>(this TValue? value, IEnumerable<TValue?> values)
    {
        values.NotNull(nameof(values));

        return values.Any(v => EqualityComparer<TValue?>.Default.Equals(value, v));
    }

    /// <summary>
    ///     Checks if the <paramref name="value" /> is equal to one of the <paramref name="values" /> provided using the specified comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="comparer">The comparer to use for equality comparison.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is equal to one of the <paramref name="values" /> according to the <paramref name="comparer" />, <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool In<TValue>(this TValue? value, IComparer<TValue?> comparer, params TValue[] values) => value.In(comparer, (IEnumerable<TValue>)values);

    /// <summary>
    ///     Checks if the <paramref name="value" /> is equal to one of the <paramref name="values" /> provided using the specified comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="comparer">The comparer to use for equality comparison.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is equal to one of the <paramref name="values" /> according to the <paramref name="comparer" />, <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool In<TValue>(this TValue? value, IComparer<TValue?> comparer, IEnumerable<TValue?> values)
    {
        values.NotNull(nameof(values));

        return values.Any(v => comparer.Compare(v, value) == 0);
    }

    /// <summary>
    ///     Checks if the <paramref name="value" /> is equal to one of the <paramref name="values" /> provided.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is equal to one of the <paramref name="values" />, <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool NotIn<TValue>(this TValue value, params TValue[] values) => value.NotIn((IEnumerable<TValue>)values);

    /// <summary>
    ///     Checks if the <paramref name="value" /> is equal to one of the <paramref name="values" /> provided.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is equal to one of the <paramref name="values" />, <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool NotIn<TValue>(this TValue value, IEnumerable<TValue> values) => !value.In(values);

    /// <summary>
    ///     Checks if the <paramref name="value" /> is not equal to any of the <paramref name="values" /> provided using the specified comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="comparer">The comparer to use for equality comparison.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is not equal to any of the <paramref name="values" /> according to the <paramref name="comparer" />,
    ///     <c>false</c> otherwise.
    /// </returns>
    public static bool NotIn<TValue>(this TValue? value, IComparer<TValue?> comparer, params TValue[] values) =>
        value.NotIn(comparer, (IEnumerable<TValue>)values);

    /// <summary>
    ///     Checks if the <paramref name="value" /> is not equal to any of the <paramref name="values" /> provided using the specified comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="comparer">The comparer to use for equality comparison.</param>
    /// <param name="values">Values to try match against.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="value" /> is not equal to any of the <paramref name="values" /> according to the <paramref name="comparer" />,
    ///     <c>false</c>
    ///     otherwise.
    /// </returns>
    public static bool NotIn<TValue>(this TValue? value, IComparer<TValue?> comparer, IEnumerable<TValue?> values) => !value.In(comparer, values);
}
