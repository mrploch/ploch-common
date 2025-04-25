using System.Management;
using System.Runtime.Versioning;

namespace Ploch.Common.Windows.Services;

/// <summary>
///     Provides utility methods for working with Windows services.
/// </summary>
/// <remarks>
///     This class contains static methods to query and retrieve details about Windows Services
///     using Windows Management Instrumentation (WMI). The features provided by this class
///     are supported only on Windows operating systems.
/// </remarks>
public static class WindowsServiceUtilities
{
    /// <summary>
    ///     Retrieves the executable path of a Windows service.
    /// </summary>
    /// <param name="serviceName">The name of the Windows service to query.</param>
    /// <returns>
    ///     A string representing the full path to the executable file associated with the specified Windows service.
    /// </returns>
    /// <remarks>
    ///     This method uses Windows Management Instrumentation (WMI) to query service information.
    ///     It is only supported on Windows operating systems.
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public static string? GetExecutablePath(string serviceName)
    {
        using var wmiService = new ManagementObject($"{WmiWin32Service.ClassName}.{nameof(WmiWin32Service.Name)}='{serviceName}'");
        wmiService.Get();

        return wmiService[nameof(WmiWin32Service.PathName)]?.ToString();
    }

//#pragma warning restore CS8603 // Possible null reference return.
}
