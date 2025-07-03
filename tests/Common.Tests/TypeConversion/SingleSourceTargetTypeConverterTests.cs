using FluentAssertions;
using Ploch.Common.TypeConversion;
using Xunit;

namespace Ploch.Common.Tests.TypeConversion;

public class SingleSourceTargetTypeConverterTests
{
    [Fact]
    public void CanHandle_for_derived_source_and_derived_target_types_returns_true()
    {
        var converter = new TypeConversionTestType1ConverterToTypeConversionTestType2IncludingDerived();

        var canHandle = converter.CanHandle(typeof(TypeConversionTestType1Derived), typeof(TypeConversionTestType2Derived));

        canHandle.Should().BeTrue();
    }

    [Fact]
    public void CanHandle_for_actual_source_and_target_types_returns_true()
    {
        var converter = new TypeConversionTestType1ConverterToTypeConversionTestType2IncludingDerived();

        var canHandle = converter.CanHandle(typeof(TypeConversionTestType1), typeof(TypeConversionTestType2));

        canHandle.Should().BeTrue();
    }

    [Fact]
    public void CanHandle_for_actual_source_and_derived_target_types_returns_true()
    {
        var converter = new TypeConversionTestType1ConverterToTypeConversionTestType2IncludingDerived();

        var canHandle = converter.CanHandle(typeof(TypeConversionTestType1), typeof(TypeConversionTestType2Derived));

        canHandle.Should().BeTrue();
    }

    [Fact]
    public void CanHandle_for_derived_source_and_actual_target_types_returns_true()
    {
        var converter = new TypeConversionTestType1ConverterToTypeConversionTestType2IncludingDerived();

        var canHandle = converter.CanHandle(typeof(TypeConversionTestType1Derived), typeof(TypeConversionTestType2));

        canHandle.Should().BeTrue();
    }

    [Fact]
    public void ConvertValue_with_null_source_value_returns_null_when_converter_supports_null_values()
    {
        // Arrange
        var converter = new TestNullableSourceConverter();

        // Act
        var result = converter.ConvertValue(null);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ConvertValue_with_non_null_source_value_returns_converted_target_value()
    {
        // Arrange
        var sourceValue = new TypeConversionTestType1("test-value");
        var converter = new TestSourceValueConverter();

        // Act
        var result = converter.ConvertValue(sourceValue);

        // Assert
        result.Should().NotBeNull();
        result!.Value2.Should().Be("test-value-converted");
    }

    [Fact]
    public void ConvertValue_should_throw_ArgumentNullException_if_can_handle_null_values_is_set_to_false()
    {
        var sut = new TestSourceValueConverter();
        var act = () => sut.ConvertValue(null);

        act.Should().Throw<ArgumentNullException>().WithParameterName("value");
    }
}

public class TypeConversionTestType1ConverterToTypeConversionTestType2IncludingDerived()
    : SingleSourceTargetTypeConverter<TypeConversionTestType1, TypeConversionTestType2>(true, true, true)
{
    protected override TypeConversionTestType2? DoConvert(TypeConversionTestType1? value, Type targetType) => throw new NotImplementedException();
}

public class TestNullableSourceConverter() : SingleSourceTargetTypeConverter<TypeConversionTestType1, TypeConversionTestType2>(true, false, false)
{
    protected override TypeConversionTestType2? DoConvert(TypeConversionTestType1? value, Type targetType) =>
        value == null ? null : new TypeConversionTestType2(value.Value);
}

public class TestSourceValueConverter() : SingleSourceTargetTypeConverter<TypeConversionTestType1, TypeConversionTestType2>(false, false, false)
{
    protected override TypeConversionTestType2? DoConvert(TypeConversionTestType1? value, Type targetType) =>
        value != null ? new TypeConversionTestType2($"{value.Value}-converted") : null;
}

public record TypeConversionTestType1(string Value);

public record TypeConversionTestType1Derived(bool BoolValue, string Value) : TypeConversionTestType1(Value);

public record TypeConversionTestType2(string Value2);

public record TypeConversionTestType2Derived(bool BoolValue2, string value2) : TypeConversionTestType2(value2);
