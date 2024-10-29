using FluentAssertions;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizer;

public class StringRandomizerTests : RandomizerTests<string>
{
    protected override IRangedRandomizer<string> CreateSUT() => Randomizers.Randomizer.GetRandomizer<string>();

    [Fact]
    public void GetRandomValue_should_return_values_within_range()
    {
        var sut = CreateSUT();

        const int minValue = 'a';
        const int maxValue = 'z';

        for (var i = 0; i < DifferentValuesCheckCount; i++)
        {
            var value = sut.GetRandomValue("a", "z");

            foreach (var character in value)
            {
                int characterNum = character;

                characterNum.Should().BeLessOrEqualTo(minValue);
                characterNum.Should().BeLessOrEqualTo(maxValue);
            }
        }
    }
}