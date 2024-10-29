using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizer;

public class DateTimeRandomizerTests : RandomizerTests<DateTime>
{
    protected override IRangedRandomizer<DateTime> CreateSUT() => Randomizers.Randomizer.GetRandomizer<DateTime>();

    [Theory]
    [AutoMockData]
    public void GetRandomValue_should_return_values_within_range(DateTime minValue, DateTime maxValue)
    {
        var sut = CreateSUT();

        for (var i = 0; i < DifferentValuesCheckCount; i++)
        {
            var value = sut.GetRandomValue(minValue, maxValue);
            value.Should().BeOnOrAfter(minValue);
            value.Should().BeOnOrBefore(maxValue);
        }
    }
}