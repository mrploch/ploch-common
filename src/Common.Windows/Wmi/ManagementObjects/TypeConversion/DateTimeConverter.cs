using System.Management;
using System.Runtime.Versioning;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public class DateTimeConverter : ManagementObjectTypeConverter<string>
{
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

        return ManagementDateTimeConverter.ToDateTime(value).ToUniversalTime();
    }
    //
    // protected override bool CanHandle(string? value, Type targetType)
    // {
    //     if (value is null)
    //     {
    //         return targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>);
    //     }
    //
    //     return true;
    // }

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
