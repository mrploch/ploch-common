using System.Globalization;
using Ploch.Common.Tests.TestTypes.TestingTypes;
using Ploch.Common.TypeConversion;

namespace Ploch.Common.Tests.TypeConversion;

public class EnumerationFieldValueCacheTest
{
    [EnumConversion]
    public enum TestEnumWithCaseInsensitiveConversion
    {
        Value1,
        Value2
    }

    [EnumConversion(true)]
    public enum TestEnumWithCaseSensitiveMatching
    {
        Value1,
        Value2
    }

    public enum TestEnumWithDuplicateMapping
    {
        Value1,

        [EnumMapping("Duplicate")]
        Value2,

        [EnumMapping("Duplicate")]
        Value3
    }

    public enum TestEnumWithMapping
    {
        [EnumMapping("ValueWithOneMappedName1")]
        ValueWithOneMappedName,

        [EnumMapping("ValueWithMultipleMappedNames1", "ValueWithMultipleMappedNames2", null)]
        ValueWithMultipleMappedNames,
        ValueWithoutMappingAttribute
    }

    [Fact]
    public void GetFieldValue_should_return_correct_value_when_field_exists()
    {
        // Arrange
        var enumType = typeof(TestEnum);
        var fieldName = nameof(TestEnum.FirstValue);

        // Act
        var result = EnumerationFieldValueCache.GetFieldValue(enumType, fieldName);

        // Assert
        result.Should().Be(TestEnum.FirstValue);
    }

    [Theory]
    [InlineData(typeof(TestEnum), "FirstValue", TestEnum.FirstValue)]
    [InlineData(typeof(TestEnum), "SecondValue", TestEnum.SecondValue)]
    [InlineData(typeof(TestEnumWithCaseInsensitiveConversion), "Value1", TestEnumWithCaseInsensitiveConversion.Value1)]
    [InlineData(typeof(TestEnumWithCaseInsensitiveConversion), "VALUE1", TestEnumWithCaseInsensitiveConversion.Value1)]
    [InlineData(typeof(TestEnumWithCaseInsensitiveConversion), "value1", TestEnumWithCaseInsensitiveConversion.Value1)]
    [InlineData(typeof(TestEnumWithCaseInsensitiveConversion), "Value2", TestEnumWithCaseInsensitiveConversion.Value2)]
    [InlineData(typeof(TestEnumWithCaseInsensitiveConversion), "VALUE2", TestEnumWithCaseInsensitiveConversion.Value2)]
    [InlineData(typeof(TestEnumWithCaseInsensitiveConversion), "value2", TestEnumWithCaseInsensitiveConversion.Value2)]
    [InlineData(typeof(TestEnumWithMapping), "ValueWithoutMappingAttribute", TestEnumWithMapping.ValueWithoutMappingAttribute)]
    [InlineData(typeof(TestEnumWithMapping), "ValueWithOneMappedName1", TestEnumWithMapping.ValueWithOneMappedName)]
    [InlineData(typeof(TestEnumWithMapping), "ValueWithMultipleMappedNames1", TestEnumWithMapping.ValueWithMultipleMappedNames)]
    [InlineData(typeof(TestEnumWithMapping), "ValueWithMultipleMappedNames2", TestEnumWithMapping.ValueWithMultipleMappedNames)]
    [InlineData(typeof(TestEnumWithMapping), "", TestEnumWithMapping.ValueWithMultipleMappedNames)]
    public void GetFieldValue_should_return_mapped_enum_field_value(Type enumType, string name, object expectedFieldValue)
    {
        var actualFieldValue = EnumerationFieldValueCache.GetFieldValue(enumType, name);

        actualFieldValue.Should().Be(expectedFieldValue);
    }

    [Fact]
    public void GetFieldValue_should_throw_if_enum_is_not_marked_as_case_insensitive_and_value_does_not_match_case()
    {
        var name = nameof(TestEnumWithCaseSensitiveMatching.Value1).ToUpper(CultureInfo.InvariantCulture);
        var enumType = typeof(TestEnumWithCaseSensitiveMatching);
        Action act = () => EnumerationFieldValueCache.GetFieldValue(enumType, name);

        act.Should().Throw<InvalidOperationException>().WithMessage($"*{name}*not found*{enumType}*");
    }

    [Fact]
    public void GetFieldValue_should_throw_invalid_operation_exception_when_field_does_not_exist()
    {
        // Arrange
        var enumType = typeof(TestEnum);
        var fieldName = "NonExistentField";

        // Act
        Action act = () => EnumerationFieldValueCache.GetFieldValue(enumType, fieldName);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"*{fieldName}*not found*{enumType}*");
    }

    [Fact]
    public void GetFieldsMapping_should_return_correct_mapping_for_valid_enum()
    {
        // Arrange
        var enumType = typeof(TestEnum);

        // Act
        var result = EnumerationFieldValueCache.GetFieldsMapping(enumType);

        // Assert
        result.Should().ContainKey(nameof(TestEnum.FirstValue)).And.ContainValue(TestEnum.FirstValue);
        result.Should().ContainKey(nameof(TestEnum.SecondValue)).And.ContainValue(TestEnum.SecondValue);
    }

    [Fact]
    public void GetFieldsMapping_should_return_correct_mapping_for_valid_enum_with_mappings()
    {
        // Arrange
        var enumType = typeof(TestEnumWithMapping);

        // Act
        var result = EnumerationFieldValueCache.GetFieldsMapping(enumType);

        // Assert
        // Assert
        result.Should().HaveCount(5);
        result.Should().ContainKey("ValueWithoutMappingAttribute").And.ContainValue(TestEnumWithMapping.ValueWithoutMappingAttribute);
        result.Should().ContainKey("ValueWithOneMappedName1").And.ContainValue(TestEnumWithMapping.ValueWithOneMappedName);
        result.Should().ContainKey("ValueWithMultipleMappedNames1").And.ContainValue(TestEnumWithMapping.ValueWithMultipleMappedNames);
        result.Should().ContainKey("ValueWithMultipleMappedNames2").And.ContainValue(TestEnumWithMapping.ValueWithMultipleMappedNames);
        result.Should().ContainKey(new(null)).And.ContainValue(TestEnumWithMapping.ValueWithMultipleMappedNames);
    }

    [Fact]
    public void GetFieldsMapping_should_throw_invalid_operation_exception_when_type_is_not_enum()
    {
        // Arrange
        var nonEnumType = typeof(string);

        // Act
        Action act = () => EnumerationFieldValueCache.GetFieldsMapping(nonEnumType);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"Type {nonEnumType} is not an enumeration");
    }

    [Fact]
    public void AddValueMappings_should_throw_invalid_operation_exception_when_duplicate_mapping_exists()
    {
        // Arrange
        var enumType = typeof(TestEnumWithDuplicateMapping);

        // Act
        Action act = () => EnumerationFieldValueCache.GetFieldsMapping(enumType);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*Duplicate*mapped*");
    }
}
