using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests;

public class IfNullHelpersTests
{
    [Theory]
    [InlineData("value", "default", "value")]
    [InlineData(null, "default", "default")]
    public void OrIfNull_should_return_value_or_default(string? value, string defaultValue, string expected)
    {
        value.OrIfNull(defaultValue).Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(EnumerableTestData))]
    public void OrIfNullOrEmpty_should_return_enumerable_or_default(IEnumerable<int> enumerable, IEnumerable<int> defaultValue, IEnumerable<int> expected)
    {
        enumerable.OrIfNullOrEmpty(defaultValue).Should().BeSameAs(expected);
    }

    public static IEnumerable<object?[]> EnumerableTestData()
    {
        var list = new List<int> { 1, 2, 3 };
        var def = new List<int> { 4, 5, 6 };

        yield return new object[] { list, def, list };
        yield return new object?[] { null, def, def };
    }
}
