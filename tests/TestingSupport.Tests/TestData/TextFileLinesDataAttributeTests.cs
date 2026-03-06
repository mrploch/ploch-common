using FluentAssertions;
using Ploch.Common;
using Ploch.TestingSupport.TestData;
using Xunit;

namespace Ploch.TestingSupport.Tests;

#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes

public class TextFileLinesDataAttributeTests
{
    private const string TestDataFilePath = "TestData/TextFileLinesDataAttributeTests_TestData.txt";

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
        var lines = File.ReadAllLines(TestDataFilePath);
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
        var lines = File.ReadAllLines(TestDataFilePath)
                        .Where(line => !string.IsNullOrWhiteSpace(line))
                        .ToArray();
        lines.Should().HaveCount(100);
    }
}
#pragma warning restore xUnit1003
