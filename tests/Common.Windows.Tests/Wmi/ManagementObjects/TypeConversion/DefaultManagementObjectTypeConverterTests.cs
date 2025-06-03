using FluentAssertions;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TypeConversion;

public class DefaultManagementObjectTypeConverterTests
{
    private readonly DefaultManagementObjectTypeConverter _converter;

    public DefaultManagementObjectTypeConverterTests() => _converter = new DefaultManagementObjectTypeConverter();

    [Fact]
    public void Order_should_be_int_max_value()
    {
        // Act
        var order = _converter.Order;

        // Assert
        order.Should().Be(int.MaxValue);
    }

    [Theory]
    [InlineData(null, typeof(string))]
    [InlineData("test", typeof(string))]
    [InlineData(123, typeof(int))]
    [InlineData(false, typeof(string))]
    public void CanHandle_should_return_true_for_handled_types(object value, Type targetType)
    {
        // Act
        var canHandle = _converter.CanHandle(value, targetType);

        // Assert
        canHandle.Should().BeTrue();
    }

    [Fact]
    public void MapValue_should_return_null_if_value_is_null()
    {
        // Act
        var result = _converter.MapValue(null, typeof(string));

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(null, typeof(string), null)]
    [InlineData("test", typeof(string), "test")]
    [InlineData(123, typeof(int), 123)]
    [InlineData("123", typeof(int), 123)]
    [InlineData("123.34", typeof(double), 123.34)]
    [InlineData(false, typeof(string), "False")]
    [InlineData(false, typeof(bool), false)]
    public void MapValue_should_correctly_map_value(object? value, Type targetType, object? expectedResult)
    {
        // Act
        var result = _converter.MapValue(value, targetType);

        // Assert
        result.Should().Be(expectedResult);
    }

    [Fact]
    public void MapValue_should_throw_InvalidOperation_Exception_if_conversion_is_not_possible()
    {
        // Arrange
        var value = "invalid";

        // Act
        Action act = () => _converter.MapValue(value, typeof(int));

        // Asert
        act.Should().Throw<TypeConversionException>();
    }
}
