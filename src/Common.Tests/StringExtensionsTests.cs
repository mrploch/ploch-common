using System.Globalization;
using System.Text;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.Collections;
using Xunit;

namespace Ploch.Common.Tests;

public class StringExtensionsTests
{
    [Theory]
    [AutoMockData]
    public void IsNullOrEmptyTest(string str)
    {
        str.IsNullOrEmpty().Should().BeFalse();
        string nullString = null;
        nullString.IsNullOrEmpty().Should().BeTrue();

        "".IsNullOrEmpty().Should().BeTrue();
    }

    [Theory]
    [AutoMockData]
    public void ToBase64String_should_correctly_encode_using_utf8_as_default_encoding(string str)
    {
        var base64StringDefaultEncoding = str.ToBase64String();
        var base64StringUtf8 = str.ToBase64String(Encoding.UTF8);
        
        var decodedDefaultEncoding = Encoding.UTF8.GetString(Convert.FromBase64String(base64StringDefaultEncoding));
        var decodedUtf8 = Encoding.UTF8.GetString(Convert.FromBase64String(base64StringUtf8));

        decodedDefaultEncoding.Should().BeEquivalentTo(decodedUtf8).And.Be(str);
    }
    
    [Theory]
    [AutoMockData]
    public void ToBase64String_should_correctly_encode_with_encoding(string str)
    {
        var base64StringUtf32 = str.ToBase64String(Encoding.UTF32);
        var base64StringUtf8 = str.ToBase64String(Encoding.UTF8);
        
        base64StringUtf32.Should().NotBeEquivalentTo(base64StringUtf8);

        var decodedUtf32 = Encoding.UTF32.GetString(Convert.FromBase64String(base64StringUtf32));
        var decodedUtf8 = Encoding.UTF8.GetString(Convert.FromBase64String(base64StringUtf8));
        
        decodedUtf32.Should().BeEquivalentTo(decodedUtf8).And.Be(str);
    }
    
    [Theory]
    [AutoMockData]
    public void FromBase64String_should_correctly_encode_with_encoding(string str)
    {
        var base64StringUtf32 = str.ToBase64String(Encoding.UTF32);
        var base64StringUtf8 = str.ToBase64String(Encoding.UTF8);
        
        base64StringUtf32.Should().NotBeEquivalentTo(base64StringUtf8);

        var decodedUtf32 = base64StringUtf32.FromBase64String(Encoding.UTF32);
        var decodedUtf8 = base64StringUtf8.FromBase64String(Encoding.UTF8);
        
        decodedUtf32.Should().BeEquivalentTo(decodedUtf8).And.Be(str);
    }
    
    [Theory]
    [AutoMockData]
    public void FromBase64String_should_correctly_encode_using_utf8_as_default_encoding(string str)
    {
        var base64StringDefaultEncoding = str.ToBase64String();
        var base64StringUtf8 = str.ToBase64String(Encoding.UTF8);
        
        var decodedDefaultEncoding = base64StringDefaultEncoding.FromBase64String();
        var decodedUtf8 = base64StringUtf8.FromBase64String(Encoding.UTF8);

        decodedDefaultEncoding.Should().BeEquivalentTo(decodedUtf8).And.Be(str);
    }

    [Fact]
    public void EqualsIgnoreCase_should_return_true_if_strings_are_equal_ignoring_case()
    {
        "test".EqualsIgnoreCase("TeSt").Should().BeTrue();
    }
    
    [Fact]
    public void EqualsIgnoreCase_should_return_false_if_strings_are_equal_ignoring_case()
    {
        "test".EqualsIgnoreCase("different string").Should().BeFalse();
    }
    
    [Fact]
    public void EqualsIgnoreCase_should_return_true_if_both_strings_are_null()
    {
        ((string)null).EqualsIgnoreCase(null).Should().BeTrue();
    }
    
    [Fact]
    public void EqualsIgnoreCase_should_return_true_if_one_strings_is_null_other_is_not()
    {
        ((string)null).EqualsIgnoreCase("test").Should().BeTrue();
    }

    [Fact]
    public void ReplaceStart_should_replace_start_of_a_string_old_value_with_a_new_value()
    {
        var str = @"c:\my\awesome\path";

        str.ReplaceStart(@"c:\my\awesome", @"d:\new").Should().Be(@"d:\new\path");
        "MY test string...".ReplaceStart("my Test", "My awesome", StringComparison.OrdinalIgnoreCase).Should().Be("My awesome string...");
        "MY test string...".ReplaceStart("my Test", "My very long replacement awesome", StringComparison.OrdinalIgnoreCase)
                           .Should()
                           .Be("My very long replacement awesome string...");
        "test string where result should be the same".ReplaceStart("est string", "not important").Should().Be("test string where result should be the same");

        "".ReplaceStart("", "My awesome", StringComparison.OrdinalIgnoreCase).Should().Be("My awesome");
    }
    
    
    [Theory]
    [AutoMockData]
    public void ToInt32_should_convert_string_to_int32(int expected)
    {
        var intStr = expected.ToString();

        var int32 = intStr.ToInt32();

        int32.Should().Be(expected);
    }

    [Theory]
    [AutoMockData]
    public void TryConvertToInt32_should_convert_string_to_int32_if_can_convert(int expected)
    {
        var intStr = expected.ToString(CultureInfo.InvariantCulture);

        var result = intStr.TryConvertToInt32(out var int32);

        result.Should().BeTrue();
        int32.Should().Be(expected);
    }

    [Theory]
    [AutoMockData]
    public void TryConvertToInt32_should_return_false_if_cannot_convert(string str)
    {
        var result = str.TryConvertToInt32(out var int32);

        result.Should().BeFalse();
        int32.Should().Be(0);
    }
    
    [Theory]
    [AutoMockData]
    public void ToInt64_should_convert_string_to_int64(int expected)
    {
        var intStr = expected.ToString();

        var int64 = intStr.ToInt64();

        int64.Should().Be(expected);
    }

    [Theory]
    [AutoMockData]
    public void TryConvertToInt64_should_convert_string_to_int64_if_can_convert(int expected)
    {
        var intStr = expected.ToString(CultureInfo.InvariantCulture);

        var result = intStr.TryConvertToInt64(out var int64);

        result.Should().BeTrue();
        int64.Should().Be(expected);
    }

    [Theory]
    [AutoMockData]
    public void TryConvertToInt64_should_return_false_if_cannot_convert(string str)
    {
        var result = str.TryConvertToInt64(out var int64);

        result.Should().BeFalse();
        int64.Should().Be(0);
    }
}