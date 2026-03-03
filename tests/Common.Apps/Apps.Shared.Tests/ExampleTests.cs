using Ploch.TestingSupport.XUnit3.AutoMoq;

namespace Ploch.Common.Apps.Shared.Tests;

public class ExampleTests
{
    [AutoMockData]
    [Theory]
    public void Example_should_add_numbers_and_verify_its_correct(int a, int b)
    {
        var result = a + b;
        result.Should().Be(a + b);
    }
}
