using FluentAssertions;
using Ploch.Common.Reflection;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

public class TypeExtensionsTests
{
    [Fact]
    public void IsImplementing_should_return_true_if_type_is_implementing_interface()
    {
        typeof(TestClass1).IsImplementing(typeof(ITestInterface1)).Should().BeTrue();
        typeof(TestClass1).IsImplementing(typeof(ITestInterface2)).Should().BeTrue();
        typeof(TestClass2).IsImplementing(typeof(ITestInterface1)).Should().BeTrue();
        typeof(TestClass2).IsImplementing(typeof(ITestInterface2)).Should().BeTrue();

        typeof(ITestInterface4).IsImplementing(typeof(ITestInterface1)).Should().BeTrue();
        typeof(ITestInterface4).IsImplementing(typeof(ITestInterface2)).Should().BeTrue();
    }

    [Fact]
    public void IsImplementing_should_return_false_if_type_is_not_implementing_interface()
    {
        typeof(TestClass1).IsImplementing(typeof(ITestInterface3)).Should().BeFalse();
        typeof(ITestInterface4).IsImplementing(typeof(ITestInterface3)).Should().BeFalse();
    }

    private interface ITestInterface1
    { }

    private interface ITestInterface2
    { }

    private interface ITestInterface3
    { }

    private class TestClass1 : ITestInterface1, ITestInterface2
    { }

    private interface ITestInterface4 : ITestInterface1, ITestInterface2
    { }

    private class TestClass2 : TestClass1
    { }
}