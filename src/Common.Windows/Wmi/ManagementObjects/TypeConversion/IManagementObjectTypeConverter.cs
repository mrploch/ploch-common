namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public interface IManagementObjectTypeConverter
{
    int Order { get; }

    bool CanHandle(object? value, Type targetType);

    object? MapValue(object? value, Type targetType);
}
