using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests
{
    public class EnvironmentUtilsTests
    {
        [Fact]
        public void GetEnvironmentCommandLine_should_retrieve_current_commandline()
        {
            var commandLine = EnvironmentUtilities.GetEnvironmentCommandLine(true);

            commandLine.Should().NotBeNull().And.NotBeEmpty();
        }

        [Fact]
        public void GetCurrentAppPath_should_not_be_empty_and_be_an_existing_directory()
        {
            var appPath = EnvironmentUtilities.GetCurrentAppPath();

            appPath.Should().NotBeNull().And.NotBeEmpty();

            Directory.Exists(appPath).Should().BeTrue();
        }
    }
}