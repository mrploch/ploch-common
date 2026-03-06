using System;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Provides extension methods for the <see cref="OperatingSystem" /> class.
/// </summary>
public static class OperatingSystemExtensions
{
    /// <summary>
    ///     Checks if the current operating system is Windows.
    /// </summary>
    /// <param name="operatingSystem">The operating system instance.</param>
    /// <returns><c>true</c> if the operating system is Windows; otherwise, <c>false</c>.</returns>
    public static bool IsWindows(this OperatingSystem operatingSystem) => operatingSystem.NotNull(nameof(operatingSystem)).Platform == PlatformID.Win32NT;
}
