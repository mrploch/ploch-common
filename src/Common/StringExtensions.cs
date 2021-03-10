using Dawn;
using System;
using System.Text;

namespace Ploch.Common
{
    /// <summary>
    /// Extension methods for <see cref="String"/> and related.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Extension method version of <see cref="string.IsNullOrEmpty"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns><c>true</c> if string is <c>null</c> or empty; otherwise, <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Encodes a string as base64 string using <see cref="Encoding.UTF8"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>Encoded version of supplied string.</returns>
        public static string ToBase64String(this string str)
        {
            return ToBase64String(str, Encoding.UTF8);
        }

        /// <summary>
        /// Encodes a string as base64 string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>Encoded version of supplied string.</returns>
        public static string ToBase64String(this string str, Encoding encoding)
        {
            Guard.Argument(str, nameof(str)).NotNull();

            return Convert.ToBase64String(encoding.GetBytes(str));
        }
    }
}