namespace Ploch.Common.Tests;

/// <summary>
///     Tests for the <see cref="OperatingSystemExtensions" /> class.
/// </summary>
public class OperatingSystemExtensionsTests
{
    /// <summary>
    ///     Provides test data for various non-Windows operating systems.
    /// </summary>
    public static IEnumerable<object[]> NonWindowsPlatforms()
    {
        yield return [new OperatingSystem(PlatformID.Unix, new())];
        yield return [new OperatingSystem(PlatformID.MacOSX, new())];
        yield return [new OperatingSystem(PlatformID.Win32S, new())];
        yield return [new OperatingSystem(PlatformID.Win32Windows, new())];
        yield return [new OperatingSystem(PlatformID.WinCE, new())];
    }

    [Fact]
    public void IsWindows_should_throw_ArgumentNullException_when_operatingSystem_is_null()
    {
        // Arrange
        OperatingSystem? sut = null;

        // Act
        var act = () => sut!.IsWindows();

        // Assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("operatingSystem");
    }

    [Fact]
    public void IsWindows_should_return_true_when_platform_is_Win32NT()
    {
        // Arrange
        var windowsOs = new OperatingSystem(PlatformID.Win32NT, new());

        // Act
        var result = windowsOs.IsWindows();

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(NonWindowsPlatforms))]
    public void IsWindows_should_return_false_for_non_windows_platforms(OperatingSystem nonWindowsOs)
    {
        // Act
        var result = nonWindowsOs.IsWindows();

        // Assert
        result.Should().BeFalse();
    }
}
