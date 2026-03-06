using System;
using Ploch.Common.ArgumentChecking;

// TODO: Implement types in the Ploch.Common.TypeConversion namespace
// https://github.com/mrploch/ploch-common/issues/158
namespace Ploch.Common.TypeConversion;

/// <summary>
///     Provides functionality for converting string values to nullable Enum types.
/// </summary>
/// <remarks>
///     This class is specifically designed to handle conversions between string representations of enumeration values
///     and their corresponding nullable Enum types. It extends the functionality provided by
///     <see cref="SingleSourceTargetTypeConverter{TSourceType, TTargetType}" />.
/// </remarks>
public class EnumConverter() : SingleSourceTargetTypeConverter<string, ValueType?>(true, true, true)
{
    /// <summary>
    ///     Determines whether the target type can be handled.
    ///     This method checks if the base implementation can handle the target type
    ///     and additionally verifies whether the type or its underlying nullable type is an enum.
    /// </summary>
    /// <param name="targetType">The target type to be checked.</param>
    /// <returns>Returns true if the target type can be handled and is an enumeration type; otherwise, false.</returns>
    public override bool CanHandleTargetType(Type targetType) =>
        base.CanHandleTargetType(targetType) && GetUnderlyingNullableType(targetType.NotNull(nameof(targetType))).IsEnum;

    /// <summary>
    ///     Converts a string value to a nullable enumeration type based on the specified target type.
    ///     This method attempts to map the string value to an enumeration field within the specified target type.
    /// </summary>
    /// <param name="value">The string value to be converted. Can be null.</param>
    /// <param name="targetType">The target type to which the value should be converted. Must be a nullable enumeration type.</param>
    /// <returns>Returns a nullable enumeration value if the conversion is successful; otherwise, null.</returns>
    protected override ValueType? DoConvert(string? value, Type targetType)
    {
        var enumType = GetUnderlyingNullableType(targetType.NotNull(nameof(targetType)));

        var fieldMap = EnumerationFieldValueCache.GetFieldsMapping(enumType);

        var stringValue = value ?? string.Empty;

        foreach (var keyValuePair in fieldMap)
        {
            if (keyValuePair.Key == stringValue)
            {
                return (Enum?)keyValuePair.Value;
            }
        }

        return null;
    }

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
