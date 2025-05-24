using System.Runtime.Versioning;
using System.ServiceProcess;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

/// <summary>
///     Provides extension methods for converting between custom service-related enumerations
///     and their equivalent .NET Framework types.
/// </summary>
/// <remarks>
///     This class contains methods that facilitate the conversion of custom enumerations,
///     such as <see cref="ServiceStartMode" />, <see cref="ServiceType" />, and <see cref="ServiceState" />,
///     to their corresponding types in the <see cref="System.ServiceProcess" /> namespace.
///     These methods are designed to simplify interoperability with Windows services.
///     All methods in this class are only supported on Windows platforms.
/// </remarks>
/// <seealso cref="ServiceStartMode" />
/// <seealso cref="ServiceType" />
/// <seealso cref="ServiceState" />
/// <seealso cref="System.ServiceProcess.ServiceStartMode" />
/// <seealso cref="System.ServiceProcess.ServiceType" />
/// <seealso cref="System.ServiceProcess.ServiceControllerStatus" />
public static class ServiceEnumExtensions
{
    /// <summary>
    ///     Converts a <see cref="ServiceStartMode" /> value to its equivalent <see cref="System.ServiceProcess.ServiceStartMode" /> value.
    /// </summary>
    /// <param name="startMode">The <see cref="ServiceStartMode" /> value to convert.</param>
    /// <returns>The corresponding <see cref="System.ServiceProcess.ServiceStartMode" /> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the <paramref name="startMode" /> value is not a valid <see cref="ServiceStartMode" />.
    /// </exception>
    /// <remarks>
    ///     This method is only supported on Windows platforms.
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public static System.ServiceProcess.ServiceStartMode ToServiceProcessStartMode(this ServiceStartMode startMode) =>
        startMode switch
        {
            ServiceStartMode.Auto => System.ServiceProcess.ServiceStartMode.Automatic,
            ServiceStartMode.Disabled => System.ServiceProcess.ServiceStartMode.Disabled,
            ServiceStartMode.Manual => System.ServiceProcess.ServiceStartMode.Manual,
            _ => throw new ArgumentOutOfRangeException(nameof(startMode), startMode, "Invalid ServiceStartMode value")
        };

    /// <summary>
    ///     Converts a <see cref="ServiceType" /> value to its equivalent <see cref="System.ServiceProcess.ServiceType" /> value.
    /// </summary>
    /// <param name="serviceType">The <see cref="ServiceType" /> value to convert.</param>
    /// <returns>The corresponding <see cref="System.ServiceProcess.ServiceType" /> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the <paramref name="serviceType" /> value is not a valid <see cref="ServiceType" />.
    /// </exception>
    /// <remarks>
    ///     This method is only supported on Windows platforms.
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public static System.ServiceProcess.ServiceType? ToServiceProcessType(this ServiceType serviceType) =>
        serviceType switch
        {
            ServiceType.KernelDriver => System.ServiceProcess.ServiceType.KernelDriver,
            ServiceType.FileSystemDriver => System.ServiceProcess.ServiceType.FileSystemDriver,
            ServiceType.Adapter => System.ServiceProcess.ServiceType.Adapter,
            ServiceType.RecognizerDriver => System.ServiceProcess.ServiceType.RecognizerDriver,
            ServiceType.OwnProcess => System.ServiceProcess.ServiceType.Win32OwnProcess,
            ServiceType.ShareProcess => System.ServiceProcess.ServiceType.Win32ShareProcess,
            ServiceType.InteractiveProcess => System.ServiceProcess.ServiceType.InteractiveProcess,
            ServiceType.Unknown => null,
            _ => throw new ArgumentOutOfRangeException(nameof(serviceType), serviceType, "Invalid ServiceType value")
        };

    /// <summary>
    ///     Converts a <see cref="ServiceState" /> value to its equivalent <see cref="System.ServiceProcess.ServiceControllerStatus" /> value.
    /// </summary>
    /// <param name="status">The <see cref="ServiceState" /> value to convert.</param>
    /// <returns>The corresponding <see cref="System.ServiceProcess.ServiceControllerStatus" /> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the <paramref name="status" /> value is not a valid <see cref="ServiceState" />.
    /// </exception>
    /// <remarks>
    ///     This method is only supported on Windows platforms.
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public static ServiceControllerStatus ToServiceProcessStatus(this ServiceState status) =>
        status switch
        {
            ServiceState.Running => ServiceControllerStatus.Running,
            ServiceState.Stopped => ServiceControllerStatus.Stopped,
            ServiceState.StartPending => ServiceControllerStatus.StartPending,
            ServiceState.StopPending => ServiceControllerStatus.StopPending,
            ServiceState.ContinuePending => ServiceControllerStatus.ContinuePending,
            ServiceState.PausePending => ServiceControllerStatus.PausePending,
            ServiceState.Paused => ServiceControllerStatus.Paused,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Invalid ServiceState value")
        };
}
