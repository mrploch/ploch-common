using Ploch.Common.Reflection;

namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public abstract class ManagementObjectTypeConverter<TSourceType> : IManagementObjectTypeConverter
{
    protected ManagementObjectTypeConverter(params IEnumerable<Type> supportedTargetTypes) => SupportedTargetTypes = new HashSet<Type>(supportedTargetTypes);

    public abstract int Order { get; }

    public bool CanHandle(object? value, Type targetType)
    {
        if (!(value is TSourceType || value is null))
        {
            return false;
        }

        if (value is null && targetType.IsValueType && !targetType.IsNullable())
        {
            return false;
        }

        if (!SupportedTargetTypes.Contains(GetUnderlyingNullableType(targetType)))
        {
            return false;
        }

        return CanHandle((TSourceType)value, targetType);
    }

    public virtual object? MapValue(object? value, Type targetType) => MapValue((TSourceType?)value, targetType);

    protected virtual ISet<Type> SupportedTargetTypes { get; }


    protected virtual bool CanHandle(TSourceType? value, Type targetType) => true;

    protected abstract object? MapValue(TSourceType? value, Type targetType);

    private static Type GetUnderlyingNullableType(Type targetType) => Nullable.GetUnderlyingType(targetType) ?? targetType;
}
