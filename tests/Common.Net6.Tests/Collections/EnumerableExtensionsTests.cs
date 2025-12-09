using AutoFixture;
using FluentAssertions;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Net6.Tests.Collections;

public class EnumerableExtensionsTests
{
    [Fact]
    public void Shuffle_should_randomly_shuffle_items_in_collection()
    {
        var strings = new Fixture().CreateMany<string>(20);
        var result = strings.Shuffle();

        // ReSharper disable once PossibleMultipleEnumeration
        result.Should().BeEquivalentTo(strings);

        var arrayStrings = strings.ToArray();
        var arrayResult = result.ToArray();

        var sameOrder = true;
        for (var i = 0; i < arrayResult.Length; i++)
        {
            if (arrayResult[i] != arrayStrings[i])
            {
                sameOrder = false;

                break;
            }
        }

        sameOrder.Should().BeFalse();
    }
}
