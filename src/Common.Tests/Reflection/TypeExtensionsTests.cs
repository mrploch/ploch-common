using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Xunit;

namespace Ploch.Common.Reflection.Tests;

[SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "Classes here are used for testing purposes and are intentionally empty.")]
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

    [Theory]
    [AutoMockData]
    public void IsEnumerable_should_return_true_if_type_is_array(int[] array)
    {
        array.GetType().IsEnumerable().Should().BeTrue();
    }

    [Theory]
    [AutoMockData]
    public void IsEnumerable_should_return_true_if_type_is_list(List<string> list)
    {
        list.GetType().IsEnumerable().Should().BeTrue();
    }

    [Theory]
    [AutoMockData]
    public void IsEnumerable_should_return_true_if_type_is_enumerable(IEnumerable<string> list)
    {
        list.GetType().IsEnumerable().Should().BeTrue();
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