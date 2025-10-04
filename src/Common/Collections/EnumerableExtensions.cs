// ReSharper disable ExceptionNotDocumented
// ReSharper disable PossibleMultipleEnumeration

#pragma warning disable S3267

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Collections;

/// <summary>
///     IEnumerable (and related) extensions.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Checks if a set of values the value using provided comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="comparer">The comparer.</param>
    /// <param name="values">The set of values.</param>
    /// <returns><c>true</c> if the set of values contains the value, <c>false</c> otherwise.</returns>
    public static bool ValueIn<TValue>(this TValue value, IEqualityComparer<TValue>? comparer, params TValue[] values) => ValueIn(value, values, comparer);

    /// <summary>
    ///     Checks if a set of values the value using default comparer.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="values">The set of values.</param>
    /// <returns><c>true</c> if the set of values contains the value, <c>false</c> otherwise.</returns>
    public static bool ValueIn<TValue>(this TValue value, params TValue[] values) => ValueIn(value, (IEnumerable<TValue>)values);

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
        // ReSharper disable once PossibleMultipleEnumeration
        values.NotNull(nameof(values));

        var actualComparer = comparer ?? EqualityComparer<TValue>.Default;
        if (comparer == null && values is ICollection<TValue> collection)
        {
            return collection.Contains(value);
        }

        // ReSharper disable once PossibleMultipleEnumeration
        return values.Any(item => actualComparer.Equals(item, value));
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
        predicate.NotNull(nameof(predicate));
        source.NotNull(nameof(source));

        return source.All(element => !predicate(element));
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
        return source.Join(separator, static v => v?.ToString());
    }

    /// <summary>
    ///     Joins the elements of the collection using the provided separator, calling <paramref name="valueSelector" /> on
    ///     each element.
    /// </summary>
    /// <param name="source">The source collection.</param>
    /// <param name="separator">The separator.</param>
    /// <param name="valueSelector">The selector of <typeparamref name="TValue" /> object property or an expression.</param>
    /// <typeparam name="TValue">The type of the collection element.</typeparam>
    /// <typeparam name="TResult">The resulting.</typeparam>
    /// <returns>String from joined elements.</returns>
    public static string Join<TValue, TResult>(this IEnumerable<TValue> source, string separator, Func<TValue, TResult> valueSelector) =>
        string.Join(separator, source.Select(valueSelector));

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
        return source.JoinWithFinalSeparator(separator, finalSeparator, static v => v?.ToString());
    }

    /// <summary>
    ///     Joins the elements of a sequence by a separator, with a final separator for the last two elements.
    /// </summary>
    /// <typeparam name="TValue">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TResult">The type of the result after applying the valueSelector function.</typeparam>
    /// <param name="source">The sequence to join.</param>
    /// <param name="separator">The separator to be used between elements.</param>
    /// <param name="finalSeparator">The separator to be used between the last two elements.</param>
    /// <param name="valueSelector">A function to select a result value from each element.</param>
    /// <returns>A string that consists of the joined elements with the separators.</returns>
    public static string JoinWithFinalSeparator<TValue, TResult>(this IEnumerable<TValue> source,
                                                                 string separator,
                                                                 string finalSeparator,
                                                                 Func<TValue, TResult> valueSelector)
    {
        source.NotNull(nameof(source));
        valueSelector.NotNull(nameof(valueSelector));
        var arraySource = source as TValue[] ?? source.ToArray();
        var count = arraySource.Length;

#pragma warning disable CC0031
        return arraySource.Take(count - 1).Join(separator, valueSelector) + finalSeparator + valueSelector(arraySource[arraySource.Length - 1]);
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
            var k = ThreadSafeRandom.Shared.Next(n + 1);
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

        var indexes = new List<int>(list.Count);
        for (var i = 0; i < list.Count; i++)
        {
            indexes.Add(i);
        }

        count = count > list.Count ? list.Count : count;

        for (var i = 0; i < count; i++)
        {
            var indexesItemNum = ThreadSafeRandom.Shared.Next(0, indexes.Count - 1);
            var itemIndex = indexes[indexesItemNum];

            result.Add(list[itemIndex]);
            indexes.RemoveAt(indexesItemNum);
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
    ///     If the <c>first</c> is not null, the query will be limited to the first <c>first</c> items.
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
    public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, bool condition, Func<IEnumerable<T>, IEnumerable<T>> action) =>
        If<IEnumerable<T>, T>(enumerable, condition, action);

    /// <summary>
    ///     Performs the specified action on each element of the <paramref name="enumerable" />.
    /// </summary>
    /// <param name="enumerable">The enumerable.</param>
    /// <param name="action">The action to perform on each element.</param>
    /// <typeparam name="T">The enumerable type.</typeparam>
    /// <returns>The same enumerable.</returns>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        action.NotNull(nameof(action));

        enumerable.NotNull(nameof(enumerable));

        foreach (var item in enumerable)
        {
#pragma warning disable CC0031 // already tested for null
            action(item);
#pragma warning restore CC0031
        }

        return enumerable;
    }

    /// <summary>
    ///     Determines whether the elements in the given enumerable are sequential.
    /// </summary>
    /// <param name="enumerable">
    ///     The enumerable to check for sequentiality. Must not be null.
    /// </param>
    /// <returns>
    ///     True if the elements in the enumerable are sequential; otherwise, false.
    /// </returns>
    public static bool AreIntegersSequentialInOrder(this IEnumerable<long> enumerable)
    {
        enumerable.NotNull(nameof(enumerable));
        var array = enumerable.Select(static v => v).ToArray();

        return array.Skip(1).Select((v, i) => v == array[i] + 1).All(static v => v);
    }

    /// <summary>
    ///     Determines whether the elements in the given enumerable are sequential.
    /// </summary>
    /// <param name="enumerable">
    ///     The enumerable to check for sequentiality. Must not be null.
    /// </param>
    /// <returns>
    ///     True if the elements in the enumerable are sequential; otherwise, false.
    /// </returns>
    public static bool AreIntegersSequentialInOrder(this IEnumerable<int> enumerable)
    {
        enumerable.NotNull(nameof(enumerable));
        var array = enumerable.Select(v => v).ToArray();

        return array.Skip(1).Select((v, i) => v == array[i] + 1).All(static v => v);
    }

    /// <summary>
    ///     Determines whether the specified enumerable is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <returns><c>true</c> if the enumerable is empty; otherwise, <c>false</c>.</returns>
    public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.NotNull(nameof(enumerable)).Any();

    /// <summary>
    ///     Determines whether the specified non-generic enumerable is empty.
    /// </summary>
    /// <param name="enumerable">The enumerable to check. Must not be null.</param>
    /// <returns><c>true</c> if the enumerable is empty (contains no elements); otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="enumerable" /> is null.</exception>
    public static bool IsEmpty(this IEnumerable enumerable)
    {
        var enumerator = enumerable.NotNull(nameof(enumerable)).GetEnumerator();

        using var disposable = enumerator as IDisposable;

        return !enumerator.MoveNext();
    }

    /// <summary>
    ///     Checks if the specified collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="enumerable">The collection to check.</param>
    /// <returns>true if the collection is null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? enumerable) => enumerable == null || enumerable.IsEmpty();

    /// <summary>
    ///     Returns the second element of a sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="enumerable">The sequence to return the second element from.</param>
    /// <returns>The second element in the specified sequence.</returns>
    /// <exception cref="InvalidOperationException">The source sequence contains fewer than two elements.</exception>
    public static T Second<T>(this IEnumerable<T> enumerable)
    {
        enumerable.NotNull(nameof(enumerable));

        // ReSharper disable once PossibleMultipleEnumeration - Guard is not enumerating
        return enumerable.Skip(1).First();
    }

    /// <summary>
    ///     Excludes the specified items from the source enumerable.
    /// </summary>
    /// <typeparam name="TItem">The type of elements in the enumerable.</typeparam>
    /// <param name="source">The source enumerable from which items will be excluded.</param>
    /// <param name="itemsToRemove">An array of items to exclude from the source enumerable.</param>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> that contains elements from the source enumerable
    ///     except for those elements that are present in <paramref name="itemsToRemove" />.
    /// </returns>
    public static IEnumerable<TItem> ExceptItems<TItem>(this IEnumerable<TItem> source, params TItem[] itemsToRemove) => source.Except(itemsToRemove);

    internal static TEnumerable If<TEnumerable, T>(this TEnumerable queryable, bool condition, Func<TEnumerable, TEnumerable> action)
        where TEnumerable : class, IEnumerable<T>
    {
        action.NotNull(nameof(action));
        queryable.NotNull(nameof(queryable));

        return condition ? action.Invoke(queryable) : queryable;
    }
}
