using System.Diagnostics;
using System.Text;
using Ploch.Common.Collections;
using Ploch.Common.IO;
using Ploch.TestingSupport.XUnit3.Dependencies;

namespace Ploch.Common.Tests.IO;

public class PathUtilsTests
{
    [Fact]
    public void GetDirectoryName_should_return_folder_name_in_provided_path() =>
        "c:/myrootfolder/mysubfolder/expected-folder-name".GetDirectoryName().Should().Be("expected-folder-name");

    [Fact]
    public void GetDirectoryName_should_throw_exception_when_directoryPath_is_null()
    {
        // Act
        Action act = () => ((string)null!).GetDirectoryName();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("directoryPath");
    }

    [Fact]
    public void GetDirectoryName_should_throw_exception_when_directoryPath_is_empty()
    {
        // Act
        Action act = () => string.Empty.GetDirectoryName();

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("directoryPath").WithMessage("*empty*");
    }

    [SupportedOSPlatform(SupportedOS.Windows)]
    [Theory]
    [InlineData("d:", @"D:\")]
    [InlineData("c:/", @"c:\")]
    public void GetDirectoryName_should_return_name_of_root_directory(string directoryPath, string expectedDirectoryName)
    {
        var directoryName = directoryPath.GetDirectoryName();

        // For root directories, DirectoryInfo.Name returns the root name itself
        directoryName.Should().Be(expectedDirectoryName);
    }

    [Theory]
    [InlineData(@"c:\myrootfolder", @"c:\myrootfolder\mysubfolder\myfile.txt", @"mysubfolder\myfile.txt")]
    [InlineData(@"c:\myrootfolder", @"c:\myrootfolder\mysubfolder\my_another_sub_folder", @"mysubfolder\my_another_sub_folder")]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void MakeRelativePath_should_return_relative_path_from_one_path_to_another(string fromPath, string toPath, string expectedRelativePath) =>
        PathUtils.GetRelativePath(fromPath, toPath).Should().Be(expectedRelativePath);

    [Theory]
    [SupportedOSPlatform(SupportedOS.macOS)]
    [InlineData(@"/myrootfolder", @"/myrootfolder/mysubfolder/myfile.txt", @"mysubfolder/myfile.txt")]
    [InlineData(@"/Users/someuser/dev", @"/Users/someuser/dev/someanotherfolder/and-yet-another", @"someanotherfolder/and-yet-another")]
    [InlineData(@"/Users/someuser/dev", @"/Users/someuser/dev/someanotherfolder/and-yet-another/test.txt", @"someanotherfolder/and-yet-another/test.txt")]
    public void MakeRelativePath_should_return_relative_path_from_one_path_to_another_on_MacOS(string fromPath, string toPath, string expectedRelativePath) =>
        PathUtils.GetRelativePath(fromPath, toPath).Should().Be(expectedRelativePath);

    [Fact]
    public void NormalizePathWithTrailingSeparator_should_add_directory_separator_to_path_that_doesnt_have_one()
    {
        // Arrange
        var path = GenerateFolderPath(3, false);
        var expected = path + Path.DirectorySeparatorChar;

        // Act
        var result = PathUtils.NormalizePathWithTrailingSeparator(path);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void NormalizePathWithTrailingSeparator_should_throw_exception_when_path_is_null()
    {
        // Act
        var act = () => PathUtils.NormalizePathWithTrailingSeparator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("path");
    }

    [Fact]
    public void NormalizePathWithTrailingSeparator_should_throw_exception_when_path_is_empty()
    {
        // Act
        var act = () => PathUtils.NormalizePathWithTrailingSeparator(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("path").WithMessage("*empty*");
    }

    [Fact]
    public void NormalizePathWithTrailingSeparator_should_normalize_relative_paths_to_absolute_paths()
    {
        // Arrange
        var relativePath = "..\\folder";
        var expectedAbsolutePath = Path.GetFullPath(relativePath) + Path.DirectorySeparatorChar;

        // Act
        var result = PathUtils.NormalizePathWithTrailingSeparator(relativePath);

        // Assert
        result.Should().Be(expectedAbsolutePath);
        Path.IsPathRooted(result).Should().BeTrue();
    }

    [Fact(Skip = "On Linux and MacOS, the backslash is a legal character in path, hence it's not modified")]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void NormalizePathWithTrailingSeparator_should_handle_paths_with_mixed_separators()
    {
        // Arrange
        var pathWithMixedSeparators = @"c:\myrootfolder/mysubfolder\anotherfolder/";
        var expected = @$"c:\myrootfolder\mysubfolder\anotherfolder{Path.DirectorySeparatorChar}";

        // Act
        var result = PathUtils.NormalizePathWithTrailingSeparator(pathWithMixedSeparators);

        // Assert
        result.Should().Be(expected);
        result.Should().EndWith(Path.DirectorySeparatorChar.ToString());
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void NormalizePathWithTrailingSeparator_should_properly_handle_UNC_paths()
    {
        // Arrange
        var uncPath = @"\\server\share\folder";
        var expected = @"\\server\share\folder" + Path.DirectorySeparatorChar;

        // Act
        var result = PathUtils.NormalizePathWithTrailingSeparator(uncPath);

        // Assert
        result.Should().Be(expected);
        result.Should().StartWith(@"\\server\share\");
        result.Should().EndWith(Path.DirectorySeparatorChar.ToString());
    }

    [Fact]
    public void NormalizePathWithTrailingSeparator_should_deduplicate_trailing_separators_if_multiple_exist()
    {
        // Arrange
        var expected = GenerateFolderPath(3, true);
        var pathWithMultipleSeparators = expected + Path.DirectorySeparatorChar + Path.DirectorySeparatorChar;

        // Act
        var result = PathUtils.NormalizePathWithTrailingSeparator(pathWithMultipleSeparators);

        // Assert
        result.Should().Be(expected);
        result.Should().EndWith(Path.DirectorySeparatorChar.ToString());
        result.Should().NotEndWith(new(Path.DirectorySeparatorChar, 2));
    }

    [Fact]
    public void NormalizePathWithoutTrailingSeparator_should_throw_exception_when_path_is_null()
    {
        // Act
        var act = () => PathUtils.NormalizePathWithoutTrailingSeparator(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("path");
    }

    [Fact]
    public void NormalizePathWithoutTrailingSeparator_should_throw_exception_when_path_is_empty()
    {
        // Act
        var act = () => PathUtils.NormalizePathWithoutTrailingSeparator(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>().WithParameterName("path").WithMessage("*empty*");
    }

    [Fact]
    public void NormalizePathWithoutTrailingSeparator_should_remove_trailing_separator_if_present()
    {
        // Arrange
        var pathWithTrailingSeparator = GenerateFolderPath(3, true);
        var expected = GenerateFolderPath(3, false);

        // Act
        var result = PathUtils.NormalizePathWithoutTrailingSeparator(pathWithTrailingSeparator);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void NormalizePathWithoutTrailingSeparator_should_handle_paths_with_no_trailing_separator()
    {
        // Arrange
        var pathWithoutTrailingSeparator = GenerateFolderPath(3, false);

        var expected = pathWithoutTrailingSeparator;

        // Act
        var result = PathUtils.NormalizePathWithoutTrailingSeparator(pathWithoutTrailingSeparator);

        // Assert
        result.Should().Be(expected);
        result.Should().NotEndWith(Path.DirectorySeparatorChar.ToString());
    }

    [Fact]
    public void NormalizePathWithoutTrailingSeparator_should_normalize_relative_paths_to_absolute_paths()
    {
        // Arrange
        var relativePath = "..\\folder";
        var expectedAbsolutePath = Path.GetFullPath(relativePath);

        // Act
        var result = PathUtils.NormalizePathWithoutTrailingSeparator(relativePath);

        // Assert
        result.Should().Be(expectedAbsolutePath);
        Path.IsPathRooted(result).Should().BeTrue();
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void NormalizePathWithoutTrailingSeparator_should_properly_handle_UNC_paths()
    {
        // Arrange
        var uncPath = @"\\server\share\folder\";
        var expected = @"\\server\share\folder";

        // Act
        var result = PathUtils.NormalizePathWithoutTrailingSeparator(uncPath);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void NormalizePathWithoutTrailingSeparator_should_remove_multiple_trailing_separators()
    {
        // Arrange
        var path = GenerateFolderPath(3, false);
        var pathWithMultipleTrailingSeparators = path + (Environment.OSVersion.IsWindows() ? @"\\\//" : "///");
        var expected = path;

        // Act
        var result = PathUtils.NormalizePathWithoutTrailingSeparator(pathWithMultipleTrailingSeparators);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void NormalizePathWithoutTrailingSeparator_should_handle_paths_with_both_types_of_directory_separators()
    {
        // Arrange
        var pathWithBothSeparators =
            $"c:{Path.DirectorySeparatorChar}myrootfolder{Path.AltDirectorySeparatorChar}subfolder1{Path.DirectorySeparatorChar}subfolder2{Path.AltDirectorySeparatorChar}subfolder3";

        var expected =
            $"c:{Path.DirectorySeparatorChar}myrootfolder{Path.DirectorySeparatorChar}subfolder1{Path.DirectorySeparatorChar}subfolder2{Path.DirectorySeparatorChar}subfolder3";

        // Act
        var result = PathUtils.NormalizePathWithoutTrailingSeparator(pathWithBothSeparators);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [SupportedOSPlatform(SupportedOS.Windows)]
    public void NormalizePathWithoutTrailingSeparator_should_maintain_consistent_behavior_across_platforms()
    {
        // Arrange
        var windowsStylePath = @"c:\folder1\folder2\";
        var unixStylePath = "c:/folder1/folder2/";

        // Act
        var windowsResult = PathUtils.NormalizePathWithoutTrailingSeparator(windowsStylePath);
        var unixResult = PathUtils.NormalizePathWithoutTrailingSeparator(unixStylePath);

        // Assert
        windowsResult.Should().Be(unixResult);
        windowsResult.Should().Be(@"c:\folder1\folder2");
    }

    [Fact]
    public void ToSafeFileName_should_replace_invalid_characters_with_underscores()
    {
        // Arrange
        var inputWithInvalidChars = "file:name*with?invalid/\\chars<>|";
        var expectedSafeFileName = Environment.OSVersion.IsWindows() ? "file_name_with_invalid__chars___" : "file:name*with?invalid_\\chars<>|";

        // Act
        var result = PathUtils.ToSafeFileName(inputWithInvalidChars);

        // Assert
        result.Should().Be(expectedSafeFileName);
        result.Should().NotContainAny(Path.GetInvalidFileNameChars().Select(c => c.ToString()));
    }

    [Fact]
    public void ToSafeFileName_should_throw_exception_when_input_is_null()
    {
        // Act
        Action act = () => PathUtils.ToSafeFileName(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("input");
    }

    [Fact]
    public void ToSafeFileName_should_handle_string_with_all_invalid_characters()
    {
        // Arrange
        var allInvalidChars = new string(Path.GetInvalidFileNameChars());
        var expectedSafeFileName = new string('_', allInvalidChars.Length);

        // Act
        var result = PathUtils.ToSafeFileName(allInvalidChars);

        // Assert
        result.Should().Be(expectedSafeFileName);
    }

    [Fact]
    public void ToSafeFileName_should_preserve_valid_characters()
    {
        // Arrange
        var validFileName = "validFileName123-_. ()";

        // Act
        var result = PathUtils.ToSafeFileName(validFileName);

        // Assert
        result.Should().Be(validFileName);
        result.Should().NotContainAny(Path.GetInvalidFileNameChars().Select(c => c.ToString()));
    }

    [Fact]
    public void ToSafeFileName_should_have_acceptable_performance_with_very_long_string()
    {
        // Arrange
        const int stringLength = 100_000;
        var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var invalidChars = Path.GetInvalidFileNameChars();

        // Create a string with a mix of valid and invalid characters
        var random = new Random(42); // Fixed seed for reproducibility
        var inputBuilder = new StringBuilder(stringLength);
        for (var i = 0; i < stringLength; i++)
        {
            // Add an invalid character every 10 characters on average
            if (i % 10 == 0 && invalidChars.Length > 0)
            {
                inputBuilder.Append(invalidChars[random.Next(invalidChars.Length)]);
            }
            else
            {
                inputBuilder.Append(validChars[random.Next(validChars.Length)]);
            }
        }

        var longInput = inputBuilder.ToString();

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = PathUtils.ToSafeFileName(longInput);
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Length.Should().Be(longInput.Length);
        result.Should().NotContainAny(Path.GetInvalidFileNameChars().Select(c => c.ToString()));

        // Performance assertion - should process in a reasonable time
        // Processing 100k characters should be fast on modern hardware
        // NOTE: This was increased to 100ms due to the increased time it takes to run tests with coverage
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100, "processing a 100K string should be reasonably fast");
    }

    [Fact]
    public void ToSafeFileName_should_handle_unicode_characters_correctly()
    {
        // Arrange
        var invalidChar1 = Path.GetInvalidFileNameChars().TakeRandom(1).First();
        var invalidChar2 = Path.GetInvalidFileNameChars().TakeRandom(1).First();
        var invalidChar3 = Path.GetInvalidFileNameChars().TakeRandom(1).First();
        var unicodeFileName = $"résumé_测试_{invalidChar1}ファ{invalidChar2}イル_🚀_{invalidChar3}πœ∑´®†¥¨ˆøπ";

        // Some Unicode characters are invalid in filenames on Windows
        // We'll make a reference with all valid Unicode chars preserved
        var expectedSafeFileName = new StringBuilder();
        var invalidChars = Path.GetInvalidFileNameChars().ToList();

        foreach (var c in unicodeFileName)
        {
            expectedSafeFileName.Append(invalidChars.Contains(c) ? '_' : c);
        }

        // Act
        var result = PathUtils.ToSafeFileName(unicodeFileName);

        // Assert
        result.Should().Be(expectedSafeFileName.ToString());
        result.Should().NotContainAny(Path.GetInvalidFileNameChars().Select(c => c.ToString()));
    }

    private static string GenerateFolderPath(int levels, bool pathSeparatorSuffix)
    {
        var sb = new StringBuilder();

        sb.Append(GenerateRoot());
        for (var level = 0; level < levels; level++)
        {
            if (level == 0)
            {
                sb.Append("root");
            }
            else
            {
#pragma warning disable CA1305 // Specify IFormatProvider - false/positive
                sb.Append(Path.DirectorySeparatorChar).Append($"folder{level}");
#pragma warning restore CA1305
            }
        }

        if (pathSeparatorSuffix)
        {
            sb.Append(Path.DirectorySeparatorChar);
        }

        var result = sb.ToString();
        if (!pathSeparatorSuffix)
        {
            result.Should().NotEndWith(Path.DirectorySeparatorChar.ToString()).And.NotEndWith(Path.AltDirectorySeparatorChar.ToString());
        }

        return result;
    }

    private static string GenerateRoot()
    {
        if (Environment.OSVersion.IsWindows())
        {
            return @"c:\";
        }

        return new([Path.VolumeSeparatorChar]);
    }
}
