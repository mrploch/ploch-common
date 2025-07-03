using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Ploch.Common.Randomizers;
using Xunit;

namespace Ploch.Common.Tests.Randomizers;

[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "False positive - this test is being used.")]
public class BooleanRandomizerTests : RandomizerTests<bool>
{
    protected override int DifferentValuesCheckCount => 1000;

    protected override int AtLeastHowManyValuesShouldBeDifferent => 2;

    [Fact]
    public void GetRandomValue_should_return_only_possible_option_for_range()
    {
        var sut = new BooleanRandomizer();

        sut.GetRandomValue(true, true).Should().BeTrue();

        sut.GetRandomValue(false, false).Should().BeFalse();
    }

    protected override IRandomizer<bool> CreateSUT() => new BooleanRandomizer();
}
