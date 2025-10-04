using FluentAssertions;
using Ploch.Common.ArgumentChecking;

// ReSharper disable MissingXmlDoc
namespace Ploch.Common.Tests.Net6.ArgumentChecking;

public class PathGuardTests
{
  [Fact]
  public void IsValidPath_should_return_original_path_when_valid()
  {
    // Arrange
    var validPath = Path.Combine("test", "path", "file.txt");

    // Act
    var result = validPath.IsValidPath(nameof(validPath));

    // Assert
    result.Should().Be(validPath);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void IsValidPath_should_throw_ArgumentException_when_path_is_invalid(string? invalidPath)
  {
    // Act & Assert
    var act = () => invalidPath.IsValidPath(nameof(invalidPath));

    act.Should().Throw<ArgumentException>().Which.ParamName.Should().Be(nameof(invalidPath));
  }

  [Fact]
  public void IsValidPath_should_throw_ArgumentException_when_path_contains_invalid_characters()
  {
    // Arrange
    var invalidPath = "test" + Path.GetInvalidPathChars()[0] + "file.txt";

    // Act & Assert
    var act = () => invalidPath.IsValidPath(nameof(invalidPath));

    act.Should().Throw<ArgumentException>().Which.Message.Should().Contain("invalid characters");
  }

  [Fact]
  public void EnsureFileExists_should_return_path_when_file_exists()
  {
    // Arrange
    var tempFile = Path.GetTempFileName();

    try
    {
      // Act
      var result = tempFile.EnsureFileExists(nameof(tempFile));

      // Assert
      result.Should().Be(tempFile);
      File.Exists(result).Should().BeTrue();
    }
    finally
    {
      // Cleanup
      if (File.Exists(tempFile))
      {
        File.Delete(tempFile);
      }
    }
  }

  [Fact]
  public void EnsureFileExists_should_throw_ArgumentException_when_file_does_not_exist()
  {
    // Arrange
    var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    // Act & Assert
    var act = () => nonExistentPath.EnsureFileExists(nameof(nonExistentPath));

    act.Should().Throw<ArgumentException>().Which.Message.Should().Contain("does not exist");
  }

    [Fact]
    public void RequiredIsValidPath_should_throw_InvalidOperationException_when_path_is_invalid()
    {
        // Arrange
        var invalidPath = "invalid" + Path.GetInvalidPathChars()[0] + "path";

        // Act & Assert
        var act = () => invalidPath.RequireValidPath(nameof(invalidPath));

        act.Should().Throw<InvalidOperationException>().Which.Message.Should().Contain("invalid");
    }
}
