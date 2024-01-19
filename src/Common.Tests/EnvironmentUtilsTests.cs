using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests;

public class EnvironmentUtilsTests
{
    [Fact]
    public void GetEnvironmentCommandLine_should_retrieve_current_commandline()
    {
        var commandLine = EnvironmentUtilities.GetEnvironmentCommandLine(true);

        commandLine.Should().NotBeNull().And.NotBeEmpty();
    }
}