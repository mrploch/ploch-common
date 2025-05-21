using AutoFixture.Xunit2;
using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class PropertyHelpersTests
{
    [Theory]
    [AutoData]
    public void GetPropertyValueTest(int intProp, string stringProp)
    {
        var testObject = new TestTypes.MyTestClass { IntProp = intProp, StringProp = stringProp };
        testObject.GetPropertyValue(nameof(TestTypes.MyTestClass.IntProp)).Should().Be(intProp);
        testObject.GetPropertyValue(nameof(TestTypes.MyTestClass.StringProp)).Should().Be(stringProp);
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
    public void HasPropertyTest()
    {
        var testObject = new TestTypes.MyTestClass();
        testObject.HasProperty(nameof(TestTypes.MyTestClass.IntProp)).Should().BeTrue();
        testObject.HasProperty("NoPropertyLikeThis").Should().BeFalse();
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
    public void TryGetStaticPropertyValue_should_return_false_if_property_not_found()
    {
        var success = typeof(TestClassWithStaticProperties).TryGetStaticPropertyValue("NonExistingProperty", out var value);

        success.Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void GetStaticPropertyValue_should_return_static_property_value_if_property_exists()
    {
        TestClassWithStaticProperties.StaticStringProp = Guid.NewGuid().ToString();

        var value = typeof(TestClassWithStaticProperties).GetStaticPropertyValue(nameof(TestClassWithStaticProperties.StaticStringProp));

        value.Should().Be(TestClassWithStaticProperties.StaticStringProp);
    }

    [Fact]
    public void GetStaticPropertyValue_should_fail_if_property_not_found()
    {
        var action = () => typeof(TestClassWithStaticProperties).GetStaticPropertyValue("NonExistingProperty");

        action.Should().ThrowExactly<InvalidOperationException>();
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
    public void GetStaticPropertyValue_with_expected_type_should_handle_null_value()
    {
        TestClassWithStaticProperties.StaticStringProp = null;
        TestClassWithStaticProperties.StaticNullableIntProp = null;

        var stringPropValue = typeof(TestClassWithStaticProperties).GetStaticPropertyValue<string>(nameof(TestClassWithStaticProperties.StaticStringProp));
        var nullableIntPropValue = typeof(TestClassWithStaticProperties).GetStaticPropertyValue<string>(nameof(TestClassWithStaticProperties.StaticStringProp));

        stringPropValue.Should().BeNull();
        nullableIntPropValue.Should().BeNull();
    }

    private static class TestClassWithStaticProperties
    {
        public static string? StaticStringProp { get; set; }

        public static int StaticIntProp { get; set; }

        public static int? StaticNullableIntProp { get; set; }
    }
}