using FluentAssertions;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizers;

public abstract class BaseRandomizerEachDifferentTest<TValue>
{
    private readonly IRandomizer<TValue> _randomizer = Randomizer.GetRandomizer<TValue>();
    
    [Fact]
    public void GetValue_should_return_different_values()
    {
        var initialValue = _randomizer.GetValue();

        for (var i = 0; i < 10; i++)
        {
            var value = _randomizer.GetValue();
            value.Should().NotBeEquivalentTo(initialValue);

            initialValue = value;
        }
    }
}