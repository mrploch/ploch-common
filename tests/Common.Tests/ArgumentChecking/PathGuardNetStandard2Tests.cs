// These tests exercise members that exist ONLY in the netstandard2.0 partial of PathGuard
// (RequireValidPath has no equivalent on the net7+ partial — it was renamed to RequiredIsValidPath there).
// The file is included only on the net8.0 leg of Common.Tests, which links against the
// netstandard2.0 binary of Ploch.Common via SetTargetFramework on the ProjectReference. See issue #207.
using Ploch.Common.ArgumentChecking;

// ReSharper disable MissingXmlDoc
namespace Ploch.Common.Tests.ArgumentChecking;

public class PathGuardNetStandard2Tests
{
    [Fact]
    public void RequireValidPath_should_return_path_when_rooted_and_valid()
    {
        var validPath = Path.Combine(Path.GetTempPath(), "file.txt");

        var result = validPath.RequireValidPath(nameof(validPath));

        result.Should().Be(validPath);
    }

    [Fact]
    public void RequireValidPath_should_throw_InvalidOperationException_when_path_is_not_rooted()
    {
        var notRooted = Path.Combine("relative", "path", "file.txt");

        var act = () => notRooted.RequireValidPath(nameof(notRooted));

        act.Should().Throw<InvalidOperationException>().WithMessage($"*rooted*{nameof(notRooted)}*");
    }

    [Fact]
    public void RequireValidPath_should_throw_InvalidOperationException_when_path_contains_invalid_characters()
    {
        var invalidChar = Path.GetInvalidPathChars().First(c => c != '\0');
        var invalidPath = Path.Combine(Path.GetTempPath(), "bad" + invalidChar + "file.txt");

        var act = () => invalidPath.RequireValidPath(nameof(invalidPath));

        act.Should().Throw<InvalidOperationException>().WithMessage($"*invalid*{nameof(invalidPath)}*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void RequireValidPath_should_throw_when_path_is_null_or_empty(string? path)
    {
        var act = () => path.RequireValidPath(nameof(path));

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void EnsureFileExists_should_return_path_when_file_exists()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var result = tempFile.EnsureFileExists(nameof(tempFile));

            result.Should().Be(tempFile);
            File.Exists(result).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [Fact]
    public void EnsureFileExists_should_throw_ArgumentException_when_file_does_not_exist()
    {
        var nonExistent = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");

        var act = () => nonExistent.EnsureFileExists(nameof(nonExistent));

        act.Should().Throw<ArgumentException>().Which.Message.Should().Contain("does not exist");
    }

    [Fact]
    public void IsValidPath_should_return_path_when_valid()
    {
        var validPath = Path.Combine("test", "path", "file.txt");

        var result = validPath.IsValidPath(nameof(validPath));

        result.Should().Be(validPath);
    }

    [Fact]
    public void IsValidPath_should_throw_when_path_contains_invalid_characters()
    {
        var invalidChar = Path.GetInvalidPathChars().First(c => c != '\0');
        var invalidPath = "test" + invalidChar + "file.txt";

        var act = () => invalidPath.IsValidPath(nameof(invalidPath));

        act.Should().Throw<ArgumentException>().Which.Message.Should().Contain("invalid characters");
    }
}
