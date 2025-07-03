using System.Reflection;
using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class MemberInfoExtensionsTests
{
    [Fact]
    public void GetValue_should_retrieve_field_value_from_object()
    {
        var testObject = new TestTypes.ClassWithFieldsAndProperties();

        MemberInfo? member = testObject.GetType().GetField(nameof(TestTypes.ClassWithFieldsAndProperties.BoolField));

        member.GetValue(testObject).Should().Be(testObject.BoolField);
    }

    [Fact]
    public void GetValue_should_retrieve_property_value_from_object()
    {
        var testObject = new TestTypes.ClassWithFieldsAndProperties();

        MemberInfo? member = testObject.GetType().GetProperty(nameof(TestTypes.ClassWithFieldsAndProperties.StringFieldProperty));

        member.GetValue(testObject).Should().Be(testObject.StringFieldProperty);
    }

    [Fact]
    public void GetValue_should_return_null_when_memberInfo_is_neither_field_nor_property()
    {
        var testObject = new TestTypes.ClassWithFieldsAndProperties();

        // Get a MethodInfo which is neither FieldInfo nor PropertyInfo
        MemberInfo? member = testObject.GetType().GetMethod("ToString");

        // Act & Assert
        member.GetValue(testObject).Should().BeNull();
    }

    [Fact]
    public void GetValue_should_handle_null_object_parameter()
    {
        // Arrange
        var testType = typeof(TestTypes.ClassWithFieldsAndProperties);
        MemberInfo fieldMember = testType.GetField(nameof(TestTypes.ClassWithFieldsAndProperties.BoolField));
        MemberInfo propertyMember = testType.GetProperty(nameof(TestTypes.ClassWithFieldsAndProperties.StringFieldProperty));

        // Act & Assert
        // For static members this would be valid - passing null as instance
        fieldMember.GetValue(null).Should().Be(null);
        propertyMember.GetValue(null).Should().Be(null);

        // Test with index parameter to ensure it handles that case too
        propertyMember.GetValue(null, 0).Should().Be(null);
    }

    [Fact]
    public void GetValue_should_retrieve_static_field_value_with_null_object()
    {
        // Arrange
        var testType = typeof(TestTypes.TestClassWithStaticFieldsAndProperties);
        MemberInfo staticFieldMember = testType.GetField(nameof(TestTypes.TestClassWithStaticFieldsAndProperties.PublicStaticField));

        // Set a value to the static field for testing
        var expectedValue = Guid.NewGuid().ToString();
        TestTypes.TestClassWithStaticFieldsAndProperties.PublicStaticField = expectedValue;

        // Act
        var result = staticFieldMember.GetValue(null);

        // Assert
        result.Should().Be(expectedValue);
    }

    [Fact]
    public void GetValue_should_retrieve_indexed_property_value_with_valid_indices()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithIndexer();
        var indexValue = 5;
        testObject[indexValue] = "test value";

        // Get the indexer property
        MemberInfo? indexerProperty = testObject.GetType().GetProperties().FirstOrDefault(p => p.GetIndexParameters().Length > 0);

        // Act
        var result = indexerProperty.GetValue(testObject, indexValue);

        // Assert
        result.Should().Be("test value");
        indexerProperty.IsIndexer().Should().BeTrue();
    }

    [Fact]
    public void GetValue_should_retrieve_property_with_multiple_index_parameters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithMultiIndexer();
        var rowIndex = 2;
        var colIndex = 3;
        var expectedValue = "test multi index";
        testObject[rowIndex, colIndex] = expectedValue;

        // Get the multi-parameter indexer property
        MemberInfo? multiIndexerProperty = testObject.GetType().GetProperties().FirstOrDefault(p => p.GetIndexParameters().Length > 1);

        // Act
        var result = multiIndexerProperty.GetValue(testObject, rowIndex, colIndex);

        // Assert
        result.Should().Be(expectedValue);
        multiIndexerProperty.IsIndexer().Should().BeTrue();
    }

    [Fact]
    public void GetValue_should_retrieve_private_field_value_from_object()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithPrivateMembers();
        var privateValue = "private field value";

        // Set private field value using reflection
        MemberInfo privateField =
            testObject.GetType().GetField(TestTypes.ClassWithPrivateMembers.PrivateFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        // Act
        var result = privateField.GetValue(testObject);

        // Assert
        result.Should().Be(TestTypes.ClassWithPrivateMembers.PrivateFieldValue);
    }
}
