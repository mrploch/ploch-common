using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests;

public class ContentSizesTests
{
    [Fact]
    public void KiloByte_should_equal_1024_bytes()
    {
        // Assert
        ContentSizes.KiloByte.Should().Be(1024);
    }

    [Fact]
    public void MegaByte_should_equal_1048576_bytes()
    {
        // Assert
        ContentSizes.MegaByte.Should().Be(1024 * 1024);
    }

    [Theory]
    [InlineData(1, 1024)]
    [InlineData(2, 2048)]
    [InlineData(0, 0)]
    [InlineData(1000, 1024000)]
    public void KilobytesToBytes_should_convert_kilobytes_to_bytes_correctly(long kilobytes, long expectedBytes)
    {
        // Act
        var result = ContentSizes.KilobytesToBytes(kilobytes);

        // Assert
        result.Should().Be(expectedBytes);
    }

    [Theory]
    [InlineData(1, 1048576)]
    [InlineData(2, 2097152)]
    [InlineData(0, 0)]
    [InlineData(1000, 1048576000)]
    public void MegabytesToBytes_should_convert_megabytes_to_bytes_correctly(long megabytes, long expectedBytes)
    {
        // Act
        var result = ContentSizes.MegabytesToBytes(megabytes);

        // Assert
        result.Should().Be(expectedBytes);
    }

    [Fact]
    public void MegaByte_should_equal_KiloByte_squared()
    {
        // Assert
        ContentSizes.MegaByte.Should().Be(ContentSizes.KiloByte * ContentSizes.KiloByte);
    }
}
