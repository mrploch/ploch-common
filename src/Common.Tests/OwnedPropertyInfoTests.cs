using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Tests.Reflection;
using Xunit;

namespace Ploch.Common.Linq;

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
}