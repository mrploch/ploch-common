using System;
using System.Collections.Generic;

namespace Ploch.Common.TypeConversion;

/// <summary>
///     Represents an abstract base class for converting between a single source type and a target type,
///     providing support for configurable nullability, handling of derived types, and extensions for
///     additional supported source and target types.
/// </summary>
/// <typeparam name="TSourceType">The source type for the conversion.</typeparam>
/// <typeparam name="TTargetType">The target type for the conversion.</typeparam>
public abstract class SingleSourceTargetTypeConverter<TSourceType, TTargetType>(bool canHandleNullSourceValue,
                                                                                bool canHandleDerivedSourceTypes,
                                                                                bool canHandleDerivedTargetTypes,
                                                                                IEnumerable<Type>? additionalSourceTypes = null,
                                                                                IEnumerable<Type>? additionalTargetTypes = null)
    : TypeConverter(canHandleNullSourceValue,
                    TypeConverterHelper.CombinedTypes<TSourceType>(additionalSourceTypes),
                    TypeConverterHelper.CombinedTypes<TTargetType>(additionalTargetTypes)),
      ITypeConverter<TSourceType, TTargetType>
{
    private readonly bool _canHandleNullSourceValue = canHandleNullSourceValue;

    /// <summary>
    ///     Converts a value of the source type to the target type.
    /// </summary>
    /// <param name="value">The source value to be converted. May be null.</param>
    /// <typeparam name="TTarget">
    ///     The specific type of the target to which the value should be converted. Must inherit from or implement
    ///     <typeparamref name="TTargetType" />.
    /// </typeparam>
    /// <returns>
    ///     The converted value of type <typeparamref name="TTarget" />, or null if the conversion could not be performed.
    /// </returns>
    public TTargetType? ConvertValue(TSourceType? value) => ConvertValue<TTargetType>(value);

    /// <summary>
    ///     Converts a value of the source type to the target type.
    /// </summary>
    /// <param name="value">
    ///     The source value of type <typeparamref name="TSourceType" /> to be converted.
    ///     Can be null if the implementation supports null source values.
    /// </param>
    /// <typeparam name="TTarget">
    ///     The specific target type to which the value will be converted.
    ///     Must inherit from or implement <typeparamref name="TTargetType" />.
    /// </typeparam>
    /// <returns>
    ///     The converted value of type <typeparamref name="TTarget" />, or null if the conversion could not be performed.
    /// </returns>
    public TTarget? ConvertValue<TTarget>(TSourceType? value) where TTarget : TTargetType => (TTarget?)ConvertValue(value, typeof(TTarget));

    /// <inheritdoc />
    public TTargetType? ConvertValue(TSourceType? value, Type targetType)
    {
        if (value is null && !_canHandleNullSourceValue)
        {
            throw new ArgumentNullException(nameof(value), "Source value cannot be null for this converter.");
        }

        if (value is not null && !CanHandleSourceType(value.GetType()))
        {
            throw new ArgumentException($"Source value type '{value.GetType()}' is not supported by this converter.", nameof(value));
        }

        return DoConvert(value, targetType);
    }

    /// <inheritdoc />
    public override bool CanHandleSourceType(Type sourceType) =>
        TypeConverterHelper.CanHandleType(canHandleDerivedSourceTypes, typeof(TSourceType), sourceType);

    /// <inheritdoc />
    public override bool CanHandleTargetType(Type targetType) =>
        TypeConverterHelper.CanHandleType(canHandleDerivedTargetTypes, typeof(TTargetType), targetType);

    /// <inheritdoc />
    public TTargetType? ConvertValueToTargetType(object? value) => (TTargetType?)ConvertValue(value, typeof(TTargetType));

    /// <inheritdoc />
    public override object? ConvertValue(object? value, Type targetType) => ConvertValue((TSourceType?)value, targetType);

    /// <summary>
    ///     Performs the conversion of the source value of type <typeparamref name="TSourceType" /> to the target type <typeparamref name="TTargetType" />.
    /// </summary>
    /// <param name="value">The source value to be converted. Can be null if <c>canHandleNullSourceValue</c> is set to true.</param>
    /// <param name="targetType">The target type to which the source value will be converted. This must be compatible with <typeparamref name="TTargetType" />.</param>
    /// <returns>
    ///     Returns the converted value of type <typeparamref name="TTargetType" /> if the conversion is successful; otherwise, returns null if the source value is
    ///     null and null handling is permitted.
    /// </returns>
    protected abstract TTargetType? DoConvert(TSourceType? value, Type targetType);
}
