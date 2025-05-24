using System.Management;
using System.Runtime.Versioning;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <inheritdoc />
public class DateTimeConverter : ManagementObjectTypeConverter<string>
{
    /// <summary>
    ///     Represents the order of the mapper within the processing chain.
    /// </summary>
    /// <remarks>
    ///     This constant is exposed to allow other converters to specify their order of execution
    ///     based on other converters.
    /// </remarks>
    public const int MapperOrder = DefaultManagementObjectTypeConverter.MapperOrder - 5;

    public DateTimeConverter() : base(typeof(DateTime), typeof(DateTimeOffset))
    { }

    public override int Order => MapperOrder;

    [SupportedOSPlatform("windows")]
    protected override object? MapValue(string? value, Type targetType)
    {
        if (value is null)
        {
            return null;
        }

        var dateTime = ManagementDateTimeConverter.ToDateTime(value).ToUniversalTime();

        var underlyingType = GetUnderlyingNullableType(targetType);

        if (underlyingType == typeof(DateTime))
        {
            return dateTime;
        }

        if (underlyingType == typeof(DateTimeOffset))
        {
            return new DateTimeOffset(dateTime);
        }

        throw new NotSupportedException($"Unsupported target type: {targetType}");
    }

    protected override bool IsTargetTypeSupported(Type targetType) => base.IsTargetTypeSupported(GetUnderlyingNullableType(targetType));

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
