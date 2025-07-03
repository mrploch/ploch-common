using System.Reflection;
using AutoFixture.Xunit2;
using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class PropertyHelpersTests
{
    [Fact]
    public void GetProperties_should_throw_if_obj_is_null()
    {
        TestTypes.MyTestClass? testObject = null;
        var exceptionWasThrown = false;
        try
        {
            testObject.GetProperties<string>();
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
        var testObject = new TestTypes.MyTestClass();
        var propertyInfos = testObject.GetProperties<TestTypes.TestTypeBase>(false);

        propertyInfos.Should()
                     .HaveCount(1)
                     .And.Contain(static pi => pi.PropertyType == typeof(TestTypes.TestTypeBase) && pi.Name == nameof(TestTypes.MyTestClass.TestTypeBaseProp));
    }

    [Fact]
    public void GetPropertiesOfTypeIncludingSubclassTest()
    {
        var testObject = new TestTypes.MyTestClass();
        var propertyInfos = testObject.GetProperties<TestTypes.TestTypeBase>();

        propertyInfos.Should()
                     .HaveCount(2)
                     .And.Contain(static pi => pi.PropertyType == typeof(TestTypes.TestTypeBase) && pi.Name == nameof(TestTypes.MyTestClass.TestTypeBaseProp))
                     .And.Contain(pi => pi.PropertyType == typeof(TestTypes.TestType2) && pi.Name == nameof(TestTypes.MyTestClass.TestType2Prop));
    }

    [Fact]
    public void GetPropertiesOfTypeTest()
    {
        var testObject = new TestTypes.MyTestClass();
        var propertyInfos = testObject.GetProperties<string>();

        propertyInfos.Should()
                     .HaveCount(2)
                     .And.Contain(pi => pi.PropertyType == typeof(string) && pi.Name == nameof(TestTypes.MyTestClass.StringProp))
                     .And.Contain(pi => pi.PropertyType == typeof(string) && pi.Name == nameof(TestTypes.MyTestClass.StringProp2));
    }

    [Fact]
    public void GetPropertyInfoThrowsIfNotFound()
    {
        var propertyName = "NonExistent";

        Action action = () => typeof(TestTypes.MyTestClass).GetPropertyInfo(propertyName, true);
        action.Should().ThrowExactly<PropertyNotFoundException>().Which.PropertyName.Should().Be(propertyName);
    }

    [Fact]
    public void GetStaticPropertyValue_should_fail_if_property_not_found()
    {
        var action = () => typeof(TestTypes.TestClassWithStaticProperties).GetStaticPropertyValue("NonExistingProperty");

        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void GetStaticPropertyValue_should_return_static_property_value_if_property_exists()
    {
        TestTypes.TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();

        var value = typeof(TestTypes.TestClassWithStaticProperties).GetStaticPropertyValue(nameof(TestTypes.TestClassWithStaticProperties.StaticStringProp));

        value.Should().Be(TestTypes.TestClassWithStaticProperties.StaticStringProp);
    }

    [Fact]
    public void GetStaticPropertyValue_with_expected_type_should_handle_null_value()
    {
        TestTypes.TestClassWithStaticProperties.StaticStringProp = null;
        TestTypes.TestClassWithStaticProperties.StaticNullableIntProp = null;

        var stringPropValue =
            typeof(TestTypes.TestClassWithStaticProperties).GetStaticPropertyValue<string>(nameof(TestTypes.TestClassWithStaticProperties.StaticStringProp));
        var nullableIntPropValue =
            typeof(TestTypes.TestClassWithStaticProperties).GetStaticPropertyValue<int?>(nameof(TestTypes.TestClassWithStaticProperties.StaticNullableIntProp));

        stringPropValue.Should().BeNull();
        nullableIntPropValue.Should().BeNull();
    }

    [Fact]
    public void GetStaticPropertyValue_with_expected_type_should_return_property_value_as_expected_type()
    {
        TestTypes.TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();
        TestTypes.TestClassWithStaticProperties.StaticIntProp = 1234;

        var stringPropValue =
            typeof(TestTypes.TestClassWithStaticProperties).GetStaticPropertyValue<string>(nameof(TestTypes.TestClassWithStaticProperties.StaticStringProp));
        var intPropValue =
            typeof(TestTypes.TestClassWithStaticProperties).GetStaticPropertyValue<int>(nameof(TestTypes.TestClassWithStaticProperties.StaticIntProp));

        stringPropValue.Should().Be(TestTypes.TestClassWithStaticProperties.StaticStringProp);
        intPropValue.Should().Be(TestTypes.TestClassWithStaticProperties.StaticIntProp);
    }

    [Fact]
    public void GetStaticPropertyValue_with_unexpected_type_should_throw()
    {
        TestTypes.TestClassWithStaticProperties.StaticStringProp = "test";

        var type = typeof(TestTypes.TestClassWithStaticProperties);
        var action = () => type.GetStaticPropertyValue<Guid>(nameof(TestTypes.TestClassWithStaticProperties.StaticStringProp));
        action.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void HasPropertyTest()
    {
        var testObject = new TestTypes.MyTestClass();
        testObject.HasProperty(nameof(TestTypes.MyTestClass.IntProp)).Should().BeTrue();
        testObject.HasProperty("NoPropertyLikeThis").Should().BeFalse();
    }

    [Theory]
    [AutoData]
    public void SetPropertyValueTest(int intProp, string stringProp)
    {
        var testObject = new TestTypes.MyTestClass();
        testObject.SetPropertyValue("IntProp", intProp);
        testObject.SetPropertyValue("StringProp", stringProp);

        testObject.IntProp.Should().Be(intProp);
        testObject.StringProp.Should().Be(stringProp);
    }

    [Fact]
    public void TryGetStaticPropertyValue_should_return_false_if_property_not_found()
    {
        var success = typeof(TestTypes.TestClassWithStaticProperties).TryGetStaticPropertyValue("NonExistingProperty", out var value);

        success.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetStaticPropertyValue_should_return_true_and_static_property_value_if_property_exists()
    {
        TestTypes.TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();

        var success =
            typeof(TestTypes.TestClassWithStaticProperties).TryGetStaticPropertyValue(nameof(TestTypes.TestClassWithStaticProperties.StaticStringProp),
                                                                                      out var value);

        success.Should().BeTrue();
        value.Should().Be(TestTypes.TestClassWithStaticProperties.StaticStringProp);
    }

    [Fact]
    public void IsStatic_should_return_true_for_static_property()
    {
        // Arrange
        var propertyInfo = typeof(TestTypes.TestClassWithStaticProperties).GetProperty(nameof(TestTypes.TestClassWithStaticProperties.StaticStringProp));

        // Act
        var isStatic = propertyInfo.IsStatic();

        // Assert
        isStatic.Should().BeTrue();
    }

    [Fact]
    public void IsStatic_should_return_false_for_instance_property()
    {
        // Arrange
        var propertyInfo = typeof(TestTypes.MyTestClass).GetProperty(nameof(TestTypes.MyTestClass.StringProp));

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
            typeof(TestTypes.TestClassWithStaticFieldsAndProperties).GetProperty(TestTypes.TestClassWithStaticFieldsAndProperties.PrivateStaticPropName,
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
            typeof(TestTypes.TestClassWithStaticFieldsAndProperties).GetProperty(TestTypes.TestClassWithStaticFieldsAndProperties.ProtectedStaticPropName,
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
}
