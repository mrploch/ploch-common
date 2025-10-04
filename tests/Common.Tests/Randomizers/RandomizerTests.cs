using Ploch.Common.Randomizers;

namespace Ploch.Common.Tests.Randomizers;

public class RandomizerTests
{
    [Fact]
    public void Randomizer_should_return_randomizer_for_correct_type()
    {
        Randomizer.GetRandomizer(typeof(string)).Should().BeOfType<StringRandomizer>();
        Randomizer.GetRandomizer(typeof(int)).Should().BeOfType<IntRandomizer>();
        Randomizer.GetRandomizer(typeof(DateTime)).Should().BeOfType<DateTimeRandomizer>();
        Randomizer.GetRandomizer(typeof(DateTimeOffset)).Should().BeOfType<DateTimeOffsetRandomizer>();
        Randomizer.GetRandomizer(typeof(bool)).Should().BeOfType<BooleanRandomizer>();
    }
}

public abstract class RandomizerTests<TValue> where TValue : notnull
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

    protected virtual IRandomizer<TValue> CreateSUT() => Randomizer.GetRandomizer<TValue>();
}
