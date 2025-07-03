using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests.Collections;

public class ArrayExtensionsTests
{
    [Theory]
    [AutoMockData]
    public void Exists_should_return_true_if_item_is_found(List<string> items)
    {
        items.Add("test-value");
        var sut = items.ToArray();

        sut.Exists(s => s == "test-value").Should().BeTrue();
        sut.Exists(s => s == "non existent value").Should().BeFalse();
    }
}
