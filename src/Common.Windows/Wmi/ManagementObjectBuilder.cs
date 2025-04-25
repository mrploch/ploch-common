using System.Reflection;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi;

public static class ManagementObjectBuilder
{
    private static readonly IList<IManagementObjectTypeConverter> _mappers = CreateMappers();

    private static IList<IManagementObjectTypeConverter> CreateMappers()
    {
        var mappers = new List<IManagementObjectTypeConverter> { new DateTimeConverter(), new EnumConverter(), new DefaultManagementObjectTypeConverter() };

        return mappers.OrderBy(m => m.Order).ToList();
    }

    public static TManagementObject BuildObject<TManagementObject>(IWmiObject wmiObject)
        where TManagementObject : new()
    {
        var wmiObjectPropertyNames = wmiObject.GetPropertyNames().ToHashSet();
        var resultObject = new TManagementObject();
        var resultObjectType = typeof(TManagementObject);
        var resultObjectProperties = resultObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyMap = new Dictionary<string, (PropertyInfo, WindowsManagementObjectPropertyAttribute?)>();
        foreach (var property in resultObjectProperties)
        {
            var attribute = property.GetCustomAttribute<WindowsManagementObjectPropertyAttribute>(true);
            propertyMap.Add(attribute?.PropertyName ?? property.Name, (property, attribute));
        }

        foreach (var wmiObjectPropertyName in wmiObjectPropertyNames)
        {
            if (!propertyMap.TryGetValue(wmiObjectPropertyName, out var propertyMapValue))
            {
                continue;
            }

            var (resultObjectProperty, propertyAttribute) = propertyMapValue;

            var wmiObjectPropertyValue = wmiObject[wmiObjectPropertyName];
            var convertedValue = ConvertWmiValue(wmiObjectPropertyValue, resultObjectProperty.PropertyType);

            resultObjectProperty.SetValue(resultObject, convertedValue, null);
        }

        return resultObject;
    }

    private static object? ConvertWmiValue(object? value, Type targetType)
    {
        foreach (var mapper in _mappers.OrderBy(m => m.Order))
        {
            if (mapper.CanHandle(value, targetType))
            {
                return mapper.MapValue(value, targetType);
            }
        }

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
