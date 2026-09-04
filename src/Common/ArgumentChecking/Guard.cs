// ReSharper disable RedundantUsingDirective

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Ploch.Common.ArgumentChecking;

/// <summary>
///     Provides guard clause extension methods for validating method parameters and enforcing preconditions.
///     These methods help to improve code reliability by catching invalid arguments early.
/// </summary>
public static partial class Guard
{
    /// <summary>
    ///     Ensures that the given boolean condition evaluates to <c>false</c>. Throws an <see cref="InvalidOperationException" /> if the condition is <c>true</c>.
    ///     This method is typically used as a guard clause to validate method preconditions or other conditions.
    /// </summary>
    /// <param name="argument">The boolean condition to evaluate. Must be <c>false</c>.</param>
    /// <param name="message">The message to include in the exception if the condition is <c>true</c>. This provides context about the failure.</param>
    /// <returns>Returns the original <paramref name="argument" /> if the condition is <c>false</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the <paramref name="argument" /> evaluates to <c>true</c>.</exception>
    public static bool RequiredFalse(this bool argument, string message)
    {
        if (argument)
        {
            throw new InvalidOperationException(message);
        }

        return argument;
    }

#if NETSTANDARD2_0
    /// <summary>
    ///     Ensures that the given boolean condition evaluates to <c>true</c>. Throws an <see cref="InvalidOperationException" /> if the condition is <c>false</c>.
    ///     This method is typically used as a guard clause to validate method preconditions or other conditions.
    /// </summary>
    /// <param name="argument">The boolean condition to evaluate. Must be <c>true</c>.</param>
    /// <param name="message">The message to include in the exception if the condition is <c>false</c>. This provides context about the failure.</param>
    /// <returns>Returns the original <paramref name="argument" /> if the condition is <c>true</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the <paramref name="argument" /> evaluates to <c>false</c>.</exception>
    [AssertionMethod]
    [SuppressMessage("ReSharper", "FlagArgument", Justification = "False positive")]
    public static bool RequiredTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool argument, string message)
    {
        if (!argument)
        {
            throw new InvalidOperationException(message);
        }

        return argument;
    }

    /// <summary>
    ///     Ensures that a nullable value type argument is not null.
    /// </summary>
    /// <remarks>
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the parameter name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <typeparam name="T">The underlying value type of the nullable argument.</typeparam>
    /// <param name="argument">The nullable value type argument to check.</param>
    /// <param name="variableName">The name of the parameter (automatically captured from the caller).</param>
    /// <returns>The non-null value of the argument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
    [AssertionMethod]
    public static T NotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this T? argument, string variableName) where T : struct
    {
        if (!argument.HasValue)
        {
            throw new ArgumentNullException(variableName);
        }

        return argument.Value;
    }

    /// <summary>
    ///     Ensures that a reference type argument is not null.
    /// </summary>
    /// <remarks>
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the parameter name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <typeparam name="T">The reference type of the argument.</typeparam>
    /// <param name="argument">The reference type argument to check.</param>
    /// <param name="variableName">The name of the variable (automatically captured from the caller).</param>
    /// <returns>The non-null argument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static T NotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this T? argument, string variableName)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(variableName);
        }

        return argument;
    }

    /// <summary>
    ///     Ensures that a nullable value type argument is not null, throwing an <see cref="InvalidOperationException" /> if it is.
    /// </summary>
    /// <remarks>
    ///     On the <c>netstandard2.0</c> target the <paramref name="memberName" /> must be supplied explicitly: unlike the
    ///     <c>net7.0+</c> build, this target cannot auto-capture it via <c>CallerArgumentExpression</c>. The parameter order
    ///     <c>(messageFormat, memberName)</c> is identical across all targets so positional call sites behave the same way.
    /// </remarks>
    /// <typeparam name="T">The underlying value type of the nullable argument.</typeparam>
    /// <param name="argument">The nullable value type argument to check.</param>
    /// <param name="messageFormat">
    ///     An optional composite format string for the exception message. <c>{0}</c> is substituted with
    ///     <paramref name="memberName" />. When <see langword="null" /> a default message is used.
    /// </param>
    /// <param name="memberName">The name of the argument used to format the exception message.</param>
    /// <returns>The non-null value of the argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static T RequiredNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this T? argument,
                                       string? messageFormat = null,
                                       string? memberName = null) where T : struct
    {
        if (!argument.HasValue)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, messageFormat ?? CannotBeNullMessageFormat, memberName));
        }

        return argument.Value;
    }

    /// <summary>
    ///     Ensures that a reference type argument is not null, throwing an <see cref="InvalidOperationException" />
    ///     if the argument is null.
    /// </summary>
    /// <remarks>
    ///     On the <c>netstandard2.0</c> target the <paramref name="memberName" /> must be supplied explicitly: unlike the
    ///     <c>net7.0+</c> build, this target cannot auto-capture it via <c>CallerArgumentExpression</c>. The parameter order
    ///     <c>(messageFormat, memberName)</c> is identical across all targets so positional call sites behave the same way.
    /// </remarks>
    /// <typeparam name="T">The reference type of the argument.</typeparam>
    /// <param name="argument">The reference type argument to check.</param>
    /// <param name="messageFormat">
    ///     An optional composite format string for the exception message. <c>{0}</c> is substituted with
    ///     <paramref name="memberName" />. When <see langword="null" /> a default message is used.
    /// </param>
    /// <param name="memberName">The name of the variable used to format the exception message.</param>
    /// <returns>The non-null argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static T RequiredNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this T? argument,
                                       string? messageFormat = null,
                                       string? memberName = null) where T : class
    {
        if (argument != null)
        {
            return argument;
        }

        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, messageFormat ?? CannotBeNullMessageFormat, memberName));
    }

    /// <summary>
    ///     Ensures that a string argument is neither null nor empty.
    /// </summary>
    /// <remarks>
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the parameter name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <param name="argument">The string argument to check.</param>
    /// <param name="parameterName">The name of the parameter (automatically captured from the caller).</param>
    /// <returns>The non-null argument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the argument is an empty string.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static string NotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this string? argument, string parameterName)
    {
        if (argument == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (string.IsNullOrEmpty(argument))
        {
            // Message text aligned with the BCL's ArgumentException.ThrowIfNullOrEmpty used by the net7.0+ partial (issue #211).
            throw new ArgumentException("The value cannot be an empty string.", parameterName);
        }

        return argument;
    }

    /// <summary>
    ///     Ensures that an enumerable argument is neither null nor empty.
    /// </summary>
    /// <typeparam name="TEnumerable">The type of the enumerable.</typeparam>
    /// <param name="argument">The enumerable argument to check.</param>
    /// <param name="parameterName">The name of the parameter (automatically captured from the caller).</param>
    /// <returns>The non-null argument.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the argument is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the argument is an empty enumerable.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static TEnumerable NotNullOrEmpty<TEnumerable>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this TEnumerable? argument,
                                                          string parameterName) where TEnumerable : class, IEnumerable
    {
        if (argument == null)
        {
            throw new ArgumentNullException(parameterName);
        }

        if (argument is ICollection collection && collection.Count == 0)
        {
            throw new ArgumentException("Argument cannot be null or empty.", parameterName);
        }

        var enumerator = argument.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            (enumerator as IDisposable)?.Dispose();

            throw new ArgumentException("Argument cannot be null or empty.", parameterName);
        }

        (enumerator as IDisposable)?.Dispose();

        return argument;
    }

    /// <summary>
    ///     Ensures that a string argument is not null or empty, throwing an exception if the condition is not met.
    /// </summary>
    /// <remarks>
    ///     On the <c>netstandard2.0</c> target the <paramref name="memberName" /> must be supplied explicitly: unlike the
    ///     <c>net7.0+</c> build, this target cannot auto-capture it via <c>CallerArgumentExpression</c>. The parameter order
    ///     <c>(messageFormat, memberName)</c> is identical across all targets so positional call sites behave the same way.
    /// </remarks>
    /// <param name="argument">The string argument to validate.</param>
    /// <param name="messageFormat">
    ///     An optional composite format string for the exception message. <c>{0}</c> is substituted with
    ///     <paramref name="memberName" />. When <see langword="null" /> a default message is used.
    /// </param>
    /// <param name="memberName">The name of the argument used to format the exception message.</param>
    /// <returns>The validated string argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null or empty.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static string RequiredNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this string? argument,
                                                string? messageFormat = null,
                                                string? memberName = null)
    {
        var validated = argument.RequiredNotNull(messageFormat, memberName);

        if (!string.IsNullOrEmpty(validated))
        {
            return validated;
        }

        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, messageFormat ?? CannotBeEmptyMessageFormat, memberName));
    }

    /// <summary>
    ///     Ensures that the provided enum value is defined within its enum type.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    /// <param name="argument">The enum value to check.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <returns>The original enum value if it is defined within the enum type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="argument" /> is not defined in the enum <typeparamref name="TEnum" />.</exception>
    [AssertionMethod]
    public static TEnum NotOutOfRange<TEnum>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this TEnum argument, string argumentName)
        where TEnum : struct, Enum
    {
        if (Enum.IsDefined(typeof(TEnum), argument) || (IsFlagsEnum<TEnum>() && HasOnlyDefinedFlagValues(argument)))
        {
            return argument;
        }

        throw new ArgumentOutOfRangeException(argumentName,
                                              argument,
                                              string.Format(CultureInfo.InvariantCulture, EnumNotDefinedMessageFormat, argument, typeof(TEnum).Name));
    }

    /// <summary>
    ///     Ensures that the given value is positive (greater than the default value for its type).
    /// </summary>
    /// <typeparam name="TValue">The type of the value, which must be a struct and implement IComparable&lt;TValue&gt;.</typeparam>
    /// <param name="argument">The value to check.</param>
    /// <param name="argumentName">The name of the argument being checked.</param>
    /// <returns>The original value if it is positive.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the <paramref name="argument" /> is not positive (i.e., less than or equal to the default value for
    ///     its type).
    /// </exception>
    [AssertionMethod]
    public static TValue Positive<TValue>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this TValue argument, string argumentName)
        where TValue : struct, IComparable<TValue>
    {
        if (argument.CompareTo(default) <= 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, argument, $"Value {argument} is not positive.");
        }

        return argument;
    }

    /// <summary>
    ///     Ensures that the argument type is assignable to <paramref name="targetType" />.
    /// </summary>
    /// <remarks>
    ///     The check is reflexive: a type is assignable to itself, so passing <paramref name="targetType" />
    ///     as the argument succeeds. This mirrors <see cref="Type.IsAssignableFrom" /> and differs from
    ///     <c>Ploch.Common.Reflection.TypeExtensions.IsImplementing</c>, which returns <see langword="false" />
    ///     for the same type.
    /// </remarks>
    /// <param name="argument">The type to check. Must not be <see langword="null" />.</param>
    /// <param name="targetType">The type the argument must be assignable to.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>The validated, non-null <paramref name="argument" />.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="argument" /> or <paramref name="targetType" /> is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="argument" /> is not assignable to <paramref name="targetType" />.
    /// </exception>
    /// <example>
    ///     <code>
    /// serviceType.AssignableTo(typeof(IMyService), nameof(serviceType));
    /// </code>
    /// </example>
    [AssertionMethod]
    [method: NotNull]
    public static Type AssignableTo([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this Type? argument, Type targetType, string parameterName)
    {
        targetType.NotNull(nameof(targetType));
        var validatedArgument = argument.NotNull(parameterName);

        AssignableToOrNull(validatedArgument, targetType, parameterName);

        return validatedArgument;
    }

    /// <summary>
    ///     Ensures that the argument type is assignable to <typeparamref name="TTarget" />.
    /// </summary>
    /// <typeparam name="TTarget">The type the argument must be assignable to.</typeparam>
    /// <param name="argument">The type to check. Must not be <see langword="null" />.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>The validated, non-null <paramref name="argument" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="argument" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="argument" /> is not assignable to <typeparamref name="TTarget" />.
    /// </exception>
    /// <example>
    ///     <code>
    /// serviceType.AssignableTo&lt;IMyService&gt;(nameof(serviceType));
    /// </code>
    /// </example>
    [AssertionMethod]
    [method: NotNull]
    public static Type AssignableTo<TTarget>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this Type? argument, string parameterName) =>
        AssignableTo(argument, typeof(TTarget), parameterName);

    /// <summary>
    ///     Ensures that the argument type is assignable to <paramref name="targetType" />, accepting
    ///     <see langword="null" />.
    /// </summary>
    /// <remarks>
    ///     Unlike <see cref="AssignableTo(Type, Type, string)" />, a <see langword="null" /> argument passes:
    ///     the assignability check applies only when a value is present. Use this when the argument itself is
    ///     optional but must be of a compatible type when supplied.
    /// </remarks>
    /// <param name="argument">The type to check, or <see langword="null" />.</param>
    /// <param name="targetType">The type the argument must be assignable to when it is not null.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>The <paramref name="argument" />, which may be <see langword="null" />.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetType" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="argument" /> is not null and is not assignable to <paramref name="targetType" />.
    /// </exception>
    public static Type? AssignableToOrNull(this Type? argument, Type targetType, string parameterName)
    {
        targetType.NotNull(nameof(targetType));

        if (argument is not null && !targetType.IsAssignableFrom(argument))
        {
            throw new ArgumentException(NotAssignableMessage(parameterName, argument, targetType), parameterName);
        }

        return argument;
    }

    /// <summary>
    ///     Ensures that the argument type is assignable to <typeparamref name="TTarget" />, accepting
    ///     <see langword="null" />.
    /// </summary>
    /// <typeparam name="TTarget">The type the argument must be assignable to when it is not null.</typeparam>
    /// <param name="argument">The type to check, or <see langword="null" />.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>The <paramref name="argument" />, which may be <see langword="null" />.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="argument" /> is not null and is not assignable to <typeparamref name="TTarget" />.
    /// </exception>
    public static Type? AssignableToOrNull<TTarget>(this Type? argument, string parameterName) => AssignableToOrNull(argument, typeof(TTarget), parameterName);

#endif

    private static bool IsFlagsEnum<TEnum>() where TEnum : struct, Enum
    {
        return typeof(TEnum).IsDefined(typeof(FlagsAttribute), false);
    }

    private static bool HasOnlyDefinedFlagValues<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        var ulongValue = ToUInt64(value);
        var mask = GetEnumValuesMask<TEnum>();

        return (ulongValue & ~mask) == 0;
    }

    private static ulong GetEnumValuesMask<TEnum>() where TEnum : struct, Enum
    {
        ulong mask = 0;

        foreach (var enumValue in GetEnumValues<TEnum>())
        {
            mask |= ToUInt64(enumValue);
        }

        return mask;
    }

    [SuppressMessage("Performance",
                     "CA2263:Prefer generic overload when type is known",
                     Justification = "The generic Enum.GetValues<T>() overload is unavailable on netstandard2.0; this polyfill centralises the target-framework conditional.")]
    private static TEnum[] GetEnumValues<TEnum>() where TEnum : struct, Enum
    {
#if NET5_0_OR_GREATER
        return Enum.GetValues<TEnum>();
#else
        return (TEnum[])Enum.GetValues(typeof(TEnum));
#endif
    }

    private static ulong ToUInt64<TEnum>(TEnum value) where TEnum : struct, Enum
    {
        return Type.GetTypeCode(Enum.GetUnderlyingType(typeof(TEnum))) switch
        {
            TypeCode.SByte => unchecked((ulong)(sbyte)(object)value),
            TypeCode.Byte => (byte)(object)value,
            TypeCode.Int16 => unchecked((ulong)(short)(object)value),
            TypeCode.UInt16 => (ushort)(object)value,
            TypeCode.Int32 => unchecked((ulong)(int)(object)value),
            TypeCode.UInt32 => (uint)(object)value,
            TypeCode.Int64 => unchecked((ulong)(long)(object)value),
            TypeCode.UInt64 => (ulong)(object)value,
            _ => throw new InvalidOperationException("Unexpected enum underlying type.")
        };
    }
}
