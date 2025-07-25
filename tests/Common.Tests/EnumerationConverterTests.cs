using System.Globalization;
using FluentAssertions;
using Ploch.Common.Tests.TestTypes.TestingTypes;

#pragma warning disable CS8604 // Possible null reference argument.

namespace Ploch.Common.Tests;

public class EnumerationConverterTests
{
    [Fact]
    public void ParseToEnum_should_throw_ArgumentNullException_if_value_is_null()
    {
        string? value = null;

        var act = () => value.ParseToEnum<TestEnum>();

        act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_valid_enum_value_with_exact_case_matching()
    {
        // Arrange
        var value = "FirstValue";

        // Act
        var result = value.ParseToEnum<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.FirstValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_int_representing_enum_flags()
    {
        // Arrange
        var flagsValue = TestEnumWithFlags.FirstValue | TestEnumWithFlags.SecondValue;

        var flagsIntValue = (int)flagsValue;

        var x = (TestEnumWithFlags)flagsIntValue;
        x.HasFlag(TestEnumWithFlags.FirstValue).Should().BeTrue();
        x.HasFlag(TestEnumWithFlags.SecondValue).Should().BeTrue();
        x.HasFlag(TestEnumWithFlags.ThirdValue).Should().BeFalse();

        // Act
        var testEnumWithFlags = flagsIntValue.ParseToEnum<TestEnumWithFlags>();

        var hasFlagFirstValue = testEnumWithFlags.HasFlag(TestEnumWithFlags.FirstValue);
        var hasFlagSecondValue = testEnumWithFlags.HasFlag(TestEnumWithFlags.SecondValue);
        var hasFlagThirdValue = testEnumWithFlags.HasFlag(TestEnumWithFlags.ThirdValue);

        hasFlagFirstValue.Should().BeTrue();
        hasFlagSecondValue.Should().BeTrue();
        hasFlagThirdValue.Should().BeFalse();
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_valid_enum_value_with_case_insensitive_matching()
    {
        // Arrange
        var value = "firstvalue"; // Lowercase version of the enum value

        // Act
        var result = value.ParseToEnum<TestEnum>(true);

        // Assert
        result.Should().Be(TestEnum.FirstValue);
    }

    [Fact]
    public void ParseToEnum_should_throw_ArgumentOutOfRangeException_when_string_value_does_not_match_any_enum_value()
    {
        // Arrange
        var value = "NonExistentValue";

        // Act
        var act = () => value.ParseToEnum<TestEnum>();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().Which.Message.Should().Contain($"'{value}' is not a valid value for enum type 'TestEnum'");
    }

    [Fact]
    public void ParseToEnum_should_throw_ArgumentNullException_when_value_is_empty_string()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var act = () => value.ParseToEnum<TestEnum>();

        // Assert
        act.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void SafeParseToEnum_should_return_null_when_input_is_null()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.SafeParseToEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafeParseToEnum_should_return_null_when_input_is_whitespace()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = value.SafeParseToEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafeParseToEnum_should_return_null_when_input_does_not_match_any_enum_value()
    {
        // Arrange
        var value = "NonExistentValue";

        // Act
        var result = value.SafeParseToEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafeParseToEnum_should_correctly_parse_valid_enum_value_with_exact_case_matching()
    {
        // Arrange
        var value = "FirstValue";

        // Act
        var result = value.SafeParseToEnum<TestEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestEnum.FirstValue);
    }

    [Fact]
    public void SafeParseToEnum_should_correctly_parse_valid_enum_value_with_case_insensitive_matching()
    {
        // Arrange
        var value = "firstValue"; // Lowercase version of the enum value

        // Act
        var result = value.SafeParseToEnum<TestEnum>(true);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestEnum.FirstValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_enum_values_with_special_characters_or_spaces()
    {
        // Arrange
        var value = "Special_Value_With_Characters";

        // Act
        var result = value.ParseToEnum<EnumWithSpecialChars>();

        // Assert
        result.Should().Be(EnumWithSpecialChars.Special_Value_With_Characters);
    }

    [Fact]
    public void ParseToEnum_should_handle_different_format_providers_when_parsing_int_to_enum()
    {
        // Arrange
        var enumValue = (int)TestEnum.SecondValue;
        var germanCulture = new CultureInfo("de-DE");

        // Act
        var result = enumValue.ParseToEnum<TestEnum>(germanCulture);

        // Assert
        result.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_int_to_enum_with_null_format_provider()
    {
        // Arrange
        var enumValue = (int)TestEnum.ThirdValue;

        // Act
        var result = enumValue.ParseToEnum<TestEnum>();

        // Assert
        result.Should().Be(TestEnum.ThirdValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_custom_enum_values()
    {
        // Arrange
        var minEnumValue = (int)EnumWithCustomValues.MinValue;
        var maxEnumValue = (int)EnumWithCustomValues.MaxValue;
        var secondValue = (int)EnumWithCustomValues.SecondValue;

        // Act
        minEnumValue.ParseToEnum<EnumWithCustomValues>().Should().Be(EnumWithCustomValues.MinValue);
        maxEnumValue.ParseToEnum<EnumWithCustomValues>().Should().Be(EnumWithCustomValues.MaxValue);
        secondValue.ParseToEnum<EnumWithCustomValues>().Should().Be(EnumWithCustomValues.SecondValue);
    }

    [Fact]
    public void SafaParseToEnum_should_return_null_when_input_is_null()
    {
        // Arrange
        int? value = null;

        // Act
        var result = value.SafaParseToEnum<TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafaParseToEnum_should_correctly_convert_valid_nullable_int_to_enum()
    {
        // Arrange
        int? enumValue = (int)TestEnum.SecondValue;

        // Act
        var result = enumValue.SafaParseToEnum<TestEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void SafaParseToEnum_should_handle_custom_format_providers_when_parsing_nullable_int_to_enum()
    {
        // Arrange
        int? enumValue = (int)TestEnum.SecondValue;
        var germanCulture = new CultureInfo("de-DE");

        // Act
        var result = enumValue.SafaParseToEnum<TestEnum>(germanCulture);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestEnum.SecondValue);
    }

    [Fact]
    public void SafaParseToEnum_should_correctly_handle_enum_flags_when_converting_from_nullable_int()
    {
        // Arrange
        int? flagsValue = (int)(TestEnumWithFlags.FirstValue | TestEnumWithFlags.SecondValue);

        // Act
        var result = flagsValue.SafaParseToEnum<TestEnumWithFlags>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveFlag(TestEnumWithFlags.FirstValue);
        result.Should().HaveFlag(TestEnumWithFlags.SecondValue);
        result.Should().NotHaveFlag(TestEnumWithFlags.ThirdValue);
    }
}
