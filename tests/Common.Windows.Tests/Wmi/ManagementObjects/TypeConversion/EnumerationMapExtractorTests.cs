using FluentAssertions;
using Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TypeConversion;

public class EnumerationMapExtractorTests
{
    [Fact]
    public void GetEnumFieldValueMap_should_return_correct_value_when_field_exists()
    {
        // Arrange
        var enumType = typeof(TestEnum);
        var fieldName = nameof(TestEnum.Value1);

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);
        var actualFieldValue = valueMap[fieldName];

        // Assert
        actualFieldValue.Should().Be(TestEnum.Value1);
    }

    [Theory]
    [InlineData(typeof(TestEnum), "Value1", TestEnum.Value1)]
    [InlineData(typeof(TestEnum), "Value2", TestEnum.Value2)]
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
    [InlineData(typeof(TestEnumWithMapping), "ValueWithoutMappingAttribute", TestEnumWithMapping.ValueWithoutMappingAttribute)]
    public void GetFieldValue_should_return_mapped_enum_field_value(Type enumType, string name, object expectedFieldValue)
    {
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);
        var actualFieldValue = valueMap[name];

        actualFieldValue.Should().Be(expectedFieldValue);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_throw_if_enum_is_not_marked_as_case_insensitive_and_value_does_not_match_case()
    {
        var name = nameof(TestEnumWithCaseSensitiveMatching.Value1).ToUpper();
        var enumType = typeof(TestEnumWithCaseSensitiveMatching);

        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        valueMap.Should().NotContainKey(name);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_throw_invalid_operation_exception_when_field_does_not_exist()
    {
        // Arrange
        var enumType = typeof(TestEnum);
        var fieldName = "NonExistentField";

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        valueMap.Should().NotContainKey(fieldName);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_return_correct_mapping_for_valid_enum()
    {
        // Arrange
        var enumType = typeof(TestEnum);

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        valueMap.Should().ContainKey(nameof(TestEnum.Value1)).And.ContainValue(TestEnum.Value1);
        valueMap.Should().ContainKey(nameof(TestEnum.Value2)).And.ContainValue(TestEnum.Value2);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_return_correct_mapping_for_valid_enum_with_mappings()
    {
        // Arrange
        var enumType = typeof(TestEnumWithMapping);

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        // Assert
        valueMap.Should().HaveCount(5);
        valueMap.Should().ContainKey("ValueWithoutMappingAttribute").And.ContainValue(TestEnumWithMapping.ValueWithoutMappingAttribute);
        valueMap.Should().ContainKey("ValueWithOneMappedName1").And.ContainValue(TestEnumWithMapping.ValueWithOneMappedName);
        valueMap.Should().ContainKey("ValueWithMultipleMappedNames1").And.ContainValue(TestEnumWithMapping.ValueWithMultipleMappedNames);
        valueMap.Should().ContainKey("ValueWithMultipleMappedNames2").And.ContainValue(TestEnumWithMapping.ValueWithMultipleMappedNames);
        valueMap.Should().ContainKey(string.Empty).And.ContainValue(TestEnumWithMapping.ValueWithMultipleMappedNames);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_throw_invalid_operation_exception_when_type_is_not_enum()
    {
        // Arrange
        var nonEnumType = typeof(string);

        // Act
        Action act = () => EnumerationMapExtractor.GetEnumFieldValueMap(nonEnumType);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"*{nonEnumType}*not*enumeration*");
    }

    [Fact]
    public void AddValueMappings_should_throw_invalid_operation_exception_when_duplicate_mapping_exists()
    {
        // Arrange
        var enumType = typeof(TestEnumWithDuplicateMapping);

        // Act
        Action act = () => EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*Duplicate*mapped*");
    }
}
