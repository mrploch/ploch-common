using FluentAssertions;
using Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TypeConversion;

public class EnumConverterTests
{
    private readonly EnumConverter _converter;

    public EnumConverterTests() => _converter = new EnumConverter();

    [Fact]
    public void Order_should_return_max_value()
    {
        // Act
        var order = _converter.Order;
        // Assert
        order.Should().Be(EnumConverter.MapperOrder);
    }

    [Theory]
    [InlineData("", typeof(TypeCode))]
    [InlineData(null, typeof(TypeCode?))]
    public void CanHandle_should_always_return_true(object value, Type targetType)
    {
        // Act
        var canHandle = _converter.CanHandle(value, targetType);
        // Assert
        canHandle.Should().BeTrue();
    }


    [Fact]
    public void CanHandle_should_return_false_if_null_value_being_converted_to_non_nullable_enum()
    {
        // Act
        var canHandle = _converter.CanHandle(null, typeof(TypeCode?));
        // Assert
        canHandle.Should().BeFalse();
    }

    [Fact]
    public void MapValue_should_return_null_when_value_is_null()
    {
        // Act
        var result = _converter.MapValue(null, typeof(TypeCode?));
        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null, typeof(TestEnum?), null)]
    // [InlineData(nameof(TestEnumWithMappings2.Value3MappedToNull), typeof(TestEnumWithMappings2), TestEnumWithMappings2.Value3MappedToNull)]
    [InlineData(null, typeof(TypeCode?), null)]
    [InlineData(null, typeof(TestEnumWithMappings2), TestEnumWithMappings2.Value3MappedToNull)]
    [InlineData("", typeof(TestEnumWithMappings2), TestEnumWithMappings2.Value3MappedToNull)]
    [InlineData("value1", typeof(TestEnumWithMappings2), TestEnumWithMappings2.Value1)]
    [InlineData("Value 1", typeof(TestEnumWithMappings2), TestEnumWithMappings2.Value1)]
    [InlineData("VALUE2", typeof(TestEnumWithMappings2), TestEnumWithMappings2.Value2)]
    [InlineData("value1", typeof(TestEnumWithMappings2?), TestEnumWithMappings2.Value1)]
    public void MapValue_should_correctly_map_value(object? value, Type targetType, object? expectedResult)
    {
        // Act
        var result = _converter.MapValue(value, targetType);
        // Assert
        result.Should().Be(expectedResult);
    }
}
