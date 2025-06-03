using System.Diagnostics;
using System.ServiceProcess;
using Ploch.Common.Windows.Processes;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.Tests;

public static class ServiceUtilities
{
    public static IDictionary<string, WindowsManagementService> GetServices()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());
        var services = queryFactory.Create().GetAll<WindowsManagementService>();

        return services.ToDictionary(s => s.Name!, s => s);
    }

    public static bool TryPause(ServiceController service)
    {
        try
        {
            service.Pause();
            service.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromSeconds(10));

            return true;
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Error pausing service '{service.ServiceName}': {ex.Message}");

            return false;
        }
    }

    public static void StopService(WindowsManagementService service, IDictionary<string, WindowsManagementService> allServices)
    {
        try
        {
            var sc = new ServiceController(service.Name!);
            if (sc.Status == ServiceControllerStatus.Running)
            {
                sc.Stop(true);

                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
            }
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Error stopping service '{service.Name}': {ex.Message}");
        }
    }

    public static async Task<bool> StopServiceAndDependencies(ServiceProcess service,
                                                              IDictionary<string, WindowsManagementService> allServices,
                                                              TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(10);
        if (service == null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        if (service.ServiceController.Status == ServiceControllerStatus.Running)
        {
            foreach (var dependentService in service.ServiceController.DependentServices)
            {
                Holder.Output.WriteLine($"Stopping dependent service {dependentService.DisplayName} ({dependentService.ServiceName}) - type {dependentService.ServiceType}.");
                if (!allServices.TryGetValue(dependentService.ServiceName, out var dependentServiceInfo))
                {
                    Holder.Output.WriteLine($"Dependent service '{dependentService.ServiceName}' not found.");

                    if (!Holder.AllServices.TryGetValue(dependentService.ServiceName, out var dependentServiceInfo1))
                    {
                        Holder.Output.WriteLine($"Dependent service '{dependentService.ServiceName}' not found in the list of all services.");

                        continue;
                    }

                    dependentServiceInfo = dependentServiceInfo1.Service;
                }

                var dependentServiceProcess = ServiceProcessLister.GetServiceProcess(dependentService);
                await StopServiceAndDependencies(dependentServiceProcess, allServices, timeout);
            }

            try
            {
                Holder.DebugTool.AddDumpEntry(new DumpEntry
                                              {
                                                  ServiceName = service.Service.Name,
                                                  ServiceDisplayName = service.Service.DisplayName,
                                                  Path = service.Service.PathName,
                                                  ActionType = ActionType.StopService
                                              });
                service.ServiceController.Stop(true);

                service.ServiceController.WaitForStatus(ServiceControllerStatus.Stopped, timeout.Value);
                Holder.DebugTool.AddDumpEntry(new DumpEntry
                                              {
                                                  ServiceName = service.Service.Name,
                                                  ServiceDisplayName = service.Service.DisplayName,
                                                  Path = service.Service.PathName,
                                                  ActionType = ActionType.StoppedService
                                              });

                return true;
            }
            catch (Exception ex)
            {
                Holder.Output.WriteLine($"Error stopping service '{service.Service.Name}': {ex.Message}");
                await KillProcess(service);

                return false;
            }
        }

        if (service.ServiceController.Status == ServiceControllerStatus.Stopped)
        {
            return true;
        }

        return false;
    }

    public static async Task<bool> KillProcess(ServiceProcess serviceProcess, CancellationToken cancellationToken = default)
    {
        try
        {
            var process = Process.GetProcessById(serviceProcess.Service.ProcessId);
            var isCriticalProcess = CriticalProcessChecker.TryGetIsCriticalProcess(process, out var critical) && critical;
            if (isCriticalProcess)
            {
                Holder.DebugTool.AddCriticalServiceEntry(serviceProcess, ActionType.KillProcess);

                var stop = false;

                if (!stop)
                {
                    return false;
                }
            }

            if (process.HasExited)
            {
                return true;
            }

            if (!await ProcessShutdownHelper.TryGracefulShutdownAsync(process, cancellationToken))
            {
                return ProcessShutdownHelper.KillProcess(process);
            }
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Error killing process with ID '{serviceProcess.Service.ProcessId}': {ex.Message}");

            return false;
        }

        return false;
    }
}
