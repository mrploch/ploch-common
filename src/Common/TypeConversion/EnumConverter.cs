using System;

// TODO: Implement types in the Ploch.Common.TypeConversion namespace
// https://github.com/mrploch/ploch-common/issues/158
namespace Ploch.Common.TypeConversion;

public class EnumConverter() : SingleSourceTargetTypeConverter<string, ValueType?>(true, true, true)
{
    public override bool CanHandleTargetType(Type targetType) => base.CanHandleTargetType(targetType) && GetUnderlyingNullableType(targetType).IsEnum;

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;

    protected override ValueType? DoConvert(string? value, Type targetType)
    {
        var enumType = GetUnderlyingNullableType(targetType);

        var fieldMap = EnumerationFieldValueCache.GetFieldsMapping(enumType);

        var stringValue = value ?? string.Empty;

        foreach (var keyValuePair in fieldMap)
        {
            if (keyValuePair.Key == stringValue)
            {
                return (Enum?)keyValuePair.Value;
            }
        }

        if (fieldMap.TryGetValue(stringValue, out var enumValue))
        {
            return (Enum?)enumValue;
        }

        return null;
    }
}
