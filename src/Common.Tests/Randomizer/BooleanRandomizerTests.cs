using System.Diagnostics.CodeAnalysis;
using Ploch.Common.Randomizers;

namespace Ploch.Common.Tests.Randomizer;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "False positive - this test is being used.")]
public class BooleanRandomizerTests : RandomizerTests<bool>
{
    protected override int DifferentValuesCheckCount => 1000;

    protected override int AtLeastHowManyValuesShouldBeDifferent => 2;

    protected override IRandomizer<bool> CreateSUT()
    {
        return new BooleanRandomizer();
    }
}
