using Ploch.Common.Linq;

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

    [Theory]
    [AutoMockData]
    public void GetValue_for_indexer_should_return_correct_property_value(TestClass testClass)
    {
        // Arrange
        var type = testClass.GetType();
        var propertyInfo = type.GetProperty("Item");
        var ownedPropertyInfo = new OwnedPropertyInfo<TestClass, string>(propertyInfo, testClass);
        OwnedPropertyInfo baseOwnedPropertyInfo = ownedPropertyInfo;

        // Act
        var val0 = baseOwnedPropertyInfo.GetValue([0]);
        var val1 = ownedPropertyInfo.GetValue([1]);

        // Assert
        val0.Should().Be(testClass.Strings[0]);
        val1.Should().Be(testClass.Strings[1]);
    }

    [Theory]
    [AutoMockData]
    public void SetValue_for_indexer_should_set_correct_property_value(TestClass testClass)
    {
        // Arrange
        var type = testClass.GetType();
        var propertyInfo = type.GetProperty("Item");
        var ownedPropertyInfo = new OwnedPropertyInfo<TestClass, string>(propertyInfo, testClass);
        OwnedPropertyInfo baseOwnedPropertyInfo = ownedPropertyInfo;

        // Act
        baseOwnedPropertyInfo.SetValue("val0", [0]);
        ownedPropertyInfo.SetValue("val1", [1]);

        // Assert
        testClass.Strings[0].Should().Be("val0");
        testClass.Strings[1].Should().Be("val1");
    }

    [Fact]
    public void OwnedProperty_methods_properties_should_be_redirected_to_owner()
    {
        // Arrange
        var testClass = new TestClass();
        var propertyInfo = typeof(TestClass).GetProperty(nameof(TestClass.TestProperty));
        var ownedPropertyInfo = new OwnedPropertyInfo<TestClass, int>(propertyInfo, testClass);

        ownedPropertyInfo.PropertyInfo.Should().BeSameAs(propertyInfo);
        ownedPropertyInfo.Name.Should().BeSameAs(propertyInfo.Name);
    }

    public class MyTestAttribAttribute : Attribute
    { }

    public class TestClass
    {
        public TestClass(params string[] strings) => Strings = strings;

        public string this[int index]
        {
            get => Strings[index];
            set => Strings[index] = value;
        }

        public string[] Strings { get; set; }

        [MyTestAttrib]
        public int TestProperty { get; set; }
    }
}
