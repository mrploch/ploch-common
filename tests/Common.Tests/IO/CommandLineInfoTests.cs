using Ploch.Common.IO;

namespace Ploch.Common.Tests.IO;

public class CommandLineInfoTests
{
    [Fact]
    public void Properties_should_expose_application_path_and_arguments()
    {
        // Arrange
        var appPath = @"C:\\Apps\\Tool.exe";
        var args = new[] { "-v", "--name", "Test" };

        // Act
        var info = new CommandLineInfo(appPath, args);

        // Assert
        info.ApplicationPath.Should().Be(appPath);
        info.Arguments.Should().BeEquivalentTo(args);
    }

    [Fact]
    public void Equals_should_return_true_when_applicationPath_and_arguments_match()
    {
        // Arrange
        var args1 = new List<string> { "-a", "1", "-b", "two" };
        var args2 = new[] { "-a", "1", "-b", "two" }; // different sequence instance, same content

        var left = new CommandLineInfo(@"C:\x\y.exe", args1);
        var right = new CommandLineInfo(@"C:\x\y.exe", args2);

        // Act / Assert
        left.Equals(right).Should().BeTrue();
        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
    }

    [Fact]
    public void Equals_should_return_false_when_applicationPath_differs()
    {
        // Arrange
        var args = new[] { "-a", "1" };

        var left = new CommandLineInfo(@"C:\a\b.exe", args);
        var right = new CommandLineInfo(@"C:\a\c.exe", args);

        // Act / Assert
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();
    }

    [Fact]
    public void Equals_should_return_false_when_arguments_differ()
    {
        // Arrange
        var left = new CommandLineInfo(@"C:\a\b.exe", ["-a", "1"]);
        var right = new CommandLineInfo(@"C:\a\b.exe", ["-a", "2"]);

        // Act / Assert
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();
    }

    [Fact]
    public void Equals_should_handle_null_applicationPath_values()
    {
        // Arrange
        var args = Array.Empty<string>();

        var left = new CommandLineInfo(null, args);
        var right = new CommandLineInfo(null, args);

        // Act / Assert
        left.Equals(right).Should().BeTrue();
        (left == right).Should().BeTrue();
        left.GetHashCode().Should().Be(right.GetHashCode());
    }
}
