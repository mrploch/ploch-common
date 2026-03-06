using Ploch.Common.Randomizers;

namespace Ploch.Common.Tests.Randomizers;

public class BaseRandomizerTests
{
    [Fact]
    public void GetRandomValue_non_generic_method_should_return_random_value()
    {
        var sut = Randomizer.GetRandomizer<int>();

        var randomValue = sut.GetRandomValue(5, (object)10);

        randomValue.Should().BeOfType<int>();
        randomValue.As<int>().Should().BeGreaterThan(4).And.BeLessThan(11);
    }
}
