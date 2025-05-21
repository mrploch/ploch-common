using FluentAssertions;
using Xunit;

namespace Ploch.Common.DataAnnotations.Tests;

public class RequiredNotDefaultDateAttributeTests
{
    public static IEnumerable<object?[]> Data =>
        new List<object?[]>
        {
            new object?[] { null, false },
            new object?[] { default(DateTime), false },
            new object?[] { default(DateTimeOffset), false },
            new object?[] { default(DateOnly), false },
            new object?[] { "some-string", false },
            new object?[] { 123, false },
            new object?[] { DateTime.Now, true },
            new object?[] { DateTimeOffset.Now, true },
            new object?[] { DateOnly.FromDateTime(DateTime.Now), true }
        };

    [Theory]
    [MemberData(nameof(Data))]
    public void IsValid_should_return_true_for_non_default_dates_and_false_for_default_or_null(object? value, bool expectedIsValidResult)
    {
        var sut = new RequiredNotDefaultDateAttribute();

        sut.IsValid(value).Should().Be(expectedIsValidResult);
    }
}