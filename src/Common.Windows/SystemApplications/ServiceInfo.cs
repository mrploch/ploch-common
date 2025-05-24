using System.ServiceProcess;

namespace Ploch.Common.Windows.SystemApplications;

public record ServiceInfo(string Name, string? DisplayName) : SystemApplicationInfo(Name, DisplayName)
{
    public int ProcessId { get; init; }

    public ServiceStartMode StartMode { get; init; }

    public bool DelayedAutoStart { get; init; }

    public bool AcceptPause { get; init; }

    public bool AcceptStop { get; init; }

    public ServiceType? ServiceType { get; init; }

    public ServiceControllerStatus? Status { get; init; }
}
