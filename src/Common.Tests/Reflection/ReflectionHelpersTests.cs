using System;
using System.Diagnostics.CodeAnalysis;
using AutoFixture.Xunit2;
using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

// ReSharper disable ExceptionNotDocumented
// ReSharper disable ExceptionNotDocumentedOptional
// ReSharper disable ExceptionNotDocumentedOptional

namespace Ploch.Common.Tests.Reflection
{
    public class PropertyHelpersTests
    {
        [Theory]
        [AutoData]
        public void GetPropertyValueTest(int intProp, string stringProp)
        {
            var testObject = new TestTypes.MyTestClass {IntProp = intProp, StringProp = stringProp};
            PropertyHelpers.GetPropertyValue(testObject, nameof(TestTypes.MyTestClass.IntProp)).Should().Be(intProp);
            PropertyHelpers.GetPropertyValue(testObject, nameof(TestTypes.MyTestClass.StringProp)).Should().Be(stringProp);
        }

        [Theory]
        [AutoData]
        public void SetPropertyValueTest(int intProp, string stringProp)
        {
            var testObject = new TestTypes.MyTestClass();
            PropertyHelpers.SetPropertyValue(testObject, "IntProp", intProp);
            PropertyHelpers.SetPropertyValue(testObject, "StringProp", stringProp);

            testObject.IntProp.Should().Be(intProp);
            testObject.StringProp.Should().Be(stringProp);
        }

        [Fact]
        public void GetPropertiesOfTypeExcludingSubclassTest()
        {
            var testObject = new TestTypes.MyTestClass();
            var propertyInfos = PropertyHelpers.GetProperties<TestTypes.TestTypeBase>(testObject, false);

            propertyInfos.Should()
                         .HaveCount(1)
                         .And.Contain(pi => pi.PropertyType == typeof(TestTypes.TestTypeBase) && pi.Name == nameof(TestTypes.MyTestClass.TestTypeBaseProp));
        }

        [Fact]
        public void GetPropertiesOfTypeIncludingSubclassTest()
        {
            var testObject = new TestTypes.MyTestClass();
            var propertyInfos = PropertyHelpers.GetProperties<TestTypes.TestTypeBase>(testObject);

            propertyInfos.Should()
                         .HaveCount(2)
                         .And.Contain(pi => pi.PropertyType == typeof(TestTypes.TestTypeBase) && pi.Name == nameof(TestTypes.MyTestClass.TestTypeBaseProp))
                         .And.Contain(pi => pi.PropertyType == typeof(TestTypes.TestType2) && pi.Name == nameof(TestTypes.MyTestClass.TestType2Prop));
        }

        [Fact]
        public void GetPropertiesOfTypeTest()
        {
            var testObject = new TestTypes.MyTestClass();
            var propertyInfos = PropertyHelpers.GetProperties<string>(testObject);

            propertyInfos.Should()
                         .HaveCount(2)
                         .And.Contain(pi => pi.PropertyType == typeof(string) && pi.Name == nameof(TestTypes.MyTestClass.StringProp))
                         .And.Contain(pi => pi.PropertyType == typeof(string) && pi.Name == nameof(TestTypes.MyTestClass.StringProp2));
        }

        [Fact]
        [SuppressMessage("ReSharper", "EventExceptionNotDocumented")]
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
            PropertyHelpers.HasProperty(testObject, nameof(TestTypes.MyTestClass.IntProp)).Should().BeTrue();
            PropertyHelpers.HasProperty(testObject, "NoPropertyLikeThis").Should().BeFalse();
        }
    }
}