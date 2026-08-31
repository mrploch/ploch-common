using System.Reflection;
using FluentAssertions;
using Ploch.TestingSupport.XUnit3.TestData;
using Xunit;
using Xunit.Sdk;

namespace Ploch.TestingSupport.XUnit3.Tests.TestData;

/// <summary>
///     Covers how <see cref="TextFileDataAttribute" /> resolves the path it was given (issue #309).
/// </summary>
[Collection(WorkingDirectoryCollection.Name)]
public class TextFileDataAttributeTests
{
    private const string RelativeTestDataFilePath = "TestData/TextFileLinesDataAttributeTests_TestData.txt";

    [Fact]
    public async Task GetData_should_resolve_a_relative_path_against_the_assembly_directory()
    {
        var expectedCount = await GetRowCountAsync(Path.Combine(AppContext.BaseDirectory, RelativeTestDataFilePath));

        var actualCount = await WithWorkingDirectoryAsync(Path.GetTempPath(), () => GetRowCountAsync(RelativeTestDataFilePath));

        actualCount.Should().BePositive().And.Be(expectedCount);
    }

    [Fact]
    public async Task GetData_should_use_a_fully_qualified_path_unchanged()
    {
        var fullyQualifiedPath = Path.Combine(AppContext.BaseDirectory, RelativeTestDataFilePath);

        var rowCount = await WithWorkingDirectoryAsync(Path.GetTempPath(), () => GetRowCountAsync(fullyQualifiedPath));

        rowCount.Should().BePositive();
    }

    /// <summary>
    ///     A path such as <c>"\TestData\cases.txt"</c> is rooted but not fully qualified on Windows - it names the current
    ///     drive, so <see cref="Path.IsPathRooted(string)" /> is the wrong test to decide whether to anchor it (issue #309).
    /// </summary>
    [Theory]
    [InlineData("/" + RelativeTestDataFilePath)]
    [InlineData(@"\TestData\TextFileLinesDataAttributeTests_TestData.txt")]
    public async Task GetData_should_anchor_a_drive_rooted_path_to_the_assembly_directory(string driveRootedPath)
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "A leading separator only means \"current drive\" on Windows; on Unix it is a fully qualified path.");

        var expectedCount = await GetRowCountAsync(Path.Combine(AppContext.BaseDirectory, RelativeTestDataFilePath));

        var actualCount = await WithWorkingDirectoryAsync(Path.GetTempPath(), () => GetRowCountAsync(driveRootedPath));

        actualCount.Should().BePositive().And.Be(expectedCount);
    }

    /// <summary>
    ///     A drive-relative path such as <c>"C:cases.txt"</c> is rooted too, yet it resolves against that drive's current
    ///     directory - ambient state the test host controls - so it must be anchored as well (issue #309).
    /// </summary>
    [Fact]
    public async Task GetData_should_anchor_a_drive_relative_path_to_the_assembly_directory()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Drive-relative paths only exist on Windows.");

        var expectedCount = await GetRowCountAsync(Path.Combine(AppContext.BaseDirectory, RelativeTestDataFilePath));

        var actualCount = await WithWorkingDirectoryAsync(Path.GetTempPath(), () => GetRowCountAsync("C:" + RelativeTestDataFilePath));

        actualCount.Should().BePositive().And.Be(expectedCount);
    }

    [Fact]
    public async Task GetData_should_use_a_unix_rooted_path_unchanged()
    {
        Assert.SkipWhen(OperatingSystem.IsWindows(), "On Windows a leading separator names the current drive rather than a fully qualified path.");

        var message = await WithWorkingDirectoryAsync(
            Path.GetTempPath(),
            async () =>
            {
                var act = () => GetRowCountAsync("/no-such-directory/no-such-test-data-file.txt");

                var assertion = await act.Should().ThrowAsync<ArgumentException>();

                return assertion.Which.Message;
            });

        message.Should().Contain("/no-such-directory/no-such-test-data-file.txt").And.NotContain(AppContext.BaseDirectory);
    }

    [Fact]
    public async Task GetData_should_name_the_resolved_path_when_the_file_is_missing()
    {
        var message = await WithWorkingDirectoryAsync(
            Path.GetTempPath(),
            async () =>
            {
                var act = () => GetRowCountAsync("TestData/no-such-test-data-file.txt");

                var assertion = await act.Should().ThrowAsync<ArgumentException>();

                return assertion.Which.Message;
            });

        message.Should().Contain(AppContext.BaseDirectory);
    }

    /// <summary>
    ///     Runs <paramref name="action" /> with the process working directory temporarily pointed elsewhere.
    /// </summary>
    /// <remarks>
    ///     The working directory is process-wide state, so this class is pinned to
    ///     <see cref="WorkingDirectoryCollection" />, which xUnit never runs in parallel with another collection. The
    ///     original directory is restored in a finally block, so no state leaks even when the action throws.
    /// </remarks>
    private static async Task<T> WithWorkingDirectoryAsync<T>(string workingDirectory, Func<Task<T>> action)
    {
        var originalWorkingDirectory = Directory.GetCurrentDirectory();
        Directory.SetCurrentDirectory(workingDirectory);
        try
        {
            return await action();
        }
        finally
        {
            Directory.SetCurrentDirectory(originalWorkingDirectory);
        }
    }

    private static async Task<int> GetRowCountAsync(string filePath)
    {
        // GetData only needs a MethodInfo with a single string parameter - this helper has one.
        var testMethod = typeof(TextFileDataAttributeTests).GetMethod(nameof(GetRowCountAsync), BindingFlags.NonPublic | BindingFlags.Static)!;

        await using var disposalTracker = new DisposalTracker();
        var rows = await new TextFileLinesDataAttribute(filePath, true).GetData(testMethod, disposalTracker);

        return rows.Count;
    }
}
