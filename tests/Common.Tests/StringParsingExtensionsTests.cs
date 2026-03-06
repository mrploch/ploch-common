namespace Ploch.Common.Tests;

public class StringParsingExtensionsTests
{
    [Fact]
    public void ParseToBool_should_return_null_for_null_or_whitespace()
    {
        string nullString = null;
        nullString.ParseToBool().Should().BeNull();
        "".ParseToBool().Should().BeNull();
        "   ".ParseToBool().Should().BeNull();
    }

    [Theory]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData("TrUe")]
    [InlineData(" true ")]
    public void ParseToBool_should_parse_true_values_case_insensitively(string input)
    {
        input.ParseToBool().Should().BeTrue();
    }

    [Theory]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("FaLsE")]
    [InlineData(" false ")]
    public void ParseToBool_should_parse_false_values_case_insensitively(string input)
    {
        input.ParseToBool().Should().BeFalse();
    }

    [Theory]
    [InlineData("yes")]
    [InlineData("no")]
    [InlineData("1")]
    [InlineData("0")]
    [InlineData("maybe")]
    [InlineData("not a bool")]
    public void ParseToBool_should_return_null_for_invalid_values(string input)
    {
        input.ParseToBool().Should().BeNull();
    }

    [Fact]
    public void ParseToInt32_should_return_null_for_null_or_whitespace()
    {
        string nullString = null;
        nullString.ParseToInt32().Should().BeNull();
        "".ParseToInt32().Should().BeNull();
        "   ".ParseToInt32().Should().BeNull();
    }

    [Theory]
    [InlineData("0", 0)]
    [InlineData("-42", -42)]
    [InlineData("  123  ", 123)]
    [InlineData("2147483647", int.MaxValue)]
    public void ParseToInt32_should_parse_valid_integers(string input, int expected)
    {
        input.ParseToInt32().Should().Be(expected);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.34")] // decimal string should not parse
    [InlineData("2147483648")] // overflow beyond int.MaxValue
    public void ParseToInt32_should_return_null_for_invalid_or_overflow_values(string input)
    {
        input.ParseToInt32().Should().BeNull();
    }

    [Fact]
    public void ParseToInt64_should_return_null_for_null_or_whitespace()
    {
        string nullString = null;
        nullString.ParseToInt64().Should().BeNull();
        "".ParseToInt64().Should().BeNull();
        "   ".ParseToInt64().Should().BeNull();
    }

    [Theory]
    [InlineData("0", 0L)]
    [InlineData("-42", -42L)]
    [InlineData("  123  ", 123L)]
    [InlineData("9223372036854775807", long.MaxValue)]
    public void ParseToInt64_should_parse_valid_integers(string input, long expected)
    {
        input.ParseToInt64().Should().Be(expected);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("12.34")] // decimal string should not parse
    [InlineData("9223372036854775808")] // overflow beyond long.MaxValue
    public void ParseToInt64_should_return_null_for_invalid_or_overflow_values(string input)
    {
        input.ParseToInt64().Should().BeNull();
    }

    [Fact]
    public void ParseToInt64_should_parse_numbers_that_do_not_fit_Int32_but_fit_Int64()
    {
        const string value = "2147483648"; // int.MaxValue + 1

        value.ParseToInt32().Should().BeNull();
        value.ParseToInt64().Should().Be(2147483648L);
    }
}
