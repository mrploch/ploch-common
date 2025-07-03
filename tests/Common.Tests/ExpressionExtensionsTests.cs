using System.Linq.Expressions;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Linq;
using Ploch.Common.Tests.Reflection;
using Xunit;

namespace Ploch.Common.Tests;

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
    public void GetMemberName_FromType_Property()
    {
        Expression<Func<TestTypes.Class1, string?>> expression = mc => mc.MyProperty;

        var name = expression.GetMemberName();
        name.Should().Be("MyProperty");
    }

    [Fact]
    public void GetMemberName_for_method_call_should_return_method_name()
    {
        Expression<Func<TestType, int>> methodCallExpression = mc => mc.MethodWithReturn();

        methodCallExpression.GetMemberName().Should().Be(nameof(TestType.MethodWithReturn));
    }

    [Fact]
    public void GetMemberName_for_primitive_property_as_object_should_return_property_name()
    {
        Expression<Func<TestType, object>> unaryExpression = mc => mc.IntProp;
        unaryExpression.GetMemberName().Should().Be(nameof(TestType.IntProp));
    }

    [Fact]
    public void GetMemberName_should_return_variable_name()
    {
        var str = "test";
        Expression<Func<string>> expression = () => str;

        expression.GetMemberName().Should().Be(nameof(str));
    }

    [Fact]
    public void GetMemberName_should_return_method_name_method_with_args()
    {
        var obj = new TestType();
        Expression<Action> expression = () => obj.MethodWithArg(123);

        expression.GetMemberName().Should().Be(nameof(TestType.MethodWithArg));
    }

    [Fact]
    public void GetMemberName_should_return_method_name_method_with_no_args()
    {
        var obj = new TestType();
        Expression<Action> expression = () => obj.MethodWithNoArgs();

        expression.GetMemberName().Should().Be(nameof(TestType.MethodWithNoArgs));
    }

    [Fact]
    public void GetMemberName_should_return_method_name_method_with_return()
    {
        var obj = new TestType();
        Expression<Action> expression = () => obj.MethodWithReturn();

        expression.GetMemberName().Should().Be(nameof(TestType.MethodWithReturn));
    }

    [Theory]
    [AutoMockData]
    public void GetProperty_should_return_OwnedPropertyInfo_for_the_provided_property_selector_expression(TestTypes.MyTestClass obj)
    {
        var property = obj.GetProperty(o => o.StringProp2);

        property.Should().NotBeNull().And.BeOfType<OwnedPropertyInfo<TestTypes.MyTestClass, string>>();
        property.Owner.Should().Be(obj);
    }

#pragma warning disable CA1822
#pragma warning disable CC0091
#pragma warning disable S1186
#pragma warning disable CC0057
#pragma warning disable S3459

    // ReSharper disable once UnusedParameter.Local
    public class TestType
    {
        public int IntProp { get; set; }

        public void MethodWithArg(int i)
        { }

        public void MethodWithNoArgs()
        { }

        public int MethodWithReturn() => 123;

        [Theory]
        [AutoMockData]
        public void GetProperty_should_return_OwnedPropertyInfo_for_the_provided_property_selector_expression(TestTypes.MyTestClass obj)
        {
            var property = obj.GetProperty(o => o.StringProp2);

            property.Should().NotBeNull().And.BeOfType<OwnedPropertyInfo<TestTypes.MyTestClass, string>>();
            property.Owner.Should().Be(obj);
        }
    }
#pragma warning restore S3459
#pragma warning restore CC0057
#pragma warning restore S1186
#pragma warning restore CC0091
#pragma warning restore CA1822
}
