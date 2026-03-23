using System.Globalization;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests;

public class IsInExtensionsTests
{
    private static readonly StringComparer IgnoringSymbolsComparer = StringComparer.Create(CultureInfo.InvariantCulture, CompareOptions.IgnoreSymbols);

    [Fact]
    public void In_should_return_true_if_string_is_in_the_list()
    {
        "test".In("t1", "t2", "test").Should().BeTrue();
    }

    [Fact]
    public void In_should_return_false_if_string_is_in_the_list()
    {
        "test".In("t1", "t2").Should().BeFalse();
    }

    [Theory]
    [AutoMockData]
    public void NotIn_should_return_true_if_string_is_in_the_list(List<string> strings)
    {
        strings.Add("test");
        "test".NotIn("t1", "t2", "test").Should().BeFalse();
    }

    [Fact]
    public void NotIn_should_return_false_if_string_is_in_the_list()
    {
        "test".NotIn("t1", "t2").Should().BeTrue();
    }

    [Theory]
    [InlineData("test", true, "t1", "t2", "test")]
    [InlineData("test", true, "t1", "t2", "TeSt")]
    [InlineData("test", true, null, null, null, null, "test")]
    [InlineData("t", true, "t", "t", "t")]
    [InlineData(null, true, "t1", "t2", "t3", null, "t4")]
    [InlineData("", true, "t1", "t2", "t3", "", "t4")]
    [InlineData(null, true, null, null, null, null, null)]
    [InlineData("test", false, "t1", "t2", "Te_St")]
    [InlineData("test", false, "t1", "t2", "t3")]
    [InlineData("", false, "t1", "t2", "t3", null, "t4")]
    public void In_should_return_if_matching_string_is_found_with_case_insensitive_matching(string? value, bool expectedResult, params string?[] strings)
    {
        value.In(StringComparer.OrdinalIgnoreCase, strings).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("test", true, "t1", "t2", "te!st")]
    [InlineData("test", false, "t1", "t2", "TeSt")]
    [InlineData("test", true, null, null, null, null, "te%st")]
    [InlineData("t", true, "t", "t", "t")]
    [InlineData(null, true, "t1", "t2", "t3", null, "t4")]
    [InlineData("", true, "t1", "t2", "t3", "", "t4")]
    [InlineData(null, true, null, null, null, null, null)]
    [InlineData("test", false, "t1", "t2", "Te_St")]
    [InlineData("test", false, "t1", "t2", "t3")]
    [InlineData("some sentence, with symbols. Another test.", true, "t1", "t2", "t3", "somesentencewithsymbolsAnothertest.")]
    [InlineData("", false, "t1", "t2", "t3", null, "t4")]
    public void In_should_return_if_matching_string_is_found_with_case_sensitive_ignoring_symbols_matching(string? value,
                                                                                                           bool expectedResult,
                                                                                                           params string?[] strings)
    {
        value.In(IgnoringSymbolsComparer, strings).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(TestEnum.FirstValue, true)]
    [InlineData(TestEnum.ThirdValue, false)]
    public void In_should_return_expected_result_for_enum_values(TestEnum value, bool expectedResult)
    {
        value.In(TestEnum.FirstValue, TestEnum.SecondValue).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("test", false, "t1", "t2", "test")]
    [InlineData("test", false, "t1", "t2", "TeSt")]
    [InlineData("test", false, null, null, null, null, "test")]
    [InlineData(null, false, "t1", "t2", "t3", null, "t4")]
    [InlineData("test", true, "t1", "t2", "t3")]
    [InlineData("test", true, "t1", "t2", "Te_St")]
    [InlineData("", true, "t1", "t2", "t3", null, "t4")]
    public void NotIn_should_return_if_matching_string_is_not_found_with_case_insensitive_matching(string? value, bool expectedResult, params string?[] strings)
    {
        value.NotIn(StringComparer.OrdinalIgnoreCase, strings).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData("test", false, "t1", "t2", "te!st")]
    [InlineData("test", true, "t1", "t2", "TeSt")]
    [InlineData("test", false, null, null, null, null, "te%st")]
    [InlineData(null, false, "t1", "t2", "t3", null, "t4")]
    [InlineData("test", true, "t1", "t2", "t3")]
    [InlineData("", true, "t1", "t2", "t3", null, "t4")]
    public void NotIn_should_return_if_matching_string_is_not_found_with_case_sensitive_ignoring_symbols_matching(string? value,
                                                                                                                  bool expectedResult,
                                                                                                                  params string?[] strings)
    {
        value.NotIn(IgnoringSymbolsComparer, strings).Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(TestEnum.FirstValue, false)]
    [InlineData(TestEnum.ThirdValue, true)]
    public void NotIn_should_return_expected_result_for_enum_values(TestEnum value, bool expectedResult)
    {
        value.NotIn(TestEnum.FirstValue, TestEnum.SecondValue).Should().Be(expectedResult);
    }

    [Fact]
    public void In_with_IEnumerable_should_return_true_if_value_is_in_the_list()
    {
        var values = new List<string> { "t1", "t2", "test" };
        "test".In(values).Should().BeTrue();
    }

    [Fact]
    public void In_with_IEnumerable_should_return_false_if_value_is_not_in_the_list()
    {
        var values = new List<string> { "t1", "t2" };
        "test".In(values).Should().BeFalse();
    }

    [Fact]
    public void In_with_IEnumerable_should_handle_null_values()
    {
        var values = new List<string?> { "t1", null, "t2" };
        ((string?)null).In(values).Should().BeTrue();
    }

    [Fact]
    public void NotIn_with_IEnumerable_should_return_true_if_value_is_not_in_the_list()
    {
        var values = new List<string> { "t1", "t2" };
        "test".NotIn(values).Should().BeTrue();
    }

    [Fact]
    public void NotIn_with_IEnumerable_should_return_false_if_value_is_in_the_list()
    {
        var values = new List<string> { "t1", "t2", "test" };
        "test".NotIn(values).Should().BeFalse();
    }

    [Fact]
    public void In_with_IComparer_and_IEnumerable_should_return_true_if_matching_value_is_found()
    {
        var values = new List<string> { "t1", "t2", "TeSt" };
        "test".In(StringComparer.OrdinalIgnoreCase, values).Should().BeTrue();
    }

    [Fact]
    public void In_with_IComparer_and_IEnumerable_should_return_false_if_matching_value_is_not_found()
    {
        var values = new List<string> { "t1", "t2", "t3" };
        "test".In(StringComparer.OrdinalIgnoreCase, values).Should().BeFalse();
    }

    [Fact]
    public void NotIn_with_IComparer_and_IEnumerable_should_return_true_if_matching_value_is_not_found()
    {
        var values = new List<string> { "t1", "t2", "t3" };
        "test".NotIn(StringComparer.OrdinalIgnoreCase, values).Should().BeTrue();
    }

    [Fact]
    public void NotIn_with_IComparer_and_IEnumerable_should_return_false_if_matching_value_is_found()
    {
        var values = new List<string> { "t1", "t2", "TeSt" };
        "test".NotIn(StringComparer.OrdinalIgnoreCase, values).Should().BeFalse();
    }
}
