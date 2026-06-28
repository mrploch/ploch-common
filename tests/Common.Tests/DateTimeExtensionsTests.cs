namespace Ploch.Common.Tests;

public class DateTimeExtensionsTests
{
  [Fact]
  public void ToEpochSeconds_should_convert_non_nullable_DateTime()
  {
    var sut = DateTime.UnixEpoch;
    sut.ToEpochSeconds().Should().Be(0);
  }

  [Fact]
  public void ToEpochSeconds_with_null_should_convert_non_nullable_DateTime()
  {
    DateTime? sut = null;
    sut.ToEpochSeconds().Should().BeNull();
  }

  [Fact]
  public void ToDateTime_should_convert_any_number_to_DateTime()
  {
    long longSeconds = 0;
    longSeconds.ToDateTime().Should().Be(DateTime.UnixEpoch);

    var intSeconds = 0;
    intSeconds.ToDateTime().Should().Be(DateTime.UnixEpoch);

    float floatSeconds = 0;
    floatSeconds.ToDateTime().Should().Be(DateTime.UnixEpoch);

    sbyte sbyteSeconds = 0;
    sbyteSeconds.ToDateTime().Should().Be(DateTime.UnixEpoch);

    sbyteSeconds.ToDateTime().ToEpochSeconds().Should().Be(0);
  }

  [Theory]
  [AutoData]
  public void ToDateTime_should_parse_a_number_to_DateTime_with_UTC_time_zone(int epochSeconds)
  {
    epochSeconds.ToDateTime().Kind.Should().Be(DateTimeKind.Utc);

    epochSeconds.ToDateTime().ToEpochSeconds().Should().Be(epochSeconds);
  }

  [Fact]
  public void ToDateTime_with_null_should_result_in_null()
  {
    long? epochSeconds = null;
    epochSeconds.ToDateTime().Should().BeNull();
  }
}
