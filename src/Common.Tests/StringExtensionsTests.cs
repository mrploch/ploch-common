using System;
using System.Text;
using FluentAssertions;
using Ploch.TestingSupport.Xunit.AutoFixture;
using Xunit;

namespace Ploch.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [AutoDataMoq]
        public void IsNullOrEmptyTest(string str)
        {
            str.IsNullOrEmpty().Should().BeFalse();
            string nullString = null;
            nullString.IsNullOrEmpty().Should().BeTrue();

            "".IsNullOrEmpty().Should().BeTrue();
        }

        [Theory]
        [AutoDataMoq]
        public void ToBase64String_should_correctly_encode(string str)
        {
            var base64String = str.ToBase64String(Encoding.UTF8);

            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(base64String));

            decoded.Should().Be(str);
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
    }
}