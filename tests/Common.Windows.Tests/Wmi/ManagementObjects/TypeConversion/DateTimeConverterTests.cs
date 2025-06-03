using System.Management;
using FluentAssertions;
using Ploch.Common.Reflection;
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

    [Theory]
    [InlineData(nameof(TypeWithDateProperties.DateTimeProperty), typeof(DateTime))]
    [InlineData(nameof(TypeWithDateProperties.NullableDateTimeProperty), typeof(DateTime?))]
    public void MapValue_should_allow_DateTime_property_to_be_set_using_reflection_using_mapped_value(string propertyName, Type targetType)
    {
        var now = DateTime.UtcNow;

        var convertedValue = _converter.MapValue(ManagementDateTimeConverter.ToDmtfDateTime(now), targetType);

        var typeWithDateProperties = new TypeWithDateProperties();
        typeWithDateProperties.SetPropertyValue(propertyName, convertedValue);
        // SetTestTypeProperty(typeWithDateProperties, propertyName, convertedValue);

        typeWithDateProperties.GetPropertyValue(propertyName).As<DateTime>().Should().BeCloseTo(now, TimeSpan.FromMilliseconds(1));
    }

    [Theory]
    [InlineData(nameof(TypeWithDateProperties.DateTimeOffsetProperty), typeof(DateTimeOffset))]
    [InlineData(nameof(TypeWithDateProperties.NullableDateTimeOffsetProperty), typeof(DateTimeOffset?))]
    public void MapValue_should_allow_DateTimeOffset_property_to_be_set_using_reflection_using_mapped_value(string propertyName, Type targetType)
    {
        var now = DateTime.UtcNow;

        var convertedValue = _converter.MapValue(ManagementDateTimeConverter.ToDmtfDateTime(now), targetType);

        var typeWithDateProperties = new TypeWithDateProperties();
        SetTestTypeProperty(typeWithDateProperties, propertyName, convertedValue);

        typeWithDateProperties.GetPropertyValue(propertyName).As<DateTimeOffset>().Should().BeCloseTo(now, TimeSpan.FromMilliseconds(1));
    }

    // [Fact]
    // public void MapValue_should_allow_DateTimeOffset_property_to_be_set_using_reflection_using_mapped_value()
    // {
    //     var now = DateTime.UtcNow;
    //
    //     var convertedValue = _converter.MapValue(ManagementDateTimeConverter.ToDmtfDateTime(now), typeof(DateTimeOffset));
    //
    //     var typeWithDateProperties = new TypeWithDateProperties();
    //     SetTestTypeProperty(typeWithDateProperties, nameof(TypeWithDateProperties.DateTimeProperty), convertedValue);
    //
    //     typeWithDateProperties.DateTimeProperty.Should().BeCloseTo(now, TimeSpan.FromMilliseconds(1));
    // }

    private static void SetTestTypeProperty(TypeWithDateProperties obj, string propertyName, object? value)
    {
        var dateTimeProperty = obj.GetType().GetProperty(propertyName);
        dateTimeProperty?.SetValue(obj, value);
    }

    private class TypeWithDateProperties
    {
        public DateTime DateTimeProperty { get; set; }

        public DateTime? NullableDateTimeProperty { get; set; }

        public DateTimeOffset DateTimeOffsetProperty { get; set; }

        public DateTimeOffset? NullableDateTimeOffsetProperty { get; set; }
    }
}
