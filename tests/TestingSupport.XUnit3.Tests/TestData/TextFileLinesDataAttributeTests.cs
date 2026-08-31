using FluentAssertions;
using Ploch.TestingSupport.XUnit3.TestData;
using Xunit;

namespace Ploch.TestingSupport.XUnit3.Tests.TestData;

#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes
public class TextFileLinesDataAttributeTests
{
    // The fixture ends with whitespace-only lines (and the empty line every trailing newline produces), so that the
    // removeEmptyEntries option has something to remove and a whitespace-only line cannot slip through unnoticed
    // (issue #309). They differ from each other because this attribute pre-enumerates test cases, and two identical
    // rows would collide on the generated test-case id.
    [Theory]
    [TextFileLinesData("TestData/TextFileLinesDataAttributeTests_TestData.txt")]
    public void TestTextFileLinesDataAttribute_should_provide_lines_from_the_specified_text_file(string line)
    {
        if (!string.IsNullOrWhiteSpace(line))
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Theory]
    [TextFileLinesData("TestData/TextFileLinesDataAttributeTests_TestData.txt", true)]
    public void TestTextFileLinesDataAttribute_with_removeEmpty_option_should_provide_lines_from_the_specified_text_file_excluding_blank_lines(string line)
    {
        line.Should().NotBeNullOrWhiteSpace();
        Guid.Parse(line).Should().NotBeEmpty();
    }
}
#pragma warning restore xUnit1003
