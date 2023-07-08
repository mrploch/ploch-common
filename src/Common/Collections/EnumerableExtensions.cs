using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;

namespace Ploch.Common.Collections
{
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
                var k = RandomUtils.SharedRandom.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }

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
                    num = RandomUtils.SharedRandom.Next(0, list.Count - 1);
                } while (previousRandoms.Contains(num));

                previousRandoms.Add(num);

                result.Add(list[num]);
            }

            return result;
        }

        public static TValue FirstOrProvided<TValue>(this IEnumerable<TValue> source, Func<TValue> defaultValueFactory)
        {
            return FirstOrProvided(source, null, defaultValueFactory);
        }

        public static TValue FirstOrProvided<TValue>(this IEnumerable<TValue> source, Func<TValue, bool>? predicate, Func<TValue> defaultValueFactory)
        {
            Guard.Argument(source, nameof(source)).NotNull();
            Guard.Argument(defaultValueFactory, nameof(defaultValueFactory)).NotNull();

            return predicate == null ? source.FirstOrDefault() : source.FirstOrDefault(predicate) ?? defaultValueFactory();
        }
    }
}