using System;
using System.Linq.Expressions;
using FluentAssertions;
using Ploch.Common.Tests.Reflection;
using Xunit;

namespace Ploch.Common.Tests
{
    public class ExpressionExtensionsTests
    {
        public string MySelfProperty { get; set; }

        [Fact]
        public void GetMemberName_NoType_Method()
        {
            Expression<Func<int>> expression = () => GetHashCode();
            var name = expression.GetMemberName();
            name.Should().Be("GetHashCode");
        }

        [Fact]
        public void GetMemberName_NoType_Property()
        {
            Expression<Func<string>> expression = () => MySelfProperty;
            var name = expression.GetMemberName();
            name.Should().Be("MySelfProperty");
        }

        [Fact]
        public void GetMemberNameTest_FromType_Property()
        {
            Expression<Func<TestTypes.Class1, string>> expression = mc => mc.MyProperty;

            var name = expression.GetMemberName();
            name.Should().Be("MyProperty");
        }

        [Fact]
        public void GetMemberName_should_return_variable_name()
        {
            string str = "test";
            Expression<Func<string>> expression = () => str;

            expression.GetMemberName().Should().Be("str");
            
        }
    }
}