using Dawn;
using System;
using System.Text;

namespace Ploch.Common
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static string ToBase64String(this string str, Encoding encoding)
        {
            Guard.Argument(str, nameof(str)).NotNull();

            return Convert.ToBase64String(encoding.GetBytes(str));
        }
    }
}