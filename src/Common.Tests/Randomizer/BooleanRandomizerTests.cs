using Ploch.Common.Randomizers;

namespace Ploch.Common.Tests.Randomizer;

public class BooleanRandomizerTests : RandomizerTests<bool>
{
    protected override int DifferentValuesCheckCount => 1000;
    protected override int AtLeastHowManyValuesShouldBeDifferent => 2;

    protected override IRandomizer<bool> CreateSUT() => new BooleanRandomizer();
}