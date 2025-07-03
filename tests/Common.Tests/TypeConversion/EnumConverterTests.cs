using FluentAssertions;
using Ploch.Common.Tests.Reflection;
using Ploch.Common.TypeConversion;
using Xunit;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TypeConversion;

public class EnumConverterTests
{
    public enum TestEnumWithMappings2
    {
        [EnumMapping("Value 1", IncludeActualEnumName = true)]
        Value1,

        [EnumMapping("Value 2", IncludeActualEnumName = true)]
        Value2,

        [EnumMapping]
        Value3MappedToNull
    }

    private readonly EnumConverter _converter = new();

    [Fact]
    public void MyMethod()
    { }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(EnumConverterTests))]
    public void CanHandle_should_return_false_if_target_type_is_not_enum(Type targetType)
    {
        var canHandle = _converter.CanHandle("test", targetType);
        var canHandleTypes = _converter.CanHandle(typeof(string), targetType);

        canHandle.Should().BeFalse();
        canHandleTypes.Should().BeFalse();
    }

    [Theory]
    [InlineData("", typeof(TypeCode))]
    [InlineData(null, typeof(TypeCode?))]
    public void CanHandle_should_return_true_for_string_value_and_enum_target_type(object value, Type targetType)
    {
        // Act
        var canHandle2 = _converter.CanHandle(value, targetType);

        // Assert
        canHandle2.Should().BeTrue();
    }

    [Theory]
    [InlineData(nameof(TypeCode.Boolean), typeof(TypeCode))]
    [InlineData(nameof(TypeCode.Boolean), typeof(TypeCode?))]
    [InlineData(null, typeof(TypeCode?))]
    [InlineData(nameof(Base64FormattingOptions.InsertLineBreaks), typeof(Base64FormattingOptions))]
    [InlineData(nameof(Base64FormattingOptions.InsertLineBreaks), typeof(Base64FormattingOptions?))]
    [InlineData(null, typeof(Base64FormattingOptions?))]
    public void CanHandl_should_return_true_for_enum_types(string? value, Type targetType)
    {
        // Act
        var canHandleValue = _converter.CanHandle(value, targetType);
        var canHandleTypes = _converter.CanHandle(typeof(string), targetType);

        // Assert
        canHandleValue.Should().BeTrue();
        canHandleTypes.Should().BeTrue();
    }

    [Fact]
    public void CanHandle_should_return_false_if_null_value_being_converted_to_non_nullable_enum()
    {
        // Act
        var canHandle2 = _converter.CanHandle((object?)null, typeof(TypeCode));

        // Assert
        canHandle2.Should().BeFalse();
    }

    [Fact]
    public void MapValue_should_return_null_when_value_is_null()
    {
        // Act
        var result2 = _converter.ConvertValue(null, typeof(TypeCode?));

        // Assert
        result2.Should().BeNull();
    }

    [Theory]
    [InlineData(null, typeof(TestTypes.TestEnum?), null)]

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
        var result2 = _converter.ConvertValue(value, targetType);

        // Assert
        result2.Should().Be(expectedResult);
    }
}
