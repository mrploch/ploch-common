namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <summary>
///     Represents errors that occur during type conversion operations in WMI management objects.
/// </summary>
public class TypeConversionException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified converted value and the target type.
    /// </summary>
    /// <remarks>
    ///     Exception message is built using the provided <paramref name="convertedValue" /> and the <paramref name="targetType" />.
    /// </remarks>
    public TypeConversionException(object convertedValue, Type targetType) : this(convertedValue, targetType, null)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified converted value, target type
    ///     and optional inner exception.
    /// </summary>
    /// <remarks>
    ///     Exception message is built using the provided <paramref name="convertedValue" /> and the <paramref name="targetType" />.
    /// </remarks>
    public TypeConversionException(object convertedValue, Type targetType, Exception? innerException) :
        this($"Failed to convert value '{convertedValue}' to type '{targetType}'.", convertedValue, targetType, innerException)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified error message, converted value and target type.
    /// </summary>
    /// <param name="message">The message that describes the error. This parameter can be null.</param>
    public TypeConversionException(string? message, object convertedValue, Type targetType) : this(message, convertedValue, targetType, null)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeConversionException" /> class with a specified error message and a reference to the inner exception that
    ///     is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception. This parameter can be null.</param>
    /// <param name="innerException">The exception that is the cause of the current exception. This parameter can be null.</param>
    public TypeConversionException(string? message, object convertedValue, Type targetType, Exception? innerException) : base(message, innerException)
    {
        ConvertedValue = convertedValue;
        TargetType = targetType;
    }

    public object ConvertedValue { get; }

    public Type TargetType { get; }
}
