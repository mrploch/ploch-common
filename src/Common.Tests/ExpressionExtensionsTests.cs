using System.Linq.Expressions;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Linq;
using Ploch.Common.Tests.Reflection;
using Xunit;

namespace Ploch.Common.Tests
{
    //   [SuppressMessage("ReSharper", "LambdaExpressionCanBeMadeStatic")]
    public class ExpressionExtensionsTests
    {
        public string? MySelfProperty { get; set; }

        [Fact]
        public void GetMemberName_NoType_Method()
        {
            Expression<Func<int>> expression = () => GetHashCode();
            var name = expression.GetMemberName();
            name.Should().Be(nameof(GetHashCode));
        }

        [Fact]
        public void GetMemberName_NoType_Property()
        {
            Expression<Func<string?>> expression = () => MySelfProperty;
            var name = expression.GetMemberName();
            name.Should().Be(nameof(MySelfProperty));
        }

        [Fact]
        public void GetMemberNameTest_FromType_Property()
        {
            Expression<Func<TestTypes.Class1, string?>> expression = mc => mc.MyProperty;

            var name = expression.GetMemberName();
            name.Should().Be("MyProperty");
        }

        [Fact]
        public void GetMemberName_should_return_variable_name()
        {
            var str = "test";
            Expression<Func<string>> expression = () => str;

            expression.GetMemberName().Should().Be(nameof(str));
        }

        [Theory]
        [AutoMockData]
        public void GetProperty_should_return_OwnedPropertyInfo_for_the_provided_property_selector_expression(TestTypes.MyTestClass obj)
        {
            var property = obj.GetProperty(o => o.StringProp2);

            property.Should().NotBeNull().And.BeOfType<OwnedPropertyInfo>();
            property.Owner.Should().Be(obj);
        }
    }
}