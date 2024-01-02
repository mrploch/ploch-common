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
}