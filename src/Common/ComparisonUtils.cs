using System.Collections.Generic;

namespace Ploch.Common
{
    /// <summary>
    ///     Comparison utility methods.
    /// </summary>
    public static class ComparisonUtils
    {
        /// <summary>
        ///     Checks if the value is not equal to default (or null) value of the type.
        /// </summary>
        /// <remarks>
        ///     Checks if a value type is not the default value (0 for int, false for bool, etc.) or if a reference type is not
        ///     null.
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>
        ///     <c>true</c> if the value is default or null, otherwise <c>false</c>
        /// </returns>
        public static bool IsNotDefault<TValue>(this TValue? value)
        {
            return !value.IsDefault();
        }

        /// <summary>
        ///     Checks if the value is equal to default (or null) value of the type.
        /// </summary>
        /// <remarks>
        ///     Checks if a value type is the default value (0 for int, false for bool, etc.) or if a reference type is
        ///     null.
        /// </remarks>
        /// <param name="value">The value.</param>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>
        ///     <c>false</c> if the value is default or null, otherwise <c>true</c>
        /// </returns>
        public static bool IsDefault<TValue>(this TValue? value)
        {
            return EqualityComparer<TValue>.Default.Equals(value, default);
        }
    }
}