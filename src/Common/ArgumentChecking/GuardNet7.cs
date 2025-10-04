// ReSharper disable RedundantUsingDirective

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Ploch.Common.ArgumentChecking;

/// <summary>
///     A static class that provides methods to perform validations and ensure arguments meet specified constraints.
///     The class consists of methods designed to handle null checks, empty checks, and range validations,
///     helping developers to enforce defensive programming practices in their code.
/// </summary>
public static partial class Guard
{
    private const string CannotBeEmptyMessageFormat = "Variable {0} cannot be empty.";
    private const string CannotBeNullMessageFormat = "Variable {0} cannot be null.";
    private const string ConditionRequiredTrueMessageFormat = "Condition {0} is required to be true in {1}, {2} at {3}";
#pragma warning disable IDE1006
    private const string EnumNotDefinedMessageFormat = "Value {0} is not defined in enum {1}.";
#pragma warning restore IDE1006

#if NET7_0_OR_GREATER
    /// <summary>
    ///     Ensures that a given boolean condition is true, throwing an <see cref="InvalidOperationException" /> if it is not.
    /// </summary>
    /// <param name="condition">The boolean condition to check.</param>
    /// <param name="messageFormat">
    ///     An optional custom error message format string that can include placeholders.
    ///     If not provided, a default error message with condition, member, and location details will be used.
    /// </param>
    /// <param name="memberName">
    ///     The name of the member calling this method, captured automatically using
    ///     <see cref="CallerMemberNameAttribute" />.
    /// </param>
    /// <param name="expression">
    ///     The actual expression being evaluated, captured automatically using
    ///     <see cref="CallerArgumentExpressionAttribute" />.
    /// </param>
    /// <param name="sourceFilePath">
    ///     The full path of the source file where this method is called, captured automatically using
    ///     <see cref="CallerFilePathAttribute" />.
    /// </param>
    /// <param name="sourceLineNumber">
    ///     The line number in the source file where this method is called, captured automatically using
    ///     <see cref="CallerLineNumberAttribute" />.
    /// </param>
    /// <returns>The original boolean condition if it evaluates to true.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the condition is false, optionally formatted with the provided message or default contextual details.
    /// </exception>
    [AssertionMethod]
    [Pure]
    [SuppressMessage("ReSharper", "TooManyArguments", Justification = "Arguments are required and are automatically captured.")]
    [SuppressMessage("ReSharper", "FlagArgument", Justification = "False positive")]
    public static bool RequiredTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition,
                                    string? messageFormat = null,
                                    [CallerMemberName] string? memberName = null,
                                    [CallerArgumentExpression(nameof(condition))]
                                    string? expression = null,
                                    [CallerFilePath] string? sourceFilePath = null,
                                    [CallerLineNumber] int sourceLineNumber = 0)
    {
        if (condition)
        {
            return condition;
        }

        var message = string.Format(CultureInfo.InvariantCulture,
                                    messageFormat ?? ConditionRequiredTrueMessageFormat,
                                    expression,
                                    memberName,
                                    sourceFilePath,
                                    sourceLineNumber);

        throw new InvalidOperationException(message);
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
    public static T NotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this T? argument,
                               [CallerArgumentExpression(nameof(argument))]
                               string? variableName = null) where T : struct
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
    [return: NotNullIfNotNull(nameof(argument))]
    [AssertionMethod]
    public static T NotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this T? argument,
                               [CallerArgumentExpression(nameof(argument))]
                               string? variableName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, variableName);

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
    /// <param name="messageFormat">The exception message format.</param>
    /// <returns>The non-null value of the argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null.</exception>
    [return: NotNullIfNotNull(nameof(argument))]
    [AssertionMethod]
    public static T RequiredNotNull<T>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this T? argument,
                                       [CallerArgumentExpression(nameof(argument))]
                                       string? memberName = null,
                                       string? messageFormat = null) where T : struct
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
    ///     This method uses <see cref="CallerArgumentExpressionAttribute" /> to automatically capture the parameter name
    ///     from the calling code, reducing the need for string literals.
    /// </remarks>
    /// <typeparam name="T">The reference type of the argument.</typeparam>
    /// <param name="argument">The reference type argument to check.</param>
    /// <param name="message">The exception message if argument is null.</param>
    /// <param name="memberName">The name of the variable (automatically captured from the caller).</param>
    /// <returns>The non-null argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null.</exception>
    [return: NotNullIfNotNull(nameof(argument))]
    [AssertionMethod]
    public static T RequiredNotNull<T>(
        [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] [JetBrains.Annotations.NotNull] this T? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))]
        string? memberName = null) where T : class
    {
        if (argument != null)
        {
            return argument;
        }

        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message ?? CannotBeNullMessageFormat, memberName));
    }

    [return: NotNullIfNotNull(nameof(argument))]
    [AssertionMethod]
    public static TValue NotNullOrDefault<TValue>(
        [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this TValue? argument,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        ArgumentNullException.ThrowIfNull(argument, argumentName);
        if (EqualityComparer<TValue>.Default.Equals(argument, default))
        {
            throw new ArgumentNullException(argumentName);
        }

        return argument;
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
    public static string NotNullOrEmpty(
        [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this string? argument,
        [CallerArgumentExpression(nameof(argument))]
        string? parameterName = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(argument, parameterName);

        return argument;
    }

    [AssertionMethod]
    public static TEnumerable NotNullOrEmpty<TEnumerable>(
        [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this TEnumerable? argument,
        [CallerArgumentExpression(nameof(argument))]
        string? parameterName = null) where TEnumerable : class, IEnumerable
    {
        argument.NotNull(parameterName);

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
    /// <param name="message">The exception message if argument is null.</param>
    /// <param name="memberName">The name of the argument (automatically captured from the caller).</param>
    /// <returns>The validated string argument.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the argument is null or empty.</exception>
    [return: NotNullIfNotNull(nameof(argument))]
    [AssertionMethod]
    public static string RequiredNotNullOrEmpty(
        [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this string? argument,
        string? message = null,
        [CallerArgumentExpression(nameof(argument))]
        string? memberName = null)
    {
        argument.RequiredNotNull(message, memberName);

        if (!string.IsNullOrEmpty(argument))
        {
            return argument;
        }

        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, message ?? CannotBeEmptyMessageFormat, memberName));
    }

    /// <summary>
    ///     Ensures that the provided enum value is a defined value within its respective enumeration type.
    ///     If the value is not defined, an <see cref="ArgumentOutOfRangeException" /> is thrown.
    /// </summary>
    /// <remarks>
    ///     This method does not handle flags - https://github.com/mrploch/ploch-common/issues/159.
    /// </remarks>
    /// <param name="argument">The enum value to validate.</param>
    /// <param name="argumentName">
    ///     The name of the argument being validated, captured automatically using
    ///     <see cref="CallerArgumentExpressionAttribute" />.
    /// </param>
    /// <typeparam name="TEnum">The type of the enumeration to check. Must be a non-nullable struct that is an enum.</typeparam>
    /// <returns>The original enum value if it is defined within the enumeration.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the provided enum value is not a defined member of the specified enumeration type.
    /// </exception>
    public static TEnum NotOutOfRange<TEnum>([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this TEnum argument,
                                             [CallerArgumentExpression(nameof(argument))]
                                             string? argumentName = null) where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(argument))
        {
            throw new ArgumentOutOfRangeException(argumentName,
                                                  argument,
                                                  string.Format(CultureInfo.InvariantCulture, EnumNotDefinedMessageFormat, argument, typeof(TEnum).Name));
        }

        return argument;
    }

    public static TValue Positive<TValue>(
        [AssertionCondition(AssertionConditionType.IS_NOT_NULL)] [System.Diagnostics.CodeAnalysis.NotNull] this TValue argument,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null) where TValue : struct, IComparable<TValue>
    {
        if (argument.CompareTo(default) <= 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, argument, $"{argumentName} must be positive.");
        }

        return argument;
    }

#endif
}
