using Ploch.Common.Collections;

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
