using System.Globalization;
using FluentAssertions;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests.ArgumentChecking;

public class PathGuardTests
{
    [Theory]
    [InlineData(null, new[] { "cannot", "null" })]
    [InlineData("", new[] { "cannot", "empty" })]
    public void RequireValidPath_should_throw_ArgumentException_when_path_is_null(string? path, IEnumerable<string> expectedStringsInExceptionMessage)
    {
        // Arrange
        // Act & Assert
        var act = () => path.IsValidPath();
        act.Should()
           .Throw<ArgumentException>()
           .Which.Message.ToLower(CultureInfo.InvariantCulture)
           .Should()
           .ContainAll([ nameof(path), ..expectedStringsInExceptionMessage ]);
    }

    [Fact]
    public void IsValidPath_should_throw_ArgumentException_when_path_contains_invalid_characters()
    {
        // Arrange
        var invalidPath = "test" + Path.GetInvalidPathChars()[0] + "path.txt";

        // Act & Assert
        var act = () => invalidPath.IsValidPath();
        act.Should().Throw<ArgumentException>().Which.Message.ToLower(CultureInfo.InvariantCulture).Should().Contain("contains invalid characters");
    }

    [Fact]
    public void IsValidPath_should_return_original_path_when_valid()
    {
        // Arrange
        var validPath = Path.Combine("test", "valid", "path.txt");

        // Act
        var result = validPath.IsValidPath();

        // Assert
        result.Should().Be(validPath);
    }

    [Fact]
    public void IsValidPath_should_accept_paths_with_valid_special_characters()
    {
        // Arrange
        var validSpecialCharPath = Path.Combine("test", "path with spaces", "$pecial_Chars-[]()", "file.txt");

        // Act
        var result = validSpecialCharPath.IsValidPath();

        // Assert
        result.Should().Be(validSpecialCharPath);
    }

    [Fact]
    public void RequiredIsValidPath_should_throw_ArgumentException_when_path_is_invalid()
    {
        // Arrange
        var invalidPath = $"a:\\this/is:an*invalid<path>{Path.InvalidPathChars.TakeRandom(1).First()}{Path.InvalidPathChars.TakeRandom(1).First()}";

        // Act & Assert
        var act = () => invalidPath.RequiredIsValidPath();
        act.Should()
           .Throw<InvalidOperationException>()
           .Which.Message.ToLower(CultureInfo.InvariantCulture)
           .Should()
           .ContainAll("path", "invalid", invalidPath.ToLowerInvariant());
    }

    [Theory]
    [InlineData(null, new[] { "cannot", "null" })]
    [InlineData("", new[] { "cannot", "empty" })]
    public void RequiredIsValidPath_should_throw_InvalidOperationException_when_path_is_null_or_empty(
        string? path,
        IEnumerable<string> expectedStringsInExceptionMessage)
    {
        // Arrange
        // Act & Assert
        var act = () => path.RequiredIsValidPath();
        act.Should()
           .Throw<InvalidOperationException>()
           .Which.Message.ToLower(CultureInfo.InvariantCulture)
           .Should()
           .ContainAll(expectedStringsInExceptionMessage);
    }

    [Fact]
    public void EnsureFileExists_should_throw_ArgumentException_when_file_does_not_exist()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nonexistent.file");

        // Act & Assert
        var act = () => nonExistentPath.EnsureFileExists();
        act.Should()
           .Throw<ArgumentException>()
           .Which.Message.ToLower(CultureInfo.InvariantCulture)
           .Should()
           .ContainAll("path does not exist", nonExistentPath.ToLowerInvariant());
    }

    [Fact]
    public void EnsureFileExists_should_return_original_path_when_file_exists()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName(); // Creates an empty file in the temp directory

        try
        {
            // Act
            var result = tempFilePath.EnsureFileExists();

            // Assert
            result.Should().Be(tempFilePath);
        }
        finally
        {
            // Clean up
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    public void RequiredFileExists_should_throw_InvalidOperationException_when_file_does_not_exist()
    {
        // Arrange
        var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "nonexistent.file");

        // Act & Assert
        var act = () => nonExistentPath.RequiredFileExists();
        act.Should()
           .Throw<InvalidOperationException>()
           .Which.Message.ToLower(CultureInfo.InvariantCulture)
           .Should()
           .ContainAll("path does not exist", nonExistentPath.ToLowerInvariant());
    }

    [Fact]
    public void RequiredFileExists_should_return_original_path_when_file_exists()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName(); // Creates an empty file in the temp directory

        try
        {
            // Act
            var result = tempFilePath.RequiredFileExists();

            // Assert
            result.Should().Be(tempFilePath);
        }
        finally
        {
            // Clean up
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    [Fact]
    public void EnsureFileExists_should_properly_handle_relative_paths()
    {
        // Arrange
        var tempDir = Path.GetTempPath();
        var fileName = $"test_{Guid.NewGuid()}.txt";
        var fullPath = Path.Combine(tempDir, fileName);
        File.WriteAllText(fullPath, "Test content");

        try
        {
            // Change to the temp directory to make the relative path work
            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(tempDir);

            try
            {
                // Act
                var result = fileName.EnsureFileExists(); // Using relative path

                // Assert
                result.Should().Be(fileName);
                Path.GetFileName(result).Should().Be(fileName);
            }
            finally
            {
                // Restore the original directory
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }
        finally
        {
            // Clean up
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }

    [Fact]
    public void IsValidPath_should_handle_paths_at_maximum_allowed_length()
    {
        // Arrange
        var maxPathLength = 260; // Standard MAX_PATH on Windows
        var fileName = "test.txt";
        var remainingLength = maxPathLength - fileName.Length - 1; // -1 for path separator
        var longDirectoryPath = new string('a', remainingLength);
        var maxLengthPath = Path.Combine(longDirectoryPath, fileName);

        // Act
        var result = maxLengthPath.IsValidPath();

        // Assert
        result.Should().Be(maxLengthPath);
        result.Length.Should().Be(maxPathLength);
    }
}
