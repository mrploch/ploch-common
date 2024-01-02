using System;
using System.Text;
using Dawn;

namespace Ploch.Common;

/// <summary>
///     Extension methods for <see cref="StringBuilder" />
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    ///     Appends the <paramref name="value" /> to the <paramref name="builder" /> if the <paramref name="value" /> is not
    ///     null.
    /// </summary>
    /// <param name="builder">The <c>StringBuilder</c></param>
    /// <param name="value">The value to append.</param>
    /// <param name="formatFunc">Optional formatting function.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>The source <c>StringBuilder</c>.</returns>
    public static StringBuilder AppendIfNotNull<TValue>(this StringBuilder builder, TValue? value, Func<TValue?, string>? formatFunc = null)
    {
        return AppendIf(builder, value, static v => !Equals(v, default), formatFunc);
    }

    /// <summary>
    ///     Appends the <paramref name="value" /> to the <paramref name="builder" /> if the <paramref name="value" /> is not
    ///     null or empty string.
    /// </summary>
    /// <param name="builder">The <c>StringBuilder</c></param>
    /// <param name="value">The value to append.</param>
    /// <param name="formatFunc">Optional formatting function.</param>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <returns>The source <c>StringBuilder</c>.</returns>
    public static StringBuilder AppendIfNotNullOrEmpty<TValue>(this StringBuilder builder, TValue? value, Func<TValue?, string>? formatFunc = null)
    {
        return AppendIf(builder, value, static v => !Equals(v, default) && !v.ToString().IsNullOrEmpty(), formatFunc);
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