using System.Text;
using FluentAssertions;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Xunit;

namespace Ploch.Common.Tests
{
    public class StringBuilderExtensionsTests
    {
        [Theory]
        [AutoMockData]
        public void AppendIfNotNull_should_append_if_string_is_not_null(StringBuilder sb)
        {
            sb.AppendIfNotNull("test");
            sb.ToString().Should().EndWith("test");
        }

        [Theory]
        [AutoMockData]
        public void AppendIfNotNull_shoul_not_append_if_string_is_null(StringBuilder sb)
        {
            sb.AppendIfNotNull((string)null);
            sb.ToString().Should().NotEndWith("test");
        }

        [Theory]
        [AutoMockData]
        public void AppendIfNotNullOrEmpty_should_append_if_string_is_not_null(StringBuilder sb)
        {
            sb.AppendIfNotNullOrEmpty("test");
            sb.ToString().Should().EndWith("test");
        }

        [Theory]
        [AutoMockData]
        public void AppendIfNotNullOrEmpty_shoul_not_append_if_string_is_null(StringBuilder sb)
        {
            sb.AppendIfNotNull(string.Empty);
            sb.ToString().Should().NotEndWith("test");
        }
    }
}