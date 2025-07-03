using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizers;

public class IntRandomizerTests : RandomizerTests<int>
{
    [Theory]
    [AutoMockData]
    public void GetRandomValue_should_return_values_within_range(int valueOne, int valueTwo)
    {
        var sut = CreateSUT();

        var minValue = Math.Min(valueOne, valueTwo);
        var maxValue = Math.Max(valueOne, valueTwo);

        for (var i = 0; i < DifferentValuesCheckCount; i++)
        {
            var value = sut.GetRandomValue(minValue, maxValue);
            value.Should().BeGreaterThanOrEqualTo(minValue);
            value.Should().BeLessThanOrEqualTo(maxValue);
        }
    }

    protected override IRangedRandomizer<int> CreateSUT() => Randomizer.GetRandomizer<int>();
}
