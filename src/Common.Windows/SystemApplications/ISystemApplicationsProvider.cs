namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     Represents a provider for retrieving information about system applications
///     such as processes and services.
/// </summary>
/// <remarks>
///     Implementations of this interface are responsible for fetching details regarding
///     the system's running processes and services. This can be useful for monitoring,
///     diagnostics, or any system-related operation where such information is required.
/// </remarks>
public interface ISystemApplicationsProvider
{
    /// <summary>
    ///     Retrieves a collection of service information currently available on the system.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="ServiceInfo" /> objects, each representing details
    ///     about an individual service, including its name, display name, process ID, start mode,
    ///     and whether it is configured for delayed automatic start.
    /// </returns>
    IEnumerable<ServiceInfo> GetServices();

    /// <summary>
    ///     Retrieves a collection of information about processes currently running on the system.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="ProcessInfo" /> objects, each representing details
    ///     about an individual process. This includes the process ID, name, optional display name,
    ///     command line information, parent process details, and other relevant metadata.
    /// </returns>
    IEnumerable<ProcessInfo> GetProcesses();
}
