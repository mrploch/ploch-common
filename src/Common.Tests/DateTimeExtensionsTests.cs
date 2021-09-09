using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void ToEpochSeconds_should_convert_non_nullable_DateTime()
        {
            var sut = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            sut.ToEpochSeconds().Should().Be(0);
        }

        [Fact]
        public void ToDateTime_should_convert_any_number_to_DateTime()
        {
            long longSeconds = 0;
            longSeconds.ToDateTime().Should().Be(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var intSeconds = 0;
            intSeconds.ToDateTime().Should().Be(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            float floatSeconds = 0;
            floatSeconds.ToDateTime().Should().Be(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            sbyte sbyteSeconds = 0;
            sbyteSeconds.ToDateTime().Should().Be(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            sbyteSeconds.ToDateTime().ToEpochSeconds().Should().Be(0);
        }

        [Theory]
        [AutoData]
        public void ToDateTime_should_parse_a_number_to_DateTime_with_UTC_time_zone(int epochSeconds)
        {
            epochSeconds.ToDateTime().Kind.Should().Be(DateTimeKind.Utc);

            epochSeconds.ToDateTime().ToEpochSeconds().Should().Be(epochSeconds);
        }
    }
}