using FluentAssertions;
using Ploch.Common;
using Ploch.TestingSupport.XUnit3.TestData;
using Xunit;

namespace Ploch.TestingSupport.XUnit3.Tests.TestData;

#pragma warning disable xUnit1003 // Theory must have test data - doesn't recognize custom data attributes
public class TextFileLinesDataAttributeTests
{
    [Theory]
    [TextFileLinesData("TestData/TextFileLinesDataAttributeTests_TestData.txt")]
    public void TestTextFileLinesDataAttribute_should_provide_lines_from_the_specified_text_file(string line)
    {
        if (line.IsNotNullOrEmpty())
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }

    [Theory]
    [TextFileLinesData("TestData/TextFileLinesDataAttributeTests_TestData.txt", true)]
    public void TestTextFileLinesDataAttribute_with_removeEmpty_option_should_provide_lines_from_the_specified_text_file_excluding_blank_lines(string line)
    {
        if (line.IsNotNullOrEmpty())
        {
            Guid.Parse(line).Should().NotBeEmpty();
        }
    }
}
#pragma warning restore xUnit1003
