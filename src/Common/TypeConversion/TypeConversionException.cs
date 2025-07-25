using System;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Represents errors that occur during type conversion operations in WMI management objects.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified error message and a reference to the inner exception that
///     is the cause of this exception.
/// </remarks>
/// <param name="message">The error message that explains the reason for the exception. This parameter can be null.</param>
/// <param name="convertedValue">The value that was attempted to be converted when the exception occurred.</param>
/// <param name="targetType">The type that the conversion operation was attempting to convert to when the exception occurred.</param>
/// <param name="innerException">The exception that is the cause of the current exception. This parameter can be null.</param>
#pragma warning disable CA1032 // Implement standard exception constructors
#pragma warning disable RCS1194 // Implement exception constructors
public class TypeConversionException(string? message, object convertedValue, Type targetType, Exception? innerException) : Exception(message, innerException)
#pragma warning restore RCS1194
#pragma warning restore CA1032
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified converted value and the target type.
    /// </summary>
    /// <remarks>
    ///     Exception message is built using the provided <paramref name="convertedValue" /> and the <paramref name="targetType" />.
    /// </remarks>
    /// <param name="convertedValue">Converted value.</param>
    /// <param name="targetType">Type conversion target type.</param>
    public TypeConversionException(object convertedValue, Type targetType) : this(convertedValue, targetType, null)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified converted value, target type
    ///     and optional inner exception.
    /// </summary>
    /// <remarks>
    ///     Exception message is built using the provided <paramref name="convertedValue" /> and the <paramref name="targetType" />.
    /// </remarks>
    /// <param name="convertedValue">Converted value.</param>
    /// <param name="targetType">Type conversion target type.</param>
    /// <param name="innerException">The inner exception.</param>
    public TypeConversionException(object convertedValue, Type targetType, Exception? innerException) :
        this($"Failed to convert value '{convertedValue}' to type '{targetType}'.", convertedValue, targetType, innerException)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified error message, converted value and target type.
    /// </summary>
    /// <param name="message">The message that describes the error. This parameter can be null.</param>
    /// <param name="convertedValue">Converted value.</param>
    /// <param name="targetType">Type conversion target type.</param>
    public TypeConversionException(string? message, object convertedValue, Type targetType) : this(message, convertedValue, targetType, null)
    { }

    /// <summary>
    ///     Gets the value that was attempted to be converted when the type conversion exception occurred.
    /// </summary>
    /// <remarks>
    ///     This property holds the original value that caused the type conversion operation to fail.
    ///     It can be used to inspect the value that resulted in the exception for debugging or error logging purposes.
    /// </remarks>
    public object ConvertedValue { get; } = convertedValue;

    /// <summary>
    ///     Gets the target type the conversion operation was attempting to convert to when the exception occurred.
    /// </summary>
    /// <remarks>
    ///     This property holds the <see cref="Type" /> that the conversion process was targeting but failed to convert to.
    ///     It can be used to provide detailed context about the conversion failure, such as the intended result type.
    /// </remarks>
    public Type TargetType { get; } = targetType;
}
