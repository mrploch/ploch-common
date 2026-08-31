using System.Reflection;
using FluentAssertions;
using Ploch.TestingSupport.TestData;
using Xunit;
using Xunit.Sdk;

namespace Ploch.TestingSupport.Tests;

/// <summary>
///   Covers how <see cref="TextFileDataAttribute" /> resolves the path it was given (issue #309).
/// </summary>
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
    public async Task GetData_should_use_an_absolute_path_unchanged()
    {
        var absolutePath = Path.Combine(AppContext.BaseDirectory, RelativeTestDataFilePath);

        var rowCount = await WithWorkingDirectoryAsync(Path.GetTempPath(), () => GetRowCountAsync(absolutePath));

        rowCount.Should().BePositive();
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
    ///   Runs <paramref name="action" /> with the process working directory temporarily pointed elsewhere.
    /// </summary>
    /// <remarks>
    ///   Nothing in this assembly resolves a path against the working directory - the data attributes anchor to the
    ///   assembly directory and the tests use absolute paths - so a test running in parallel cannot observe the change.
    ///   The original directory is restored in a finally block, so no state leaks even when the action throws.
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
