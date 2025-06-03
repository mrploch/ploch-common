using System.Diagnostics;
using System.ServiceProcess;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.Tests;

public class ServiceInfo
{
    private Process? _process;

    public required WindowsManagementService Service { get; init; }

    public required ServiceController ServiceController { get; init; }

    public Process? Process
    {
        get
        {
            if (_process == null)
            {
                RefreshProcess();
            }

            return _process;
        }

        set => _process = value;
    }

    public void RefreshProcess() => Process = Process.GetProcessById(Service.ProcessId);
}