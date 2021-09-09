using System.Collections.Generic;
using Dawn;
using JetBrains.Annotations;

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
        public static bool ValueIn<TValue>(this TValue value, IEqualityComparer<TValue> comparer, [NotNull] params TValue[] values)
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
        public static bool ValueIn<TValue>(this TValue value, [NotNull] params TValue[] values)
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
        public static bool ValueIn<TValue>(this TValue value, [NotNull] IEnumerable<TValue> values, IEqualityComparer<TValue> comparer = null)
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
    }
}