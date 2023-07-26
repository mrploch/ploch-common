using System.Collections.Generic;
using Dawn;

namespace Ploch.Common
{
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
        public static bool NotIn<TValue>(this TValue value, params TValue[] values)
        {
            return NotIn(value, (IEnumerable<TValue>)values);
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
        public static bool NotIn<TValue>(this TValue value, IEnumerable<TValue> values)
        {
            return !In(value, values);
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
        public static bool In<TValue>(this TValue value, params TValue[] values)
        {
            return In(value, (IEnumerable<TValue>)values);
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
        public static bool In<TValue>(this TValue? value, IEnumerable<TValue> values)
        {
            Guard.Argument(values).NotNull();

            if (value.IsDefault())
            {
                return false;
            }

            foreach (var v in values)
            {
                if (value.IsNotDefault() && value!.Equals(v))
                {
                    return true;
                }
            }

            return false;
        }
    }
}