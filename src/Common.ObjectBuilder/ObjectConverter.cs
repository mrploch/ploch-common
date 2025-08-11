using System.ComponentModel;
using System.Reflection;
using Ploch.Common.ObjectBuilder.TypeConverters;
using Ploch.Common.TypeConversion;
using EnumConverter = Ploch.Common.TypeConversion.EnumConverter;
using TypeConverter = System.ComponentModel.TypeConverter;

namespace Ploch.Common.ObjectBuilder;

public class DotNetTypeConverterWrapper(TypeConverter typeConverter) : ITypeConverter
{
    public int Order { get; } = int.MaxValue;

    public bool CanHandle(object? value, Type targetType) => typeConverter.CanConvertTo(targetType);

    public bool CanHandleSourceType(Type sourceType) => typeConverter.CanConvertFrom(sourceType);

    public bool CanHandleTargetType(Type targetType) => typeConverter.CanConvertTo(targetType);

    public bool CanHandle(Type sourceType, Type targetType) => typeConverter.CanConvertTo(targetType) && typeConverter.CanConvertFrom(sourceType);

    public object? ConvertValue(object? value, Type targetType) => typeConverter.ConvertTo(value, targetType);
}

public class DefaultConverter(IEnumerable<ITypeConverter> converters) : ITypeConverter
{
    public int Order { get; } = int.MaxValue;

    public IEnumerable<Type> SupportedSourceTypes { get; } = [];

    public IEnumerable<Type> SupportedTargetTypes { get; } = [];

    public bool CanHandle(object? value, Type targetType) => true;

    public bool CanHandleSourceType(Type sourceType) => true;

    public bool CanHandleTargetType(Type targetType) => true;

    public bool CanHandle(Type sourceType, Type targetType)
    {
        var matchingConverters = converters.Where(c => c.CanHandle(sourceType, targetType));

        foreach (var matchingConverter in matchingConverters)
        {
            if (matchingConverter.CanHandle(sourceType, targetType))
            {
                return true;
            }
        }

        var sourceTypeConverter = TypeDescriptor.GetConverter(targetType);

        return sourceTypeConverter.CanConvertTo(targetType) && sourceTypeConverter.CanConvertFrom(sourceType);
    }

    public object? ConvertValue(object? value, Type targetType) => throw new NotImplementedException();
}

public static class ObjectConverter
{
    private static readonly IList<ITypeConverter> _mappers = CreateMappers();

    public static TManagementObject BuildObject<TManagementObject>(ISourceObject sourceObject) where TManagementObject : new()
    {
        var wmiObjectPropertyNames = sourceObject.GetPropertyNames().ToHashSet();
        var resultObject = new TManagementObject();
        var resultObjectType = typeof(TManagementObject);
        var resultObjectProperties = resultObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyMap = new Dictionary<string, (PropertyInfo, ObjectPropertyAttribute?)>();
        foreach (var property in resultObjectProperties)
        {
            var attribute = property.GetCustomAttribute<ObjectPropertyAttribute>(true);
            propertyMap.Add(attribute?.PropertyName ?? property.Name, (property, attribute));
        }

        foreach (var wmiObjectPropertyName in wmiObjectPropertyNames)
        {
            try
            {
                if (!propertyMap.TryGetValue(wmiObjectPropertyName, out var propertyMapValue))
                {
                    continue;
                }

                var (resultObjectProperty, propertyAttribute) = propertyMapValue;

                var wmiObjectPropertyValue = sourceObject.GetPropertyValue(wmiObjectPropertyName);
                var convertedValue = ConvertWmiValue(wmiObjectPropertyValue, resultObjectProperty.PropertyType);

                resultObjectProperty.SetValue(resultObject, convertedValue, null);
            }
            catch (TypeConversionException ex)
            {
                Console.WriteLine(ex);

                throw;
            }
        }

        return resultObject;
    }

    private static IList<ITypeConverter> CreateMappers()
    {
        var mappers =
            new List<ITypeConverter>
            {
                new ManagementObjectDateTimeOffsetTypeConverter(), new ManagementObjectDateTimeTypeConverter(), new EnumConverter(), new DefaultConverter([])
            };

        return mappers.OrderBy(m => m.Order).ToList();
    }

    private static object? ConvertWmiValue(object? value, Type targetType)
    {
        foreach (var mapper in _mappers.OrderBy(m => m.Order))
        {
            if (mapper.CanHandle(value, targetType))
            {
                return mapper.ConvertValue(value, targetType);
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
