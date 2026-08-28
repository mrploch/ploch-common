using FluentAssertions;
using Ploch.Common;
using Ploch.TestingSupport.TestData;
using Xunit;

namespace Ploch.TestingSupport.Tests;

#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes

public class TextFileLinesDataAttributeTests
{
    // Attribute arguments must be compile-time constants, so the relative form is kept for the
    // [TextFileLinesData] usages below. Those go through TextFileDataAttribute, which resolves
    // against the working directory rather than the assembly; correcting that is a behaviour
    // change in a published package and is tracked separately by issue #309.
    private const string TestDataFilePath = "TestData/TextFileLinesDataAttributeTests_TestData.txt";

    // Direct reads resolve against the assembly directory (where the file is copied) rather than
    // the process working directory, which the test runner is free to change. See issue #299.
    private static readonly string ResolvedTestDataFilePath = Path.Combine(AppContext.BaseDirectory, TestDataFilePath);

    [Theory]
    [TextFileLinesData(TestDataFilePath)]
    public void TestTextFileLinesDataAttribute_should_provide_lines_from_the_specified_text_file(string line)
    {
        if (line.IsNotNullOrEmpty())
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Fact]
    public void TestTextFileLinesDataAttribute_TestDataLines_should_have_correct_count()
    {
        var lines = File.ReadAllLines(ResolvedTestDataFilePath);
        lines.Should().HaveCount(100);
    }

    [Theory]
    [TextFileLinesData(TestDataFilePath, true)]
    public void TestTextFileLinesDataAttribute_with_removeEmpty_option_should_provide_lines_from_the_specified_text_file_excluding_blank_lines(string line)
    {
        if (line.IsNotNullOrEmpty())
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Fact]
    public void TestTextFileLinesDataAttribute_with_removeEmpty_option__TestDataLinesWithoutEmptyLines_should_have_correct_count()
    {
        var lines = File.ReadAllLines(ResolvedTestDataFilePath)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToArray();
        lines.Should().HaveCount(100);
    }
}
#pragma warning restore xUnit1003
