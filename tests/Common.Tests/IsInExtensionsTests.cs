using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Xunit;

namespace Ploch.Common.Tests;

public class IsInExtensionsTests
{
    [Theory]
    [AutoMockData]
    public void In_should_return_true_if_string_is_in_the_list(List<string> strings)
    {
        "test".In("t1", "t2", "test").Should().BeTrue();
    }

    [Theory]
    [AutoMockData]
    public void In_should_return_false_if_string_is_in_the_list(List<string> strings)
    {
        "test".In("t1", "t2").Should().BeFalse();
    }

    [Theory]
    [AutoMockData]
    public void NotIn_should_return_true_if_string_is_in_the_list(List<string> strings)
    {
        strings.Add("test");
        "test".NotIn("t1", "t2", "test").Should().BeFalse();
    }

    [Theory]
    [AutoMockData]
    public void NotIn_should_return_false_if_string_is_in_the_list(List<string> strings)
    {
        "test".NotIn("t1", "t2").Should().BeTrue();
    }
}
