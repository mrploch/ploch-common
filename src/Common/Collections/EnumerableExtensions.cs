﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace Ploch.Common.Collections;

/// <summary>
///     IEnumerable (and related) extensions.
/// </summary>
public static class EnumerableExtensions
{
    private static readonly Random Random = new();

    /// <summary>
    ///     Checks if a set of values the value using provided comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="comparer">The comparer.</param>
    /// <param name="values">The set of values.</param>
    /// <returns><c>true</c> if the set of values contains the value, <c>false</c> otherwise.</returns>
    public static bool ValueIn<TValue>(this TValue value, IEqualityComparer<TValue>? comparer, params TValue[] values)
    {
        return ValueIn(value, values, comparer);
    }

    /// <summary>
    ///     Checks if a set of values the value using default comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">The set of values.</param>
    /// <returns><c>true</c> if the set of values contains the value, <c>false</c> otherwise.</returns>
    public static bool ValueIn<TValue>(this TValue value, params TValue[] values)
    {
        return ValueIn(value, (IEnumerable<TValue>)values);
    }

    /// <summary>
    ///     Checks if a set of values the value using provided comparer (or default comparer if null).
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">The set of values.</param>
    /// <param name="comparer">The comparer or null to use the default comparer.</param>
    /// <returns><c>true</c> if the set of values contains the value, <c>false</c> otherwise.</returns>
    public static bool ValueIn<TValue>(this TValue value, IEnumerable<TValue> values, IEqualityComparer<TValue>? comparer = null)
    {
        Guard.Argument(values, nameof(values)).NotNull();

        var actualComparer = comparer ?? EqualityComparer<TValue>.Default;
        if (comparer == null && values is ICollection<TValue> collection)
        {
            return collection.Contains(value);
        }

        foreach (var item in values)
        {
            if (actualComparer.Equals(item, value))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Verifies that none of the items in the collection matches the predicate.
    /// </summary>
    /// <param name="source">The source collection.</param>
    /// <param name="predicate">The predicate.</param>
    /// <typeparam name="TSource">Collection item type.</typeparam>
    /// <returns><c>true</c> if none of the items matched the predicate,otherwise <c>false</c>.</returns>
    public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        Guard.Argument(predicate, nameof(predicate)).NotNull();
        Guard.Argument(source, nameof(source)).NotNull();

        foreach (var element in source)
        {
            if (predicate(element))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Joins the elements of the collection using the provided separator, calling <c>ToString</c> on each element of the
    ///     collection.
    /// </summary>
    /// <param name="source">The source collection.</param>
    /// <param name="separator">The separator.</param>
    /// <typeparam name="TValue">The type of the collection element.</typeparam>
    /// <returns>String from joined elements.</returns>
    public static string Join<TValue>(this IEnumerable<TValue> source, string separator)
    {
        return Join(source, separator, v => v?.ToString());
    }

    /// <summary>
    ///     Joins the elements of the collection using the provided separator, calling <paramref name="valueSelector" /> on
    ///     each element.
    /// </summary>
    /// <param name="source">The source collection.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="valueSelector">The selector of <typeparamref name="TValue" /> object property or an expression.</param>
    /// <typeparam name="TValue">The type of the collection element.</typeparam>
    /// <typeparam name="TResult">The resulting </typeparam>
    /// <returns>String from joined elements.</returns>
    public static string Join<TValue, TResult>(this IEnumerable<TValue> source, string separator, Func<TValue, TResult> valueSelector)
    {
        return string.Join(separator, source.Select(valueSelector));
    }

    /// <summary>
    ///     Joins the elements of the collection using the provided separator, calling <c>ToString</c> on each element of the
    ///     collection. The last element is joined using the <paramref name="finalSeparator" />.
    /// </summary>
    /// <param name="source">The source collection.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="finalSeparator">The final separator.</param>
    /// <typeparam name="TValue">The type of the collection element.</typeparam>
    /// <returns>String from joined elements.</returns>
    public static string JoinWithFinalSeparator<TValue>(this IEnumerable<TValue> source, string separator, string finalSeparator)
    {
        return JoinWithFinalSeparator(source, separator, finalSeparator, v => v?.ToString());
    }

    public static string JoinWithFinalSeparator<TValue, TResult>(this IEnumerable<TValue> source,
                                                                 string separator,
                                                                 string finalSeparator,
                                                                 Func<TValue, TResult> valueSelector)
    {
        Guard.Argument(source, nameof(source)).NotNull();
        Guard.Argument(valueSelector, nameof(valueSelector)).NotNull();
        var arraySource = source as TValue[] ?? source.ToArray();
        var count = arraySource.Length;

#pragma warning disable CC0031
        return Join(arraySource.Take(count - 1), separator, valueSelector) + finalSeparator + valueSelector(arraySource[arraySource.Length - 1]);
#pragma warning restore CC0031
    }

    /// <summary>
    ///     Randomly shuffles the elements of the source enumerable.
    /// </summary>
    /// <param name="source">The collection to shuffle.</param>
    /// <typeparam name="TValue">The type of values in the enumerable.</typeparam>
    /// <returns>Randomly shuffled enumerable.</returns>
    public static IEnumerable<TValue> Shuffle<TValue>(this IEnumerable<TValue> source)
    {
        var list = source.ToList();
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }

        return list;
    }

    /// <summary>
    ///     Takes random <paramref name="count" /> amount of items from the <paramref name="source" /> enumerable.
    /// </summary>
    /// <param name="source">The source enumerable.</param>
    /// <param name="count">The number of values to take.</param>
    /// <typeparam name="TValue">The enumerable value type.</typeparam>
    /// <returns>The random items from the source enumerable.</returns>
    public static IEnumerable<TValue> TakeRandom<TValue>(this IEnumerable<TValue> source, int count)
    {
        var list = source.ToList();

        var result = new List<TValue>();
        var previousRandoms = new List<int>();

        for (var i = 0; i < count; i++)
        {
            int num;
            do
            {
                num = Random.Next(0, list.Count - 1);
            } while (previousRandoms.Contains(num));

            previousRandoms.Add(num);

            result.Add(list[num]);
        }

        return result;
    }

    /// <summary>
    ///     Creates an enumerable query if the condition is true.
    /// </summary>
    /// <remarks>
    ///     This method is useful when you want to conditionally add a where clause to a query.
    /// </remarks>
    /// <example>
    ///     Method below returns a list of cars ordered by creation date.
    ///     If the <c>createdAfter</c> is not null, the query will be filtered by the creation date.
    ///     If the <c>createdBefore</c> is not null, the query will be further filtered by the creation date.
    ///     <code>
    /// var carsList = GetCars();
    /// carsList.OrderBy(x =&gt; x.Created)
    /// .If(createdAfter.HasValue, x =&gt; x.Where(y =&gt; y.Created &gt; createdAfter!.Value))
    /// .If(createdBefore.HasValue, x =&gt; x.Where(y =&gt; y.Created &lt; createdBefore!.Value))
    /// .If(first.HasValue, x =&gt; x.Take(first!.Value))
    /// </code>
    /// </example>
    /// <param name="enumerable">The source enumerable.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The query action to perform on <paramref name="enumerable" />.</param>
    /// <typeparam name="T">The enumerable value type.</typeparam>
    /// <returns>The resulting enumerable.</returns>
    public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, bool condition, Func<IEnumerable<T>, IEnumerable<T>> action)
    {
        return If<IEnumerable<T>, T>(enumerable, condition, action);
    }

    internal static TEnumerable If<TEnumerable, T>(this TEnumerable queryable, bool condition, Func<TEnumerable, TEnumerable> action)
        where TEnumerable : class, IEnumerable<T>
    {
        Guard.Argument(action, nameof(action)).NotNull();
        Guard.Argument(queryable, nameof(queryable)).NotNull();

        return condition ? action!(queryable) : queryable;
    }

    /// <summary>
    ///     Performs the specified action on each element of the <paramref name="enumerable" />.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action to perform on each element.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The same enumerable.</returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        Guard.Argument(action, nameof(action)).NotNull();
        Guard.Argument(enumerable, nameof(enumerable)).NotNull();

        foreach (var item in enumerable)
        {
            action(item);
        }

        return enumerable;
    }
}