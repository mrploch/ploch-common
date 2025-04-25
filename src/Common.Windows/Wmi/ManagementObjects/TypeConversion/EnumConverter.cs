namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public class EnumConverter : ManagementObjectTypeConverter<string>
{
    public const int MapperOrder = DefaultManagementObjectTypeConverter.MapperOrder - 100;

    public override int Order { get; } = MapperOrder;

    protected override bool CanHandle(string? value, Type targetType) => GetUnderlyingNullableType(targetType).IsEnum;

    protected override object? MapValue(string? value, Type targetType)
    {
        var enumType = GetUnderlyingNullableType(targetType);

        var fieldMap = EnumFieldValueCache.GetFieldsMapping(enumType);

        var stringValue = value ?? string.Empty;

        if (fieldMap.TryGetValue(stringValue, out var enumValue))
        {
            return enumValue;
        }

        return null;
    }

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
