using System;
using System.Collections.Generic;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Reflection;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Base abstract class for type converters that can convert values between different types.
/// </summary>
/// <param name="canHandleNullSourceValue">Indicates whether the converter can handle null source values.</param>
/// <param name="supportedSourceTypes">Collection of source types that this converter can handle.</param>
/// <param name="supportedTargetTypes">Collection of target types that this converter can convert to.</param>
public abstract class TypeConverter(bool canHandleNullSourceValue, IEnumerable<Type> supportedSourceTypes, IEnumerable<Type> supportedTargetTypes)
    : ITypeConverter
{
    /// <summary>
    ///     Gets the collection of source types that this converter can handle.
    /// </summary>
    public IEnumerable<Type> SupportedSourceTypes { get; } = supportedSourceTypes;

    /// <summary>
    ///     Gets the collection of target types that this converter can convert to.
    /// </summary>
    public IEnumerable<Type> SupportedTargetTypes { get; } = supportedTargetTypes;

    /// <summary>
    ///     Gets the processing order for this type converter.
    ///     Determines the sequence in which the converter should be executed when multiple converters are available.
    /// </summary>
    public int Order { get; }

    /// <summary>
    ///     Determines whether this converter can handle the conversion of the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to convert, which may be null.</param>
    /// <param name="targetType">The type to convert the value to.</param>
    /// <returns>
    ///     <c>true</c> if this converter can handle the conversion; otherwise, <c>false</c>.
    ///     For null values, returns <c>true</c> only if the converter is configured to handle null values
    ///     and the target type is either a reference type or a nullable value type.
    /// </returns>
    public bool CanHandle(object? value, Type targetType)
    {
        targetType.NotNull(nameof(targetType));

        if (value is not null)
        {
            return CanHandle(value.GetType(), targetType);
        }

        if (!canHandleNullSourceValue)
        {
            return false;
        }

        return !targetType.IsValueType || targetType.IsNullable();
    }

    /// <summary>
    ///     Determines whether this converter can handle the specified source type.
    /// </summary>
    /// <param name="sourceType">The source type to check.</param>
    /// <returns><c>true</c> if this converter can handle the source type; otherwise, <c>false</c>.</returns>
    public abstract bool CanHandleSourceType(Type sourceType);

    /// <summary>
    ///     Determines whether this converter can handle the specified target type.
    /// </summary>
    /// <param name="targetType">The target type to check.</param>
    /// <returns><c>true</c> if this converter can handle the target type; otherwise, <c>false</c>.</returns>
    public abstract bool CanHandleTargetType(Type targetType);

    /// <summary>
    ///     Determines whether this converter can handle the conversion from the specified source type to the target type.
    /// </summary>
    /// <param name="sourceType">The source type to convert from.</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <returns><c>true</c> if this converter can handle both the source and target types; otherwise, <c>false</c>.</returns>
    public bool CanHandle(Type sourceType, Type targetType) => CanHandleSourceType(sourceType) && CanHandleTargetType(targetType);

    /// <summary>
    ///     Converts the specified value to the target type.
    /// </summary>
    /// <param name="value">The value to convert, which may be null.</param>
    /// <param name="targetType">The type to convert the value to.</param>
    /// <returns>The converted value, or null if the conversion is not possible or the input value is null.</returns>
    public abstract object? ConvertValue(object? value, Type targetType);
}
