using FluentAssertions;
using Ploch.Common.Tests.Reflection;
using Ploch.Common.Tests.TypeConversion;
using Ploch.Common.TypeConversion;
using Xunit;

namespace Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TypeConversion;

public class EnumerationMapExtractorTests
{
    [Fact]
    public void GetEnumFieldValueMap_should_return_correct_value_when_field_exists()
    {
        // Arrange
        var enumType = typeof(TestTypes.TestEnum);
        var fieldName = nameof(TestTypes.TestEnum.FirstValue);

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);
        var actualFieldValue = valueMap[fieldName];

        // Assert
        actualFieldValue.Should().Be(TestTypes.TestEnum.FirstValue);
    }

    [Theory]
    [InlineData(typeof(TestTypes.TestEnum), nameof(TestTypes.TestEnum.FirstValue), TestTypes.TestEnum.FirstValue)]
    [InlineData(typeof(TestTypes.TestEnum), nameof(TestTypes.TestEnum.SecondValue), TestTypes.TestEnum.SecondValue)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion),
                "Value1",
                EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion.Value1)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion),
                "VALUE1",
                EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion.Value1)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion),
                "value1",
                EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion.Value1)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion),
                "Value2",
                EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion.Value2)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion),
                "VALUE2",
                EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion.Value2)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion),
                "value2",
                EnumerationFieldValueCacheTest.TestEnumWithCaseInsensitiveConversion.Value2)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithMapping),
                "ValueWithoutMappingAttribute",
                EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithoutMappingAttribute)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithMapping),
                "ValueWithOneMappedName1",
                EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithOneMappedName)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithMapping),
                "ValueWithMultipleMappedNames1",
                EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithMultipleMappedNames)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithMapping),
                "ValueWithMultipleMappedNames2",
                EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithMultipleMappedNames)]
    [InlineData(typeof(EnumerationFieldValueCacheTest.TestEnumWithMapping),
                "",
                EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithMultipleMappedNames)]
    public void GetFieldValue_should_return_mapped_enum_field_value(Type enumType, string name, object expectedFieldValue)
    {
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);
        var actualFieldValue = valueMap[name];

        actualFieldValue.Should().Be(expectedFieldValue);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_throw_if_enum_is_not_marked_as_case_insensitive_and_value_does_not_match_case()
    {
        var name = nameof(EnumerationFieldValueCacheTest.TestEnumWithCaseSensitiveMatching.Value1).ToUpper();
        var enumType = typeof(EnumerationFieldValueCacheTest.TestEnumWithCaseSensitiveMatching);

        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        valueMap.Should().NotContainKey(name);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_throw_invalid_operation_exception_when_field_does_not_exist()
    {
        // Arrange
        var enumType = typeof(TestTypes.TestEnum);
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
        var enumType = typeof(TestTypes.TestEnum);

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        valueMap.Should().ContainKey(nameof(TestTypes.TestEnum.FirstValue)).And.ContainValue(TestTypes.TestEnum.FirstValue);
        valueMap.Should().ContainKey(nameof(TestTypes.TestEnum.SecondValue)).And.ContainValue(TestTypes.TestEnum.SecondValue);
    }

    [Fact]
    public void GetEnumFieldValueMap_should_return_correct_mapping_for_valid_enum_with_mappings()
    {
        // Arrange
        var enumType = typeof(EnumerationFieldValueCacheTest.TestEnumWithMapping);

        // Act
        var valueMap = EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        // Assert
        valueMap.Should().HaveCount(5);
        valueMap.Should()
                .ContainKey("ValueWithoutMappingAttribute")
                .And.ContainValue(EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithoutMappingAttribute);
        valueMap.Should().ContainKey("ValueWithOneMappedName1").And.ContainValue(EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithOneMappedName);
        valueMap.Should()
                .ContainKey("ValueWithMultipleMappedNames1")
                .And.ContainValue(EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithMultipleMappedNames);
        valueMap.Should()
                .ContainKey("ValueWithMultipleMappedNames2")
                .And.ContainValue(EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithMultipleMappedNames);
        valueMap.Should().ContainKey(string.Empty).And.ContainValue(EnumerationFieldValueCacheTest.TestEnumWithMapping.ValueWithMultipleMappedNames);
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
        var enumType = typeof(EnumerationFieldValueCacheTest.TestEnumWithDuplicateMapping);

        // Act
        Action act = () => EnumerationMapExtractor.GetEnumFieldValueMap(enumType);

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage("*Duplicate*mapped*");
    }
}
