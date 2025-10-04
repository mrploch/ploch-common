using Ploch.Common.Randomizers;

namespace Ploch.Common.Tests.Randomizers;

public class DateTimeOffsetRandomizerTests : RandomizerTests<DateTimeOffset>
{
    [Theory]
    [AutoMockData]
    public void GetRandomValue_should_return_values_within_range(DateTime dateOne, DateTime dateTwo)
    {
        var sut = CreateSUT();

        var minValue = dateOne < dateTwo ? dateOne : dateTwo;
        var maxValue = dateOne < dateTwo ? dateTwo : dateOne;

        for (var i = 0; i < DifferentValuesCheckCount; i++)
        {
            var value = sut.GetRandomValue(minValue, maxValue);
            value.Should().BeOnOrAfter(minValue);
            value.Should().BeOnOrBefore(maxValue);
        }
    }

    protected override IRangedRandomizer<DateTimeOffset> CreateSUT() => Randomizer.GetRandomizer<DateTimeOffset>();
}
