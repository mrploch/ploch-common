using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using Ploch.Common.Reflection;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

public class PropertyHelpersTests
{
    [Fact]
    public void GetProperties_should_throw_if_obj_is_null()
    {
        MyTestClass? testObject = null;
        var exceptionWasThrown = false;
        try
        {
#pragma warning disable CS8604 // Possible null reference argument.
            testObject.GetProperties<string>();
#pragma warning restore CS8604 // Possible null reference argument.
        }
        catch (ArgumentNullException ex)
        {
            exceptionWasThrown = true;

            ex.ParamName.Should().Be("obj");
        }

        exceptionWasThrown.Should().BeTrue();
    }

    [Fact]
    public void GetPropertiesOfTypeExcludingSubclassTest()
    {
        var testObject = new MyTestClass();
        var propertyInfos = testObject.GetProperties<TestTypeBase>(false);

        propertyInfos.Should()
                     .HaveCount(1)
                     .And.Contain(static pi => pi.PropertyType == typeof(TestTypeBase) && pi.Name == nameof(MyTestClass.TestTypeBaseProp));
    }

    [Fact]
    public void GetPropertiesOfTypeIncludingSubclassTest()
    {
        var testObject = new MyTestClass();
        var propertyInfos = testObject.GetProperties<TestTypeBase>();

        propertyInfos.Should()
                     .HaveCount(2)
                     .And.Contain(static pi => pi.PropertyType == typeof(TestTypeBase) && pi.Name == nameof(MyTestClass.TestTypeBaseProp))
                     .And.Contain(pi => pi.PropertyType == typeof(TestType2) && pi.Name == nameof(MyTestClass.TestType2Prop));
    }

    [Fact]
    public void GetPropertiesOfTypeTest()
    {
        var testObject = new MyTestClass();
        var propertyInfos = testObject.GetProperties<string>();

        propertyInfos.Should()
                     .HaveCount(2)
                     .And.Contain(pi => pi.PropertyType == typeof(string) && pi.Name == nameof(MyTestClass.StringProp))
                     .And.Contain(pi => pi.PropertyType == typeof(string) && pi.Name == nameof(MyTestClass.StringProp2));
    }

    [Fact]
    public void GetPropertyInfoThrowsIfNotFound()
    {
        var propertyName = "NonExistent";

        Action action = () => typeof(MyTestClass).GetPropertyInfo(propertyName, true);
        action.Should().ThrowExactly<PropertyNotFoundException>().Which.PropertyName.Should().Be(propertyName);
    }

    [Fact]
    public void GetStaticPropertyValue_should_fail_if_property_not_found()
    {
        var action = () => typeof(TestClassWithStaticProperties).GetStaticPropertyValue("NonExistingProperty");

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void GetStaticPropertyValue_should_return_static_property_value_if_property_exists()
    {
        TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();

        var value = typeof(TestClassWithStaticProperties).GetStaticPropertyValue(nameof(TestClassWithStaticProperties.StaticStringProp));

        value.Should().Be(TestClassWithStaticProperties.StaticStringProp);
    }

    [Fact]
    public void GetStaticPropertyValue_with_expected_type_should_handle_null_value()
    {
        TestClassWithStaticProperties.StaticStringProp = null;
        TestClassWithStaticProperties.StaticNullableIntProp = null;

        var stringPropValue = typeof(TestClassWithStaticProperties).GetStaticPropertyValue<string>(nameof(TestClassWithStaticProperties.StaticStringProp));
        var nullableIntPropValue =
            typeof(TestClassWithStaticProperties).GetStaticPropertyValue<int?>(nameof(TestClassWithStaticProperties.StaticNullableIntProp));

        stringPropValue.Should().BeNull();
        nullableIntPropValue.Should().BeNull();
    }

    [Fact]
    public void GetStaticPropertyValue_with_expected_type_should_return_property_value_as_expected_type()
    {
        TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();
        TestClassWithStaticProperties.StaticIntProp = 1234;

        var stringPropValue = typeof(TestClassWithStaticProperties).GetStaticPropertyValue<string>(nameof(TestClassWithStaticProperties.StaticStringProp));
        var intPropValue = typeof(TestClassWithStaticProperties).GetStaticPropertyValue<int>(nameof(TestClassWithStaticProperties.StaticIntProp));

        stringPropValue.Should().Be(TestClassWithStaticProperties.StaticStringProp);
        intPropValue.Should().Be(TestClassWithStaticProperties.StaticIntProp);
    }

    [Fact]
    public void GetStaticPropertyValue_with_unexpected_type_should_throw()
    {
        TestClassWithStaticProperties.StaticStringProp = "test";

        var type = typeof(TestClassWithStaticProperties);
        var action = () => type.GetStaticPropertyValue<Guid>(nameof(TestClassWithStaticProperties.StaticStringProp));
        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void HasPropertyTest()
    {
        var testObject = new MyTestClass();
        testObject.HasProperty(nameof(MyTestClass.IntProp)).Should().BeTrue();
        testObject.HasProperty("NoPropertyLikeThis").Should().BeFalse();
    }

    [Theory]
    [AutoData]
    public void SetPropertyValueTest(int intProp, string stringProp)
    {
        var testObject = new MyTestClass();
        testObject.SetPropertyValue("IntProp", intProp);
        testObject.SetPropertyValue("StringProp", stringProp);

        testObject.IntProp.Should().Be(intProp);
        testObject.StringProp.Should().Be(stringProp);
    }

    [Fact]
    public void TryGetStaticPropertyValue_should_return_false_if_property_not_found()
    {
        var success = typeof(TestClassWithStaticProperties).TryGetStaticPropertyValue("NonExistingProperty", out var value);

        success.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetStaticPropertyValue_should_return_true_and_static_property_value_if_property_exists()
    {
        TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();

        var success = typeof(TestClassWithStaticProperties).TryGetStaticPropertyValue(nameof(TestClassWithStaticProperties.StaticStringProp), out var value);

        success.Should().BeTrue();
        value.Should().Be(TestClassWithStaticProperties.StaticStringProp);
    }

    [Fact]
    public void IsStatic_should_return_true_for_static_property()
    {
        // Arrange
        var propertyInfo = typeof(TestClassWithStaticProperties).GetProperty(nameof(TestClassWithStaticProperties.StaticStringProp));

        // Act
        var isStatic = propertyInfo.IsStatic();

        // Assert
        isStatic.Should().BeTrue();
    }

    [Fact]
    public void IsStatic_should_return_false_for_instance_property()
    {
        // Arrange
        var propertyInfo = typeof(MyTestClass).GetProperty(nameof(MyTestClass.StringProp));

        // Act
        var isStatic = propertyInfo.IsStatic();

        // Assert
        isStatic.Should().BeFalse();
    }

    [Fact]
    public void IsStatic_should_work_with_private_static_properties()
    {
        // Arrange
        var propertyInfo =
            typeof(TestClassWithStaticFieldsAndProperties).GetProperty(TestClassWithStaticFieldsAndProperties.PrivateStaticPropName,
                                                                       BindingFlags.Static | BindingFlags.NonPublic);

        // Act
        var isStatic = propertyInfo.IsStatic();

        // Assert
        isStatic.Should().BeTrue();
    }

    [Fact]
    public void IsStatic_should_work_with_protected_static_properties()
    {
        // Arrange
        var propertyInfo =
            typeof(TestClassWithStaticFieldsAndProperties).GetProperty(TestClassWithStaticFieldsAndProperties.ProtectedStaticPropName,
                                                                       BindingFlags.Static | BindingFlags.NonPublic);

        // Act
        var isStatic = propertyInfo.IsStatic();

        // Assert
        isStatic.Should().BeTrue();
    }

    [Fact]
    public void IsStatic_should_throw_when_propertyInfo_is_null()
    {
        // Arrange
        PropertyInfo? propertyInfo = null;

        // Act
        Action action = () => propertyInfo.IsStatic();

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void GetPropertyValues_should_throw_when_obj_is_null()
    {
        // Arrange
        object? testObject = null;

        // Act
        Action action = () => testObject.GetPropertyValues();

        // Assert
        action.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("obj");
    }

    [Fact]
    public void GetPropertyValues_should_return_empty_collection_for_object_with_no_properties()
    {
        // Arrange
        var objectWithNoProperties = new EmptyClass();

        // Act
        var result = objectWithNoProperties.GetPropertyValues();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetPropertyValues_should_correctly_return_property_names_and_values_for_simple_object()
    {
        // Arrange
        var testObject = new SimplePropertiesTestClass { IntProp = 42, StringProp = "Test Value", StringProp2 = "Another Value" };

        // Act
        var result = testObject.GetPropertyValues().ToList();

        // Assert
        result.Should().Contain(item => item.Item1 == nameof(SimplePropertiesTestClass.IntProp) && (int)item.Item2! == 42);
        result.Should().Contain(item => item.Item1 == nameof(SimplePropertiesTestClass.StringProp) && (string)item.Item2! == "Test Value");
        result.Should().Contain(item => item.Item1 == nameof(SimplePropertiesTestClass.StringProp2) && (string)item.Item2! == "Another Value");

        // The other properties should be there too, but we're just checking the ones we set explicitly
    }

    [Fact]
    public void GetPropertyValues_should_handle_properties_with_null_values_correctly()
    {
        // Arrange
        var testObject = new TestClassWithNullableProperties { StringProp = "Not Null", StringProp2 = null, NullableIntProp = 21, NullableIntProp2 = null };

        // Act
        var result = testObject.GetPropertyValues().ToList();

        // Assert
        result.Should().Contain(item => item.Item1 == nameof(ClassWithNullableProperties.NonNullProperty) && (string)item.Item2! == "Not Null");
        result.Should().Contain(item => item.Item1 == nameof(ClassWithNullableProperties.NullableProperty) && item.Item2 == null);
    }

    [Fact]
    public void GetPropertyValues_should_only_include_public_properties()
    {
        // Arrange
        var testObject = new ClassWithPrivateProperties { PublicProperty = "Public Value" };
        testObject.SetPrivatePropertyValue("Private Value");

        // Act
        var result = testObject.GetPropertyValues().ToList();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(item => item.Item1 == nameof(ClassWithPrivateProperties.PublicProperty) && (string)item.Item2! == "Public Value");
        result.Should().NotContain(item => item.Item1 == "PrivateProperty");
    }

    [Fact]
    public void GetPropertyValues_should_include_inherited_properties()
    {
        // Arrange
        var derivedObject = new DerivedClass { BaseProperty = "Base Value", DerivedProperty = "Derived Value" };

        // Act
        var result = derivedObject.GetPropertyValues().ToList();

        // Assert
        result.Should().Contain(item => item.Item1 == nameof(DerivedClass.BaseProperty) && (string)item.Item2! == "Base Value");
        result.Should().Contain(item => item.Item1 == nameof(DerivedClass.DerivedProperty) && (string)item.Item2! == "Derived Value");
        result.Count(item => string.Equals(item.Item1, nameof(DerivedClass.BaseProperty), StringComparison.Ordinal) ||
                             string.Equals(item.Item1, nameof(DerivedClass.DerivedProperty), StringComparison.Ordinal))
              .Should()
              .Be(2);
    }

    [Fact]
    public void GetPropertyValues_should_handle_objects_with_complex_property_types()
    {
        // Arrange
        var complexObject = new ClassWithComplexProperties
                                {
                                    Id = 123,
                                    Name = "Complex Object",
                                    NestedObject = new() { Value = "Nested Value" },
                                    ListProperty = new() { "Item1", "Item2", "Item3" }
                                };

        // Act
        var result = complexObject.GetPropertyValues().ToList();

        // Assert
        result.Should().Contain(item => item.Item1 == nameof(ClassWithComplexProperties.Id) && (int)item.Item2! == 123);
        result.Should().Contain(item => item.Item1 == nameof(ClassWithComplexProperties.Name) && (string)item.Item2! == "Complex Object");

        // Complex property assertions
        var nestedObjectTuple = result.FirstOrDefault(item => item.Item1 == nameof(ClassWithComplexProperties.NestedObject));
        nestedObjectTuple.Item2.Should().NotBeNull();
        nestedObjectTuple.Item2.Should().BeOfType<NestedClass>();
        ((NestedClass)nestedObjectTuple.Item2!).Value.Should().Be("Nested Value");

        var listPropertyTuple = result.FirstOrDefault(item => item.Item1 == nameof(ClassWithComplexProperties.ListProperty));
        listPropertyTuple.Item2.Should().NotBeNull();
        listPropertyTuple.Item2.Should().BeOfType<List<string>>();
        ((List<string>)listPropertyTuple.Item2!).Should().HaveCount(3);
        ((List<string>)listPropertyTuple.Item2!).Should().ContainInOrder("Item1", "Item2", "Item3");
    }

    [Fact]
    public void GetPropertyValues_should_correctly_handle_indexed_properties()
    {
        // Arrange
        var testObject = new ClassWithIndexer();
        testObject[0] = "First";
        testObject[1] = "Second";
        testObject[2] = "Third";

        // Act
        var result = testObject.GetPropertyValues().ToList();

        // Assert
        var indexerProperty = result.FirstOrDefault(item => item.Item1 == PropertyHelpers.IndexerPropertyName);
        indexerProperty.Item1.Should().Be(PropertyHelpers.IndexerPropertyName);

        // The indexer property should be present but its value behavior is implementation-dependent
        // We just verify it exists since accessing the value directly would require an index
        indexerProperty.Should().NotBeNull();

        // Also verify that we have other properties
        result.Should().Contain(item => item.Item1 == nameof(ClassWithIndexer.Count));
        result.First(item => item.Item1 == nameof(ClassWithIndexer.Count)).Item2.Should().Be(3);
    }

    [Fact]
    public void GetPropertyValues_should_work_with_anonymous_types()
    {
        // Arrange
        var anonymousObject = new { Id = 42, Name = "Anonymous", IsActive = true, CreatedDate = DateTime.Now };

        // Act
        var result = anonymousObject.GetPropertyValues().ToList();

        // Assert
        result.Should().HaveCount(4);
        result.Should().Contain(item => item.Item1 == "Id" && (int)item.Item2! == 42);
        result.Should().Contain(item => item.Item1 == "Name" && (string)item.Item2! == "Anonymous");
        result.Should().Contain(item => item.Item1 == "IsActive" && (bool)item.Item2! == true);
        result.Should().Contain(item => item.Item1 == "CreatedDate" && item.Item2 is DateTime);
    }

    private class BaseClass
    {
        public string BaseProperty { get; set; } = string.Empty;
    }

    private class DerivedClass : BaseClass
    {
        public string DerivedProperty { get; set; } = string.Empty;
    }

    public class ClassWithNullableProperties
    {
        public string NonNullProperty { get; set; } = string.Empty;

        public string? NullableProperty { get; set; }
    }

    private class ClassWithPrivateProperties
    {
        public string PublicProperty { get; set; } = string.Empty;

        private string PrivateProperty { get; set; } = string.Empty;

        public void SetPrivatePropertyValue(string value) => PrivateProperty = value;
    }

    private class NestedClass
    {
        public string Value { get; set; } = string.Empty;
    }

    private class ClassWithComplexProperties
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public NestedClass NestedObject { get; set; } = new();

        public List<string> ListProperty { get; set; } = new();
    }

    private class ClassWithIndexer
    {
        private readonly Dictionary<int, string> _items = new();

        public string this[int index]
        {
            get => _items.TryGetValue(index, out var value) ? value : string.Empty;
            set => _items[index] = value;
        }

        public int Count => _items.Count;
    }
}
