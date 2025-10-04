using Ploch.Common.Matchers;

namespace Ploch.Common.Tests.Matchers;

public class GlobEvaluatorTests
{
    [Theory]
    [InlineData("test1-included-suffix", true)]
    [InlineData("test1-abc-123", true)]
    [InlineData("test1-123", false)]
    [InlineData("test2-abc-123", true)]
    [InlineData("test2-abc-def-123-excluded-suffix", false)]
    [InlineData("", false)]
    public void IsMatch_should_return_true_if_string_matches_include_pattern(string value, bool expectedIsMatch)
    {
        var sut = new GlobEvaluator(["*abc*", "*included-suffix"], ["*excluded-suffix"]);

        sut.IsMatch(value).Should().Be(expectedIsMatch);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsMatch_should_return_configured_nullMatchResult_when_input_is_null(bool nullMatchResult)
    {
        // Arrange
        var sut = new GlobEvaluator(["*pattern*"], ["*excluded*"], nullMatchResult);

        // Act & Assert
        sut.IsMatch(null).Should().Be(nullMatchResult);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsMatch_should_return_true_when_input_is_empty_string_and_pattern_matches_empty_string(bool emptyMatchResult)
    {
        // Arrange
        var sut = new GlobEvaluator(["*"], [], emptyMatchResult: emptyMatchResult);

        // Act & Assert
        sut.IsMatch(string.Empty).Should().Be(emptyMatchResult);
    }

    [Theory]
    [InlineData("TestPattern", true, StringComparison.OrdinalIgnoreCase)]
    [InlineData("TestPattern", true, StringComparison.Ordinal)]
    [InlineData("testpattern", true, StringComparison.OrdinalIgnoreCase)]
    [InlineData("testpattern", false, StringComparison.Ordinal)]
    public void IsMatch_should_respect_case_sensitivity_according_to_comparison_type(string value, bool expectedResult, StringComparison comparisonType)
    {
        // Arrange
        var pattern = "TestPattern";
        var sut = new GlobEvaluator([pattern], [], comparisonType: comparisonType);

        // Act & Assert
        sut.IsMatch(value).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("file-with-excluded-part", false)]
    [InlineData("normal-file", true)]
    [InlineData("another-excluded-content-file", false)]
    public void IsMatch_should_correctly_anything_include_pattern_and_only_exclude_patterns(string value, bool expectedResult)
    {
        // Arrange
        var sut = new GlobEvaluator(["*"], ["*excluded*"]);

        // Act & Assert
        sut.IsMatch(value).Should().Be(expectedResult);
    }

    [Fact]
    public void IsMatch_should_correctly_process_very_long_input_strings()
    {
        // Arrange
        var longString = new string('a', 10000) + "match-pattern" + new string('b', 10000);
        var sut = new GlobEvaluator(["*match-pattern*"], []);

        // Act & Assert
        sut.IsMatch(longString).Should().BeTrue();
        sut.IsMatch(new('x', 20000)).Should().BeFalse();
    }
}
