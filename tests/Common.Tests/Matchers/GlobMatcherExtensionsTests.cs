using Microsoft.Extensions.FileSystemGlobbing;
using Ploch.Common.Matchers;

namespace Ploch.Common.Tests.Matchers;

public class GlobMatcherExtensionsTests
{
    [Theory]
    [InlineData("ddd abc 123", true)]
    [InlineData("def 123", true)]
    [InlineData("ddd ghi 123", false)]
    [InlineData("ddd xyz 123", false)]
    public void IncludePatterns_ExcludePatterns_should_add_include_and_exclude_patterns_in_a_fluent_manner_and_return_matcher(string input,
                                                                                                                              bool expectedIsMatch)
    {
        var matcher1 = new Matcher();
        var matcher = matcher1.IncludePatterns("*abc*", "def**").ExcludePatterns("**xyz*", "*ghi*");

        matcher1.Should().Be(matcher);
        matcher.Match(input).HasMatches.Should().Be(expectedIsMatch);
    }

    [Theory]
    [InlineData("ddd abc 123", true)]
    [InlineData("def 123", true)]
    [InlineData("ddd ghi 123", false)]
    [InlineData("ddd xyz 123", false)]
    public void IncludePatterns_ExcludePatterns_IEnumerable_should_add_include_and_exclude_patterns_in_a_fluent_manner_and_return_matcher(string input,
                                                                                                                                          bool expectedIsMatch)
    {
        var matcher1 = new Matcher();
        var matcher = matcher1.IncludePatterns(new List<string>(["*abc*", "def**"])).ExcludePatterns(new List<string>(["**xyz*", "*ghi*"]));

        matcher1.Should().Be(matcher);
        matcher.Match(input).HasMatches.Should().Be(expectedIsMatch);
    }
}
