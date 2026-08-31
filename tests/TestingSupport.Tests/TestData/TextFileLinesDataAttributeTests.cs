using FluentAssertions;
using Ploch.TestingSupport.TestData;
using Xunit;
using Xunit.Sdk;

namespace Ploch.TestingSupport.Tests;

#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes

public class TextFileLinesDataAttributeTests
{
    // A relative path is fine as an attribute argument (which must be a compile-time constant):
    // TextFileDataAttribute anchors it to the test assembly's directory, where the file is copied.
    // See issue #309.
    private const string TestDataFilePath = "TestData/TextFileLinesDataAttributeTests_TestData.txt";

    // The fixture holds this many GUID lines plus a few deliberately blank and whitespace-only ones,
    // so that the removeEmptyEntries option has something to remove.
    private const int GuidLineCount = 100;

    [Theory]
    [TextFileLinesData(TestDataFilePath)]
    public void TextFileLinesDataAttribute_should_provide_lines_from_the_specified_text_file(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Theory]
    [TextFileLinesData(TestDataFilePath, true)]
    public void TextFileLinesDataAttribute_with_removeEmptyEntries_should_provide_only_non_blank_lines(string line)
    {
        line.Should().NotBeNullOrWhiteSpace();
        Guid.Parse(line).Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetData_should_return_every_line_including_the_blank_ones()
    {
        var lines = await GetLinesAsync(new TextFileLinesDataAttribute(TestDataFilePath));

        lines.Should().HaveCountGreaterThan(GuidLineCount);
        lines.Should().Contain(line => string.IsNullOrWhiteSpace(line));
    }

    [Fact]
    public async Task GetData_with_removeEmptyEntries_should_exclude_the_blank_lines()
    {
        var allLines = await GetLinesAsync(new TextFileLinesDataAttribute(TestDataFilePath));
        var nonBlankLines = await GetLinesAsync(new TextFileLinesDataAttribute(TestDataFilePath, true));

        nonBlankLines.Should().HaveCount(GuidLineCount);
        nonBlankLines.Should().HaveCountLessThan(allLines.Count);
        nonBlankLines.Should().OnlyContain(line => !string.IsNullOrWhiteSpace(line));
    }

    private static async Task<IReadOnlyList<string>> GetLinesAsync(TextFileLinesDataAttribute attribute)
    {
        // GetData only needs a MethodInfo with a single string parameter; the theory above provides one.
        var testMethod = typeof(TextFileLinesDataAttributeTests)
            .GetMethod(nameof(TextFileLinesDataAttribute_should_provide_lines_from_the_specified_text_file))!;

        await using var disposalTracker = new DisposalTracker();
        var rows = await attribute.GetData(testMethod, disposalTracker);

        return rows.Select(row => (string)row.GetData()[0]!).ToList();
    }
}
#pragma warning restore xUnit1003
