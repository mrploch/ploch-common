using FluentAssertions;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizer;

public abstract class RandomizerTests<TValue>
    where TValue : notnull
{
    protected virtual int DifferentValuesCheckCount => 10;

    protected virtual int AtLeastHowManyValuesShouldBeDifferent => 10;

    [Fact]
    public void GetRandomValue_should_return_random_value()
    {
        var values = new Dictionary<TValue, int>();

        var sut = CreateSUT();

        for (var i = 0; i < DifferentValuesCheckCount; i++)
        {
            var randomValue = sut.GetRandomValue();
            if (!values.TryAdd(randomValue, 1))
            {
                values[randomValue]++;
            }
        }

        values.Count.Should().BeGreaterThanOrEqualTo(AtLeastHowManyValuesShouldBeDifferent);
    }

    protected virtual IRandomizer<TValue> CreateSUT()
    {
        return Randomizers.Randomizer.GetRandomizer<TValue>();
    }
}
