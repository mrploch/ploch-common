namespace Ploch.Common.Tests.Collections;

public class KeyValuePairExtensionsTests
{
    [Theory]
    [AutoMockData]
    public void DeconstructTest(Dictionary<string, int> dictionary)
    {
        dictionary.Should().NotBeEmpty();

        foreach (var (key, value) in dictionary)
        {
            key.Should().NotBeNull();
        }
    }
}
