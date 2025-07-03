using FluentAssertions;
using Ploch.Common;
using Ploch.TestingSupport.TestData;
using Ploch.TestingSupport.TestOrdering;

namespace Ploch.TestingSupport.Tests;

#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes
[TestCaseOrderer($"{nameof(Ploch)}.{nameof(TestingSupport)}.{nameof(PriorityOrderer)}", $"{nameof(Ploch)}.{nameof(TestingSupport)}")]
public class TextFileLinesDataAttributeTests
{
    private static readonly List<string> TestDataLines = new();
    private static readonly List<string> TestDataLinesWithoutEmptyLines = new();

    [Theory]
    [TextFileLinesData("TestData/TextFileLinesDataAttributeTests_TestData.txt")] [TestPriority(0)]
    public void TestTextFileLinesDataAttribute_should_provide_lines_from_the_specified_text_file(string line)
    {
        TestDataLines.Add(line);

        // Test the lines
        if (line.IsNotNullOrEmpty())
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Fact]
    [TestPriority(1)]
    public void TestTextFileLinesDataAttribute_TestDataLines_should_have_correct_count()
    {
        TestDataLines.Should().HaveCount(101);
    }

    [Theory]
    [TextFileLinesData("TestData/TextFileLinesDataAttributeTests_TestData.txt", true)] [TestPriority(2)]
    public void TestTextFileLinesDataAttribute_with_removeEmpty_option_should_provide_lines_from_the_specified_text_file_excluding_blank_lines(string line)
    {
        TestDataLinesWithoutEmptyLines.Add(line);

        // Test the lines
        if (line.IsNotNullOrEmpty())
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Fact]
    [TestPriority(3)]
    public void TestTextFileLinesDataAttribute_with_removeEmpty_option__TestDataLinesWithoutEmptyLines_should_have_correct_count()
    {
        TestDataLinesWithoutEmptyLines.Should().HaveCount(100);
    }
}
#pragma warning restore xUnit1003
