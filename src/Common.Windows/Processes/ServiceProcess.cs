using System.Diagnostics;
using System.ServiceProcess;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.Tests;

public class ServiceProcess
{
    public required WindowsManagementService Service { get; init; }

    public required ServiceController ServiceController { get; init; }

    public Process? Process { get; set; }

    public WindowsManagementProcess? WmiProcess { get; set; }
}

public class SystemApplications
{
    //public IEnumerable<>
}
