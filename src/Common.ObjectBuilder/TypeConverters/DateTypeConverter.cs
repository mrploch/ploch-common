using System.Management;
using Ploch.Common.TypeConversion;

namespace Ploch.Common.ObjectBuilder.TypeConverters;

public class ManagementObjectDateTimeOffsetTypeConverter : ManagementObjectDateTypeConverter<DateTimeOffset>
{
    protected override DateTimeOffset? ConvertToTargetType(DateTime dateTime, Type underlyingType) => throw new NotImplementedException();
}

/// <inheritdoc />
public class ManagementObjectDateTimeTypeConverter : ManagementObjectDateTypeConverter<DateTime>
{
    protected override DateTime? ConvertToTargetType(DateTime dateTime, Type underlyingType) => dateTime;
}

public abstract class ManagementObjectDateTypeConverter<T> : NullabeTypeConverter<string, T?> where T : struct
{
    protected override T? DoConvert(string? value, Type targetType)
    {
        if (value is null)
        {
            return null;
        }

        var dateTime = ManagementDateTimeConverter.ToDateTime(value).ToUniversalTime();

        var underlyingType = GetUnderlyingNullableType(targetType);
        if (underlyingType == typeof(T))
        {
            return ConvertToTargetType(dateTime, underlyingType);
        }

        throw new NotSupportedException($"Unsupported target type: {targetType}");
    }

    protected abstract T? ConvertToTargetType(DateTime dateTime, Type underlyingType);
}

public abstract class NullabeTypeConverter<TSource, TTarget>() : SingleSourceTargetTypeConverter<TSource?, TTarget?>(true, true, false)
{
    protected static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
