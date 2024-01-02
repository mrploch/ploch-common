using FluentAssertions;
using Ploch.Common.Linq;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class OwnedPropertyInfoTests
{
    [Fact]
    public void GetValue_ShouldReturnCorrectValue()
    {
        // Arrange
        var testClass = new TestClass { TestProperty = 5 };
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty));
        var ownedPropertyInfo = new OwnedPropertyInfo<TestClass, int>(propertyInfo, testClass);

        // Act
        var value = ownedPropertyInfo.GetValue();

        // Assert
        value.Should().Be(5);
    }

    [Fact]
    public void SetValue_ShouldSetCorrectValue()
    {
        // Arrange
        var testClass = new TestClass();
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty));
        var ownedPropertyInfo = new OwnedPropertyInfo<TestClass, int>(propertyInfo, testClass);

        // Act
        ownedPropertyInfo.SetValue(10);
        var value = testClass.TestProperty;

        // Assert
        value.Should().Be(10);
    }

    [Fact]
    public void OwnedProperty_methods_properties_should_be_redirected_to_owner()
    {
        // Arrange
        var testClass = new TestClass();
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty));
        var ownedPropertyInfo = new OwnedPropertyInfo<TestClass, int>(propertyInfo, testClass);

        ownedPropertyInfo.PropertyInfo.Should().BeSameAs(propertyInfo);
        ownedPropertyInfo.DeclaringType.Should().BeSameAs(propertyInfo.DeclaringType);
        ownedPropertyInfo.Name.Should().BeSameAs(propertyInfo.Name);
        ownedPropertyInfo.ReflectedType.Should().BeSameAs(propertyInfo.ReflectedType);
        ownedPropertyInfo.Attributes.Should().Be(propertyInfo.Attributes);
        ownedPropertyInfo.CanRead.Should().Be(propertyInfo.CanRead);
        ownedPropertyInfo.CanWrite.Should().Be(propertyInfo.CanWrite);
        ownedPropertyInfo.PropertyType.Should().BeSameAs(propertyInfo.PropertyType);
        ownedPropertyInfo.GetAccessors(true).Should().BeEquivalentTo(propertyInfo.GetAccessors(true));
        ownedPropertyInfo.GetGetMethod(true).Should().BeSameAs(propertyInfo.GetGetMethod(true));
        ownedPropertyInfo.GetSetMethod(true).Should().BeSameAs(propertyInfo.GetSetMethod(true));
        ownedPropertyInfo.GetIndexParameters().Should().BeSameAs(propertyInfo.GetIndexParameters());
        ownedPropertyInfo.GetCustomAttributes(true).Should().BeEquivalentTo(propertyInfo.GetCustomAttributes(true));
        ownedPropertyInfo.GetCustomAttributes(typeof(MyTestAttribAttribute), true)
                         .Should()
                         .BeEquivalentTo(propertyInfo.GetCustomAttributes(typeof(MyTestAttribAttribute), true));
        ownedPropertyInfo.IsDefined(typeof(MyTestAttribAttribute), true).Should().Be(propertyInfo.IsDefined(typeof(MyTestAttribAttribute), true));
    }

    public class MyTestAttribAttribute : Attribute
    { }

    private class TestClass
    {
        [MyTestAttrib]
        public int TestProperty { get; set; }
    }
}