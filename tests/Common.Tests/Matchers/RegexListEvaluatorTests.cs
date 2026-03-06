using System.Text.RegularExpressions;
using Ploch.Common.Matchers;

namespace Ploch.Common.Tests.Matchers;

public class RegexListEvaluatorTests
{
    [Fact]
    public void IsMatch_should_return_true_when_input_matches_any_regex()
    {
        // Arrange
        var regexList = new List<string> { "^foo", "bar$", "baz" };
        var sut = new RegexListEvaluator(regexList);

        // Act
        var result = sut.IsMatch("hello bar");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsMatch_should_return_false_when_input_matches_no_regex()
    {
        // Arrange
        var regexList = new List<string> { "^foo", "bar$", "baz" };
        var sut = new RegexListEvaluator(regexList);

        // Act
        var result = sut.IsMatch("qux");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsMatch_should_respect_ignore_case_parameter()
    {
        // Arrange
        var regexList = new List<string> { "abc" };
        var sutIgnoreCase = new RegexListEvaluator(regexList, ignoreCase: true);
        var sutCaseSensitive = new RegexListEvaluator(regexList, ignoreCase: false);

        // Act & Assert
        sutIgnoreCase.IsMatch("ABC").Should().BeTrue();
        sutCaseSensitive.IsMatch("ABC").Should().BeFalse();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsMatch_should_return_null_value_match_result_when_input_is_null(bool nullValueMatchResult)
    {
        // Arrange
        var regexList = new List<string> { "abc" };
        var sut = new RegexListEvaluator(regexList, nullValueMatchResult);

        // Act
        var result = sut.IsMatch(null);

        // Assert
        result.Should().Be(nullValueMatchResult);
    }

    [Fact]
    public void IsMatch_should_return_false_when_regex_list_is_empty()
    {
        // Arrange
        var regexList = new List<string>();
        var sut = new RegexListEvaluator(regexList);

        // Act
        var result = sut.IsMatch("anything");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Constructor_should_throw_exception_when_given_invalid_regex_patterns()
    {
        // Arrange
        var invalidRegexList = new List<string> { "[invalid", "foo" };

        // Act
        var act = () => new RegexListEvaluator(invalidRegexList);

        // Assert
        act.Should().Throw<RegexParseException>();
    }
}
