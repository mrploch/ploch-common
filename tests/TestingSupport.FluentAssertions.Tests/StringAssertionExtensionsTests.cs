namespace Ploch.TestingSupport.FluentAssertions.Tests;

public class StringAssertionExtensionsTests(ITestOutputHelper output)
{
    [Fact]
    public void ContainAllEquivalentOf_should_throw_if_no_values_are_provided()
    {
        "a message".Should()
                   .Invoking(assertions => assertions.ContainAllEquivalentOf())
                   .Should()
                   .Throw<XunitException>()
                   .WithMessage("You have to provide at least one value to check for.");
    }

    [Theory]
    [AutoData]
    public void ContainAllEquivalentOf_should_pass_if_all_of_the_strings_are_found(string? str1, string str2)
    {
        $"message with {str1} and {str2}".Should().ContainAllEquivalentOf(str1, str2);
    }

    [Theory]
    [AutoData]
    public void ContainAllEquivalentOf_should_pass_if_all_of_the_strings_are_found_in_any_case(string str1, string str2)
    {
        $"message with {str1.ToLower()} and {str2.ToUpper()}".Should().ContainAllEquivalentOf(str1.ToUpper(), str2.ToLower());
    }

    [Theory]
    [AutoData]
    public void ContainAllEquivalentOf_should_throw_if_one_of_the_strings_is_not_found(string? str1, string str2, string notFound)
    {
        $"message with {str1} and {str2}".Should()
                                         .Invoking(assertions => assertions.ContainAllEquivalentOf(str1, str2, notFound))
                                         .Should()
                                         .Throw<XunitException>()
                                         .WithMessage($"Expected * contain * but *{notFound}*not found*");
    }
}
