using System.Diagnostics;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     Provides functionality to build <see cref="ServiceInfo" /> objects from
///     <see cref="WindowsManagementService" /> instances.
/// </summary>
/// <remarks>
///     This static class is responsible for extracting and processing information from
///     <see cref="WindowsManagementService" /> objects to create <see cref="ServiceInfo" /> instances.
///     It handles various service-related properties, including name, description, file path,
///     version information, and other attributes.
/// </remarks>
public static class ServiceInfoBuilder
{
    /// <summary>
    ///     Creates a new instance of the <see cref="ServiceInfo" /> class based on the specified
    ///     <see cref="WindowsManagementService" /> object.
    /// </summary>
    /// <param name="service">
    ///     The <see cref="WindowsManagementService" /> instance representing the service from which
    ///     the <see cref="ServiceInfo" /> will be created. This parameter must not be null.
    /// </param>
    /// <returns>
    ///     A <see cref="ServiceInfo" /> object containing detailed information about the specified service,
    ///     including its name, description, file path, version information, and various service-related properties.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="service" /> or any of its required properties are null or empty.
    /// </exception>
    /// <remarks>
    ///     This method extracts and processes various properties of the provided
    ///     <see cref="WindowsManagementService" /> instance, such as its name, description, file path,
    ///     and service-specific attributes. It also retrieves file version information for the service's
    ///     executable file.
    /// </remarks>
    public static ServiceInfo Create(WindowsManagementService service)
    {
        var applicationPath = CommandLineParser.GetApplicationPath(service.PathName.RequiredNotNullOrEmpty());
        var fileVersionInfo = FileVersionInfo.GetVersionInfo(applicationPath!);

        return new ServiceInfo(service.Name.RequiredNotNullOrEmpty(), service.Caption)
               {
                   Description = service.Description,
                   FilePath = service.PathName,
                   FileVersionInfo = fileVersionInfo,
                   AcceptPause = service.AcceptPause,
                   StartMode = service.StartMode.RequiredNotNull().ToServiceProcessStartMode(),
                   DelayedAutoStart = service.DelayedAutoStart,
                   AcceptStop = service.AcceptStop,
                   ServiceType = service.ServiceType.RequiredNotNull().ToServiceProcessType(),
                   Status = service.State.RequiredNotNull().ToServiceProcessStatus(),
                   IsRunning = service.Started,
                   ProcessId = service.ProcessId
               };
    }
}
