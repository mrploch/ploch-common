using FluentAssertions;
using Xunit;

namespace Ploch.TestingSupport.XUnit3.Dependencies;

public class SmokeTests
{
    [Fact]
    public void Dependencies_package_should_be_test_discoverable()
    {
        true.Should().BeTrue();
    }
}
