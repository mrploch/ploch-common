using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Linq;
using Ploch.Common.Tests.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Linq;

public class OwnedPropertyInfoTests
{
    [Theory]
    [AutoMockData]
    public void GetValue_should_return_property_value_for_source_object(TestTypes.MyTestClass obj)
    {
        var property = obj.GetProperty(o => o.StringProp2);

        var value = property.GetValue();
        value.Should().Be(obj.StringProp2);
    }

    [Theory]
    [AutoMockData]
    public void SetValue_should_set_property_value_on_source_object(TestTypes.MyTestClass obj)
    {
        var property = obj.GetProperty(o => o.StringProp2);

        property.SetValue("new-value");

        obj.StringProp2.Should().Be("new-value");
    }

    [Theory]
    [AutoMockData]
    public void Name_should_return_property_name(TestTypes.MyTestClass obj)
    {
        var property = obj.GetProperty(o => o.StringProp2);

        property.Name.Should().Be(nameof(TestTypes.MyTestClass.StringProp2));
    }

    [Theory]
    [AutoMockData]
    public void PropertyInfo_should_return_property_info(TestTypes.MyTestClass obj)
    {
        var property = obj.GetProperty(o => o.StringProp2);

        property.PropertyInfo.Should().BeSameAs(typeof(TestTypes.MyTestClass).GetProperty(nameof(TestTypes.MyTestClass.StringProp2)));
    }

    [Theory]
    [AutoMockData]
    public void Owner_should_return_source_object(TestTypes.MyTestClass obj)
    {
        var property = obj.GetProperty(o => o.StringProp2);

        property.Owner.Should().BeSameAs(obj);
    }

    [Theory]
    [AutoMockData]
    public void IOwnedPropertyInfo_GetValue_should_delegate_to_base_GetValue(TestTypes.MyTestClass obj)
    {
        // Arrange
        var property = obj.GetProperty(o => o.StringProp2);
        var ownedPropertyInfo = (IOwnedPropertyInfo)property;

        // Act
        var value = ownedPropertyInfo.GetValue();

        // Assert
        value.Should().Be(obj.StringProp2);
    }

    [Theory]
    [AutoMockData]
    public void IOwnedPropertyInfo_GetValue_should_return_null_when_property_value_is_null(TestTypes.MyTestClass obj)
    {
        // Arrange
        obj.StringProp2 = null;
        var property = obj.GetProperty(o => o.StringProp2);
        var ownedPropertyInfo = (IOwnedPropertyInfo)property;

        // Act
        var value = ownedPropertyInfo.GetValue();

        // Assert
        value.Should().BeNull();
    }

    [Theory]
    [AutoMockData]
    public void IOwnedPropertyInfo_GetValue_should_handle_value_type_properties(TestTypes.MyTestClass obj)
    {
        // Arrange
        var expectedValue = obj.IntProp;
        var property = obj.GetProperty(o => o.IntProp);
        var ownedPropertyInfo = (IOwnedPropertyInfo)property;

        // Act
        var value = ownedPropertyInfo.GetValue();

        // Assert
        value.Should().Be(expectedValue);
        value.Should().BeOfType<int>();
    }

    [Theory]
    [AutoMockData]
    public void IOwnedPropertyInfo_SetValue_should_delegate_to_base_SetValue(TestTypes.MyTestClass obj)
    {
        // Arrange
        var property = obj.GetProperty(o => o.StringProp2);
        var ownedPropertyInfo = (IOwnedPropertyInfo)property;
        var newValue = "delegated-value";

        // Act
        ownedPropertyInfo.SetValue(newValue);

        // Assert
        obj.StringProp2.Should().Be(newValue);
    }

    [Theory]
    [AutoMockData]
    public void IOwnedPropertyInfo_SetValue_should_throw_appropriate_exception_when_setting_incompatible_type(TestTypes.MyTestClass obj)
    {
        // Arrange
        var property = obj.GetProperty(o => o.IntProp);
        var ownedPropertyInfo = (IOwnedPropertyInfo)property;
        var incompatibleValue = "this is a string, not an int";

        // Act & Assert
        var act = () => ownedPropertyInfo.SetValue(incompatibleValue);

        act.Should().Throw<ArgumentException>().WithMessage("*Object*System.String**System.Int32*");
    }

    [Theory] [AutoMockData]
    public void IOwnedPropertyInfo_SetValue_should_set_indexed_property_value_correctly(TestTypes.ClassWithIndexer obj)
    {
        // Arrange
        var property = obj.GetProperty(o => o[0]);
        var ownedPropertyInfo = (IOwnedPropertyInfo)property;
        var newValue = "new-indexed-value";

        // Act
        ownedPropertyInfo.SetValue(newValue, [ 0 ]);

        // Assert
        obj[0].Should().Be(newValue);
    }
}
