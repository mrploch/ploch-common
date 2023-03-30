using System;
using System.Text;
using Dawn;

namespace Ploch.Common
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendIfNotNull<TValue>(this StringBuilder builder, TValue? value, Func<TValue, string>? formatFunc = null)
        {
            return AppendIf(builder, value, static v => !Equals(v, default), formatFunc!);
        }

        public static StringBuilder AppendIfNotNullOrEmpty<TValue>(this StringBuilder builder, TValue? value, Func<TValue, string>? formatFunc = null)
        {
            return AppendIf(builder, value, static v => !Equals(v, default) && !v.ToString().IsNullOrEmpty(), formatFunc!);
        }

        private static StringBuilder AppendIf<TValue>(this StringBuilder builder, TValue? value, Func<TValue?, bool> test, Func<TValue?, string>? formatFunc = null)
        {
            Guard.Argument(test, nameof(test)).NotNull();
            Guard.Argument(builder, nameof(builder)).NotNull();

            // It is already checked by Guard
#pragma warning disable CC0031
            if (test(value))
#pragma warning restore CC0031
            {
                builder.Append(formatFunc != null ? formatFunc(value) : value);
            }

            return builder;
        }
    }
}