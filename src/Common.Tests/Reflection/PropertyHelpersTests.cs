﻿using AutoFixture.Xunit2;
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
}