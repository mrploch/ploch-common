﻿using System;
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
        /// <param name="comparer">The comparer or null to use the default comparer.</param>
        /// <param name="values">The set of values.</param>
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

        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            Guard.Argument(source, nameof(source)).NotNull();
            Guard.Argument(predicate, nameof(predicate)).NotNull();

            foreach (var element in source)
            {
                if (predicate(element))
                {
                    return false;
                }
            }

            return true;
        }

        public static string Join<TValue>(this IEnumerable<TValue> source, string separator)
        {
            return Join(source, separator, v => v.ToString());
        }

        public static string Join<TValue, TResult>(this IEnumerable<TValue> source, string separator, Func<TValue, TResult> valueSelector)
        {
            return string.Join(separator, source.Select(valueSelector));
        }

        public static string JoinWithFinalSeparator<TValue>(this IEnumerable<TValue> source, string separator, string finalSeparator)
        {
            return JoinWithFinalSeparator(source, separator, finalSeparator, v => v.ToString());
        }

        public static string JoinWithFinalSeparator<TValue, TResult>(this IEnumerable<TValue> source,
                                                                     string separator,
                                                                     string finalSeparator,
                                                                     Func<TValue, TResult> valueSelector)
        {
            var arraySource = source as TValue[] ?? source.ToArray();
            var count = arraySource.Length;

            return Join(arraySource.Take(count - 1), separator, valueSelector) + finalSeparator + valueSelector(arraySource.Last());
        }

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

            for (int i = 0; i < count; i++)
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