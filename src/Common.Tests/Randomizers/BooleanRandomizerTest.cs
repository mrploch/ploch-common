using JetBrains.Annotations;
using Ploch.Common.Randomizers;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ploch.Common.Tests.Randomizers;

[TestSubject(typeof(BooleanRandomizer))]
public class BooleanRandomizerTest
{
    private readonly IRandomizer<bool> _booleanRandomizer = Randomizer.GetRandomizer<bool>();


    [Fact]
    public void GetValue_should_be_random()
    {
        // Arrange
        var results = new List<bool>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            results.Add(_booleanRandomizer.GetValue());
        }

        // Assert
        results.Should().Contain(true);
        results.Should().Contain(false);
    }
}