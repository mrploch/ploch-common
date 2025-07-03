using System;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Represents a generic interface for defining custom type conversion logic between different types.
///     This interface provides methods to determine the supported types for conversion,
///     whether specific conversions are possible, and to perform the actual conversion.
/// </summary>
public interface ITypeConverter
{
    int Order { get; }

    /// <summary>
    ///     Determines whether the current converter can handle the specified value and target type.
    /// </summary>
    /// <param name="value">
    ///     The value to be converted, which can be null. If the value is null, the method checks
    ///     whether the converter is configured to handle null source values.
    /// </param>
    /// <param name="targetType">
    ///     The target type to which the value needs to be converted. The target type is checked
    ///     for compatibility with the converter, including whether it is a nullable value type.
    /// </param>
    /// <returns>
    ///     True if the converter can handle the conversion from the given value to the target type;
    ///     otherwise, false.
    /// </returns>
    bool CanHandle(object? value, Type targetType);

    /// <summary>
    ///     Determines whether the specified source type is supported by the type converter.
    /// </summary>
    /// <param name="sourceType">
    ///     The source type to test. This type is checked against the converter's supported source types,
    ///     including optional handling of derived types if applicable.
    /// </param>
    /// <returns>
    ///     True if the converter can handle the specified source type, either directly or via inheritance
    ///     (if configured to do so); otherwise, false.
    /// </returns>
    bool CanHandleSourceType(Type sourceType);

    /// <summary>
    ///     Determines whether the converter is capable of handling the specified target type for conversion purposes.
    /// </summary>
    /// <param name="targetType">
    ///     The target type to be verified. This type is checked to determine if it is supported
    ///     for conversion, taking into account derived type compatibility if applicable.
    /// </param>
    /// <returns>
    ///     True if the converter can handle the specified target type for conversion; otherwise, false.
    /// </returns>
    bool CanHandleTargetType(Type targetType);

    /// <summary>
    ///     Determines whether the current converter can handle the conversion from the specified source type to the target type.
    /// </summary>
    /// <param name="sourceType">The type of the source value that needs to be converted.</param>
    /// <param name="targetType">The target type to which the value needs to be converted.</param>
    /// <returns>
    ///     True if the converter can handle the conversion from the source type to the target type; otherwise, false.
    /// </returns>
    bool CanHandle(Type sourceType, Type targetType);

    /// <summary>
    ///     Maps the specified value to the given target type using the conversion logic implemented by the current converter.
    /// </summary>
    /// <param name="value">The value to be converted, which can be null.</param>
    /// <param name="targetType">The target type to which the value needs to be converted.</param>
    /// <returns>
    ///     The converted value, or null if the conversion cannot be performed or the input value is null.
    /// </returns>
    object? ConvertValue(object? value, Type targetType);
}

/// <summary>
///     Defines a generic interface for type conversion between a specified source type and target type.
///     This interface enables converting values from the source type to the target type,
///     as well as providing the ability to specify alternative target types during conversion operations.
/// </summary>
/// <typeparam name="TSourceType">The type of the source value that will be converted.</typeparam>
/// <typeparam name="TTargetType">The type of the target value to which the source type will be converted.</typeparam>
public interface ITypeConverter<TSourceType, TTargetType> : ISourceTypeConverter<TSourceType>, ITargetTypeConverter<TTargetType>
{
    /// <summary>
    ///     Converts the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to be converted.</param>
    /// <returns>The converted value of the target type, or null if the conversion fails.</returns>
    TTargetType? ConvertValue(TSourceType? value);

    /// <summary>
    ///     Converts the specified value to the target type.
    /// </summary>
    /// <param name="value">
    ///     The value to be converted. This can be null if the implementation supports handling null values.
    /// </param>
    /// <typeparam name="TTarget">
    ///     The generic type to which the value will be converted. This type must be assignable from the target type defined
    ///     by the converter implementation.
    /// </typeparam>
    /// <returns>
    ///     The converted value of type <typeparamref name="TTarget" />, or null if the conversion cannot be performed.
    /// </returns>
    TTarget? ConvertValue<TTarget>(TSourceType? value) where TTarget : TTargetType;

    /// <summary>
    ///     Converts a value from the source type to the target type specified, considering additional type-handling rules.
    /// </summary>
    /// <param name="value">
    ///     The value of type <typeparamref name="TSourceType" /> to be converted. This value can be null,
    ///     in which case the method will handle null values according to the converter's configuration.
    /// </param>
    /// <param name="targetType">
    ///     The target type to which the value should be converted. The method ensures compatibility
    ///     between the source value and the specified target type before performing the conversion.
    /// </param>
    /// <returns>
    ///     An instance of <typeparamref name="TTargetType" />, or null if the value is null and the converter is configured to handle null values.
    ///     If the conversion is not possible, an exception is thrown.
    /// </returns>
    TTargetType? ConvertValue(TSourceType? value, Type targetType);
}
