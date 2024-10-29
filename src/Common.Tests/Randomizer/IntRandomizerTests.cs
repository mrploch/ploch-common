using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizer;

public class IntRandomizerTests : RandomizerTests<int>
{
    protected override IRangedRandomizer<int> CreateSUT() => Randomizers.Randomizer.GetRandomizer<int>();

    [Theory]
    [AutoMockData]
    public void GetRandomValue_should_return_values_within_range(int minValue, int maxValue)
    {
        var sut = CreateSUT();

        for (var i = 0; i < DifferentValuesCheckCount; i++)
        {
            var value = sut.GetRandomValue(minValue, maxValue);
            value.Should().BeLessOrEqualTo(minValue);
            value.Should().BeLessOrEqualTo(maxValue);
        }
    }
}