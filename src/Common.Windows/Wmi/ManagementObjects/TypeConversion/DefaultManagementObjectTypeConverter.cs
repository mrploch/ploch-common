namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

public class DefaultManagementObjectTypeConverter : IManagementObjectTypeConverter
{
    public const int MapperOrder = int.MaxValue;

    public int Order => MapperOrder;

    public bool CanHandle(object? value, Type targetType) => true;

    public object? MapValue(object? value, Type targetType)
    {
        if (value is null)
        {
            return null;
        }

        if (targetType == typeof(string))
        {
            return value.ToString();
        }

        return Convert.ChangeType(value, targetType);
    }
}
