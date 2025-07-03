using System.Linq.Expressions;
using AutoFixture.Xunit2;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class PropertyHelpersGetPropertyValueTests
{
    [Fact]
    public void GetPropertyValue_for_object_should_correctly_retrieve_value_for_indexed_properties()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithIndexer();
        testObject[0] = "Value0";
        testObject[1] = "Value1";

        // Act
        var result0 = testObject.GetPropertyValue(PropertyHelpers.IndexerPropertyName, [ 0 ]);
        var result1 = testObject.GetPropertyValue(PropertyHelpers.IndexerPropertyName, [ 1 ]);

        // Assert
        result0.Should().Be("Value0");
        result1.Should().Be("Value1");
    }

    [Theory]
    [AutoMockData]
    public void GetPropertyValue_for_object_should_handle_explicitly_implemented_interface_properties(TestTypes.ClassImplementingInterface testObject)
    {
        // Arrange
        TestTypes.ITestInterface interfaceObject = testObject;

        // Act
        var result = interfaceObject.GetPropertyValue(x => x.InterfaceProperty);

        // Assert
        result.Should().Be(testObject.InterfaceProperty);
    }

    [Fact]
    public void GetPropertyValue_for_object_should_handle_properties_from_implemented_interfaces()
    {
        // Arrange
        TestTypes.ITestInterface testObject = new TestTypes.ClassImplementingInterface();

        // Act
        var result = testObject.GetPropertyValue(nameof(TestTypes.ITestInterface.InterfaceProperty));

        // Assert
        result.Should().Be(TestTypes.ClassImplementingInterface.DefaultInterfacePropertyValue);
    }

    [Fact]
    public void GetPropertyValue_for_object_should_handle_properties_with_custom_getters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithCustomGetter();

        // Act
        var result = testObject.GetPropertyValue(nameof(TestTypes.ClassWithCustomGetter.PropertyWithCustomGetter));

        // Assert
        result.Should().Be(TestTypes.ClassWithCustomGetter.CustomGetterValue);
    }

    [Fact]
    public void GetPropertyValue_for_object_should_handle_properties_with_private_setters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithPrivateSetter();

        // Act
        var result = testObject.GetPropertyValue(nameof(TestTypes.ClassWithPrivateSetter.PropertyWithPrivateSetter));

        // Assert
        result.Should().Be(TestTypes.ClassWithPrivateSetter.DefaultValue);
    }

    [Theory]
    [AutoMockData]
    public void GetPropertyValue_for_object_should_handle_value_and_reference_types(TestTypes.ClassWithValueAndReferenceTypes testObject)
    {
        // Act
        var valueTypeResult = testObject.GetPropertyValue(nameof(TestTypes.ClassWithValueAndReferenceTypes.ValueTypeProperty));
        var referenceTypeResult = testObject.GetPropertyValue(nameof(TestTypes.ClassWithValueAndReferenceTypes.ReferenceTypeProperty));

        // Assert
        valueTypeResult.Should().Be(testObject.ValueTypeProperty);
        referenceTypeResult.Should().Be(testObject.ReferenceTypeProperty);
    }

    [Fact]
    public void GetPropertyValue_for_object_should_throw_ArgumentNullException_when_obj_is_null()
    {
        // Arrange
        TestTypes.MyTestClass? testObject = null;

        // Act
        Action action = () => testObject.GetPropertyValue(nameof(TestTypes.MyTestClass.IntProp)!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("obj");
    }

    [Fact]
    public void GetPropertyValue_for_object_should_throw_PropertyAccessException_when_index_is_of_different_type_than_actual()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithIndexer();
        testObject[0] = "Value0";
        testObject[1] = "Value1";

        // Act
        Action action = () => testObject.GetPropertyValue(PropertyHelpers.IndexerPropertyName, [ "a" ]);

        // Assert
        action.Should().ThrowExactly<PropertyIndexerMismatchException>().Which.PropertyName.Should().Be(PropertyHelpers.IndexerPropertyName);
    }

    [Fact]
    public void GetPropertyValue_for_object_should_throw_PropertyWriteOnlyException_for_write_only_property()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithWriteOnlyProperty();

        // Act
        Action action = () => testObject.GetPropertyValue(nameof(TestTypes.ClassWithWriteOnlyProperty.WriteOnlyProperty));

        // Assert
        action.Should()
              .ThrowExactly<PropertyWriteOnlyException>()
              .Which.PropertyName.Should()
              .Be(nameof(TestTypes.ClassWithWriteOnlyProperty.WriteOnlyProperty));
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_correctly_retrieve_value_for_indexed_properties()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithIndexer();
        testObject[0] = "Value0";
        testObject[1] = "Value1";
        var indexerProperty = typeof(TestTypes.ClassWithIndexer).GetProperty(PropertyHelpers.IndexerPropertyName);

        // Act
        var result0 = testObject.GetPropertyValue(indexerProperty!, [ 0 ]);
        var result1 = testObject.GetPropertyValue(indexerProperty!, [ 1 ]);

        // Assert
        result0.Should().Be("Value0");
        result1.Should().Be("Value1");
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_handle_properties_from_implemented_interfaces()
    {
        // Arrange
        TestTypes.ITestInterface testObject = new TestTypes.ClassImplementingInterface();
        var propertyInfo = typeof(TestTypes.ITestInterface).GetProperty(nameof(TestTypes.ITestInterface.InterfaceProperty));

        // Act
        var result = testObject.GetPropertyValue(propertyInfo!);

        // Assert
        result.Should().Be(TestTypes.ClassImplementingInterface.DefaultInterfacePropertyValue);
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_handle_properties_with_custom_getters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithCustomGetter();
        var propertyInfo = typeof(TestTypes.ClassWithCustomGetter).GetProperty(nameof(TestTypes.ClassWithCustomGetter.PropertyWithCustomGetter))!;

        // Act
        var result = testObject.GetPropertyValue(propertyInfo);

        // Assert
        result.Should().Be(TestTypes.ClassWithCustomGetter.CustomGetterValue);
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_handle_properties_with_private_setters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithPrivateSetter();
        var propertyInfo = typeof(TestTypes.ClassWithPrivateSetter).GetProperty(nameof(TestTypes.ClassWithPrivateSetter.PropertyWithPrivateSetter))!;

        // Act
        var result = testObject.GetPropertyValue(propertyInfo);

        // Assert
        result.Should().Be(TestTypes.ClassWithPrivateSetter.DefaultValue);
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_handle_properties_with_protected_setters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithProtectedSetter();
        var propertyInfo = typeof(TestTypes.ClassWithProtectedSetter).GetProperty(nameof(TestTypes.ClassWithProtectedSetter.PropertyWithProtectedSetter))!;

        // Act
        var result = testObject.GetPropertyValue(propertyInfo);

        // Assert
        result.Should().Be(TestTypes.ClassWithProtectedSetter.DefaultValue);
    }

    [Theory]
    [AutoMockData]
    public void GetPropertyValue_for_PropertyInfo_should_handle_value_and_reference_types(TestTypes.ClassWithValueAndReferenceTypes testObject)
    {
        // Arrange
        var valueTypePropertyInfo =
            typeof(TestTypes.ClassWithValueAndReferenceTypes).GetProperty(nameof(TestTypes.ClassWithValueAndReferenceTypes.ValueTypeProperty));
        var referenceTypePropertyInfo =
            typeof(TestTypes.ClassWithValueAndReferenceTypes).GetProperty(nameof(TestTypes.ClassWithValueAndReferenceTypes.ReferenceTypeProperty));

        // Act
        var valueTypeResult = testObject.GetPropertyValue(valueTypePropertyInfo!);
        var referenceTypeResult = testObject.GetPropertyValue(referenceTypePropertyInfo!);

        // Assert
        valueTypeResult.Should().Be(testObject.ValueTypeProperty);
        referenceTypeResult.Should().Be(testObject.ReferenceTypeProperty);
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_throw_PropertyAccessException_when_index_is_of_different_type_than_actual()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithIndexer();
        testObject[0] = "Value0";
        testObject[1] = "Value1";

        var propertyInfo = typeof(TestTypes.ClassWithIndexer).GetProperty(PropertyHelpers.IndexerPropertyName);

        // Act

        Action action = () => testObject.GetPropertyValue(propertyInfo!, [ "a" ]);

        // Assert
        action.Should().ThrowExactly<PropertyIndexerMismatchException>().Which.PropertyName.Should().Be(PropertyHelpers.IndexerPropertyName);
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_throw_PropertyIndexerMismatchException_when_index_is_missing_for_indexed_property()
    {
        // Arrange
        var testObject = new TestClass();
        var propertyInfo = typeof(TestClass).GetProperty("Item");

        // Act
        Action action = () => testObject.GetPropertyValue(propertyInfo!);

        // Assert
        action.Should().ThrowExactly<PropertyIndexerMismatchException>().Which.PropertyName.Should().Be("Item");
    }

    [Fact]
    public void GetPropertyValue_for_PropertyInfo_should_throw_PropertyWriteOnlyException_for_write_only_property()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithWriteOnlyProperty();
        var propertyInfo = typeof(TestTypes.ClassWithWriteOnlyProperty).GetProperty(nameof(TestTypes.ClassWithWriteOnlyProperty.WriteOnlyProperty))!;

        // Act
        Action action = () => testObject.GetPropertyValue(propertyInfo);

        // Assert
        action.Should()
              .ThrowExactly<PropertyWriteOnlyException>()
              .Which.PropertyName.Should()
              .Be(nameof(TestTypes.ClassWithWriteOnlyProperty.WriteOnlyProperty));
    }

    [Theory]
    [AutoMockData]
    public void GetPropertyValue_should_handle_properties_from_base_classes(TestTypes.TestType2 derivedClass)
    {
        // Act
        var result = derivedClass.GetPropertyValue(x => x.BaseProperty);

        // Assert
        result.Should().Be(derivedClass.BaseProperty);
    }

    [Theory]
    [AutoMockData]
    public void GetPropertyValue_should_handle_properties_from_structs(int testValue)
    {
        // Arrange
        var testStruct = new TestTypes.TestStruct(testValue, new TestTypes.TestStruct2 { IntProperty = testValue, StringProperty = Guid.NewGuid().ToString() });

        // Act
        var result = testStruct.GetPropertyValue(x => x.StructProperty);

        // Assert
        result.Should().Be(testValue);
    }

    [Fact]
    public void GetPropertyValue_should_handle_properties_with_internal_setters()
    {
        // Arrange
        var testObject = new TestTypes.ClassWithInternalSetter();

        // Act
        var result = testObject.GetPropertyValue(x => x.PropertyWithInternalSetter);

        // Assert
        result.Should().Be(TestTypes.ClassWithInternalSetter.DefaultValue);
    }

    [Theory]
    [AutoData]
    public void GetPropertyValue_should_retrieve_property_value_and_cast_it_to_requested_type(int intProp,
                                                                                              int? nullableInt,
                                                                                              string stringProp,
                                                                                              DateTimeOffset dateTimeOffset,
                                                                                              DateTimeOffset? nullableDateTimeOffset)
    {
        var testObject = new TestTypes.MyTestClass
                         { IntProp = intProp,
                           StringProp = stringProp,
                           DateTimeOffsetProp = dateTimeOffset,
                           NullableIntProp = nullableInt,
                           NullableDateTimeOffsetProp = nullableDateTimeOffset };
        testObject.GetPropertyValue<TestTypes.MyTestClass, int>(nameof(TestTypes.MyTestClass.IntProp)).Should().Be(intProp);
        testObject.GetPropertyValue<TestTypes.MyTestClass, int?>(nameof(TestTypes.MyTestClass.NullableIntProp)).Should().Be(nullableInt);
        testObject.GetPropertyValue<TestTypes.MyTestClass, string>(nameof(TestTypes.MyTestClass.StringProp)).Should().Be(stringProp);
        testObject.GetPropertyValue<TestTypes.MyTestClass, DateTimeOffset>(nameof(TestTypes.MyTestClass.DateTimeOffsetProp)).Should().Be(dateTimeOffset);
        testObject.GetPropertyValue<TestTypes.MyTestClass, DateTimeOffset?>(nameof(TestTypes.MyTestClass.NullableDateTimeOffsetProp))
                  .Should()
                  .Be(nullableDateTimeOffset);
    }

    [Fact]
    public void GetPropertyValue_should_throw_ArgumentNullException_when_propertyExpression_is_null()
    {
        // Arrange
        var testObject = new TestClass();
        Expression<Func<TestClass, string>>? nullExpression = null;

        // Act
        Action action = () => testObject.GetPropertyValue(nullExpression!);

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>().WithParameterName("propertySelector");
    }

    [Theory]
    [AutoData]
    public void GetPropertyValueTest(int intProp, string stringProp)
    {
        var testObject = new TestTypes.MyTestClass { IntProp = intProp, StringProp = stringProp };
        testObject.GetPropertyValue(nameof(TestTypes.MyTestClass.IntProp)).Should().Be(intProp);
        testObject.GetPropertyValue(nameof(TestTypes.MyTestClass.StringProp)).Should().Be(stringProp);
    }

    private class TestClass
    {
        public string Name { get; set; } = "TestName";

        public int Age { get; set; } = 30;

        public string this[int index] => $"IndexedValue{index}";
    }
}
