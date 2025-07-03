using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests;

public class ComparisonUtilsTests
{
    [Fact]
    public void IsNotDefault_should_be_true_for_non_default_value()
    {
        1.IsNotDefault().Should().BeTrue();
    }

    [Fact]
    public void IsNotDefault_should_be_false_for_default_value()
    {
        default(int).IsNotDefault().Should().BeFalse();
    }

    [Fact]
    public void IsNotDefault_should_be_false_for_default_null_value()
    {
        default(string).IsNotDefault().Should().BeFalse();
    }

    [Fact]
    public void IsDefault_should_be_false_for_non_default_value()
    {
        1.IsDefault().Should().BeFalse();
    }

    [Fact]
    public void IsDefault_should_be_true_for_default_value()
    {
        default(int).IsDefault().Should().BeTrue();
    }

    [Fact]
    public void IsDefault_should_be_true_for_default_null_value()
    {
        default(string).IsDefault().Should().BeTrue();
    }
}
