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
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the argument name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <typeparam name="T">The underlying value type of the nullable argument.</typeparam>
    /// <param name="argument">The nullable value type argument to check.</param>
    /// <param name="memberName">The name of the argument (automatically captured from the caller).</param>
    /// <returns>The non-null value of the argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static T RequiredNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this T? argument, string memberName) where T : struct
    {
        if (!argument.HasValue)
        {
            throw new InvalidOperationException($"Variable {memberName} does not have value.");
        }

        return argument.Value;
    }

    /// <summary>
    ///     Ensures that a reference type argument is not null, throwing an <see cref="InvalidOperationException" />
    ///     if the argument is null.
    /// </summary>
    /// <remarks>
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the parameter name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <typeparam name="T">The reference type of the argument.</typeparam>
    /// <param name="argument">The reference type argument to check.</param>
    /// <param name="memberName">The name of the variable (automatically captured from the caller).</param>
    /// <param name="message">The exception message if argument is null.</param>
    /// <returns>The non-null argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static T RequiredNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this T? argument,
                                       string? memberName,
                                       string? message = null) where T : class
    {
        if (argument != null)
        {
            return argument;
        }

        if (memberName is null && message is null)
        {
            throw new InvalidOperationException("Both memberName and message arguments cannot be null at the same time.");
        }

        var format = message != null ?
            string.Format(CultureInfo.InvariantCulture, message, memberName) :
            $"Variable {memberName} is null, but was expected to not be null.";

        throw new InvalidOperationException(format);
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
            throw new ArgumentException("Argument cannot be null or empty.", parameterName);
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
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the argument name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <param name="argument">The string argument to validate.</param>
    /// <param name="memberName">The name of the argument (automatically captured from the caller).</param>
    /// <param name="message">The exception message if argument is null.</param>
    /// <returns>The validated string argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null or empty.</exception>
    [AssertionMethod]
    [method: NotNull]
    public static string RequiredNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [NotNull] this string? argument,
                                                string memberName,
                                                string? message = null)
    {
        argument.RequiredNotNull(memberName, message);

        if (string.IsNullOrEmpty(argument))
        {
            var format = message != null ? string.Format(CultureInfo.InvariantCulture, message, memberName) : $"Variable {memberName} is empty.";

            throw new InvalidOperationException(format);
        }

        return argument!;
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
        if (!Enum.IsDefined(typeof(TEnum), argument))
        {
            throw new ArgumentOutOfRangeException(argumentName,
                                                  argument,
                                                  string.Format(CultureInfo.InvariantCulture, EnumNotDefinedMessageFormat, argument, typeof(TEnum).Name));
        }

        return argument;
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

#endif
}
