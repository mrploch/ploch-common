using System.Globalization;
using FluentAssertions;
using Ploch.Common.Tests.Reflection;
using Xunit;

#pragma warning disable CS8604 // Possible null reference argument.

namespace Ploch.Common.Tests;

public class EnumerationConverterTests
{
    [Fact]
    public void ParseToEnum_should_throw_ArgumentNullException_if_value_is_null()
    {
        string? value = null;

        var act = () => value.ParseToEnum<TestTypes.TestEnum>();

        act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_valid_enum_value_with_exact_case_matching()
    {
        // Arrange
        var value = "FirstValue";

        // Act
        var result = value.ParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().Be(TestTypes.TestEnum.FirstValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_int_representing_enum_flags()
    {
        // Arrange
        var flagsValue = TestTypes.TestEnumWithFlags.FirstValue | TestTypes.TestEnumWithFlags.SecondValue;

        var flagsIntValue = (int)flagsValue;

        var x = (TestTypes.TestEnumWithFlags)flagsIntValue;
        x.HasFlag(TestTypes.TestEnumWithFlags.FirstValue).Should().BeTrue();
        x.HasFlag(TestTypes.TestEnumWithFlags.SecondValue).Should().BeTrue();
        x.HasFlag(TestTypes.TestEnumWithFlags.ThirdValue).Should().BeFalse();

        // Act
        var testEnumWithFlags = flagsIntValue.ParseToEnum<TestTypes.TestEnumWithFlags>();

        var hasFlagFirstValue = testEnumWithFlags.HasFlag(TestTypes.TestEnumWithFlags.FirstValue);
        var hasFlagSecondValue = testEnumWithFlags.HasFlag(TestTypes.TestEnumWithFlags.SecondValue);
        var hasFlagThirdValue = testEnumWithFlags.HasFlag(TestTypes.TestEnumWithFlags.ThirdValue);

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
        var result = value.ParseToEnum<TestTypes.TestEnum>(true);

        // Assert
        result.Should().Be(TestTypes.TestEnum.FirstValue);
    }

    [Fact]
    public void ParseToEnum_should_throw_ArgumentOutOfRangeException_when_string_value_does_not_match_any_enum_value()
    {
        // Arrange
        var value = "NonExistentValue";

        // Act
        var act = () => value.ParseToEnum<TestTypes.TestEnum>();

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().Which.Message.Should().Contain($"'{value}' is not a valid value for enum type 'TestEnum'");
    }

    [Fact]
    public void ParseToEnum_should_throw_ArgumentNullException_when_value_is_empty_string()
    {
        // Arrange
        var value = string.Empty;

        // Act
        var act = () => value.ParseToEnum<TestTypes.TestEnum>();

        // Assert
        act.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(nameof(value));
    }

    [Fact]
    public void SafeParseToEnum_should_return_null_when_input_is_null()
    {
        // Arrange
        string? value = null;

        // Act
        var result = value.SafeParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafeParseToEnum_should_return_null_when_input_is_whitespace()
    {
        // Arrange
        var value = "   ";

        // Act
        var result = value.SafeParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafeParseToEnum_should_return_null_when_input_does_not_match_any_enum_value()
    {
        // Arrange
        var value = "NonExistentValue";

        // Act
        var result = value.SafeParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafeParseToEnum_should_correctly_parse_valid_enum_value_with_exact_case_matching()
    {
        // Arrange
        var value = "FirstValue";

        // Act
        var result = value.SafeParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestTypes.TestEnum.FirstValue);
    }

    [Fact]
    public void SafeParseToEnum_should_correctly_parse_valid_enum_value_with_case_insensitive_matching()
    {
        // Arrange
        var value = "firstValue"; // Lowercase version of the enum value

        // Act
        var result = value.SafeParseToEnum<TestTypes.TestEnum>(true);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestTypes.TestEnum.FirstValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_enum_values_with_special_characters_or_spaces()
    {
        // Arrange
        var value = "Special_Value_With_Characters";

        // Act
        var result = value.ParseToEnum<TestTypes.EnumWithSpecialChars>();

        // Assert
        result.Should().Be(TestTypes.EnumWithSpecialChars.Special_Value_With_Characters);
    }

    [Fact]
    public void ParseToEnum_should_handle_different_format_providers_when_parsing_int_to_enum()
    {
        // Arrange
        var enumValue = (int)TestTypes.TestEnum.SecondValue;
        var germanCulture = new CultureInfo("de-DE");

        // Act
        var result = enumValue.ParseToEnum<TestTypes.TestEnum>(germanCulture);

        // Assert
        result.Should().Be(TestTypes.TestEnum.SecondValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_int_to_enum_with_null_format_provider()
    {
        // Arrange
        var enumValue = (int)TestTypes.TestEnum.ThirdValue;

        // Act
        var result = enumValue.ParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().Be(TestTypes.TestEnum.ThirdValue);
    }

    [Fact]
    public void ParseToEnum_should_correctly_parse_custom_enum_values()
    {
        // Arrange
        var minEnumValue = (int)TestTypes.EnumWithCustomValues.MinValue;
        var maxEnumValue = (int)TestTypes.EnumWithCustomValues.MaxValue;
        var secondValue = (int)TestTypes.EnumWithCustomValues.SecondValue;

        // Act
        minEnumValue.ParseToEnum<TestTypes.EnumWithCustomValues>().Should().Be(TestTypes.EnumWithCustomValues.MinValue);
        maxEnumValue.ParseToEnum<TestTypes.EnumWithCustomValues>().Should().Be(TestTypes.EnumWithCustomValues.MaxValue);
        secondValue.ParseToEnum<TestTypes.EnumWithCustomValues>().Should().Be(TestTypes.EnumWithCustomValues.SecondValue);
    }

    [Fact]
    public void SafaParseToEnum_should_return_null_when_input_is_null()
    {
        // Arrange
        int? value = null;

        // Act
        var result = value.SafaParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void SafaParseToEnum_should_correctly_convert_valid_nullable_int_to_enum()
    {
        // Arrange
        int? enumValue = (int)TestTypes.TestEnum.SecondValue;

        // Act
        var result = enumValue.SafaParseToEnum<TestTypes.TestEnum>();

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestTypes.TestEnum.SecondValue);
    }

    [Fact]
    public void SafaParseToEnum_should_handle_custom_format_providers_when_parsing_nullable_int_to_enum()
    {
        // Arrange
        int? enumValue = (int)TestTypes.TestEnum.SecondValue;
        var germanCulture = new CultureInfo("de-DE");

        // Act
        var result = enumValue.SafaParseToEnum<TestTypes.TestEnum>(germanCulture);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(TestTypes.TestEnum.SecondValue);
    }

    [Fact]
    public void SafaParseToEnum_should_correctly_handle_enum_flags_when_converting_from_nullable_int()
    {
        // Arrange
        int? flagsValue = (int)(TestTypes.TestEnumWithFlags.FirstValue | TestTypes.TestEnumWithFlags.SecondValue);

        // Act
        var result = flagsValue.SafaParseToEnum<TestTypes.TestEnumWithFlags>();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveFlag(TestTypes.TestEnumWithFlags.FirstValue);
        result.Should().HaveFlag(TestTypes.TestEnumWithFlags.SecondValue);
        result.Should().NotHaveFlag(TestTypes.TestEnumWithFlags.ThirdValue);
    }
}
