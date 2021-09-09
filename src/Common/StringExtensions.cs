using System;
using System.Text;
using Dawn;
using JetBrains.Annotations;

namespace Ploch.Common
{
    /// <summary>
    ///     Extension methods for <see cref="string" /> and related.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Extension method version of <see cref="string.IsNullOrEmpty" />.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns><c>true</c> if string is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        ///     Encodes a string as base64 string using <see cref="Encoding.UTF8" />.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>Encoded version of supplied string.</returns>
        public static string ToBase64String(this string str)
        {
            return ToBase64String(str, Encoding.UTF8);
        }

        /// <summary>
        ///     Encodes a string as base64 string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>Encoded version of supplied string.</returns>
        public static string ToBase64String(this string str, Encoding encoding)
        {
            Guard.Argument(str, nameof(str)).NotNull();

            return Convert.ToBase64String(encoding.GetBytes(str));
        }

        public static string FromBase64String([NotNull] this string str)
        {
            return FromBase64String(str, Encoding.UTF8);
        }

        public static string FromBase64String([NotNull] this string str, Encoding encoding)
        {
            Guard.Argument(str, nameof(str)).NotNull();

            return encoding.GetString(Convert.FromBase64String(str));
        }

        public static bool EqualsIgnoreCase(this string str, string other, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str == null && other == null)
            {
                return true;
            }

            if (str == null || other == null)
            {
                return false;
            }

            return str.Equals(other, comparison);
        }

        public static string ReplaceStart([NotNull] this string str, [NotNull] string oldValue, [NotNull] string newValue, StringComparison stringComparison = StringComparison.InvariantCulture)
        {
            Guard.Argument(str, nameof(str)).NotNull();
            Guard.Argument(newValue, nameof(newValue)).NotNull();
            Guard.Argument(oldValue, nameof(oldValue)).NotNull();
            
            if (!str.StartsWith(oldValue, stringComparison))
            {
                return str;
            }

            return newValue + str.Substring(oldValue.Length, str.Length - oldValue.Length);
        }
    }
}