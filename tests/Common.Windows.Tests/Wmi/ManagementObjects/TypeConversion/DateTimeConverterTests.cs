using System.Management;
using FluentAssertions;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TypeConversion;

public class DateTimeConverterTests
{
    private readonly DateTimeConverter _converter;

    public DateTimeConverterTests() => _converter = new DateTimeConverter();

    [Fact]
    public void Order_ShouldReturnMaxValue()
    {
        // Act
        var order = _converter.Order;
        // Assert
        order.Should().Be(DateTimeConverter.MapperOrder);
    }

    [Theory]
    [InlineData("abc", typeof(DateTime))]
    [InlineData("abc", typeof(DateTime?))]
    [InlineData(null, typeof(DateTime?))]

    //  [InlineData(null, typeof(DateTimeOffset?))]
    public void CanHandle_ShouldAlwaysReturnTrue(object value, Type targetType)
    {
        // Act
        var canHandle = _converter.CanHandle(value, targetType);
        // Assert
        canHandle.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, typeof(DateTime))]
    [InlineData(null, typeof(DateTimeOffset))]
    public void CanHandle_should_return_false_if_value_is_null_and_target_type_is_not_nullable(object value, Type targetType)
    {
        // Act
        var canHandle = _converter.CanHandle(value, targetType);

        // Assert
        canHandle.Should().BeFalse();
    }


    [Fact]
    public void MapValue_should_convert_to_DateTime()
    {
        var utcNow = DateTime.UtcNow;
        // Act
        var result = _converter.MapValue(ManagementDateTimeConverter.ToDmtfDateTime(utcNow), typeof(DateTime));

        var dateTimeResult = (DateTime)result!;

        // Assert
        dateTimeResult.Should().BeCloseTo(utcNow, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void MapValue_should_convert_to_nullable_DateTime()
    {
        var utcNow = DateTime.UtcNow;
        var dmtfDateTime = ManagementDateTimeConverter.ToDmtfDateTime(utcNow);

        // Act
        _converter.CanHandle(dmtfDateTime, typeof(DateTime?)).Should().BeTrue();
        var result = _converter.MapValue(dmtfDateTime, typeof(DateTime?));

        var dateTimeResult = (DateTime)result!;

        // Assert
        dateTimeResult.Should().BeCloseTo(utcNow, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public void MapValue_should_convert_null_value_to_nullable_DateTime()
    {
        // Act
        var result = _converter.MapValue(null, typeof(DateTime?));

        var dateTimeResult = (DateTime?)result!;

        // Assert
        dateTimeResult.Should().BeNull();
    }

    [Fact]
    public void MapValue_should_convert_date_time()
    {
        var now = DateTime.Now;
        // Act
        var result = _converter.MapValue(ManagementDateTimeConverter.ToDmtfDateTime(now), typeof(DateTime));

        var dateTimeResult = (DateTime)result!;

        // Assert
        dateTimeResult.Should().BeCloseTo(now.ToUniversalTime(), TimeSpan.FromMilliseconds(1));
    }
}
