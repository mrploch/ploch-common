using System.Diagnostics;
using System.ServiceProcess;
using Ploch.Common.Windows.Processes;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.Tests;

public static class ServiceProcessLister
{
    private static IWmiQuery CreateWmiQuery(IWmiObjectQueryFactory? queryFactory = null)
    {
        queryFactory ??= new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());

        return queryFactory.Create();
    }

    public static IEnumerable<ServiceProcess> GetServiceProcesses()
    {
        using var query = CreateWmiQuery();
        var services = query.GetAll<WindowsManagementService>();
        var servicesByProcessId = new Dictionary<ProcessProperties, List<WindowsManagementService>>();
        var processes = query.GetAll<WindowsManagementProcess>().ToDictionary(p => p.ProcessId, p => p);
        var serviceProcesses = services.Select(s => new ServiceProcess
                                                    {
                                                        Service = s,
                                                        ServiceController = new ServiceController(s.Name!),
                                                        Process = Process.GetProcessById(s.ProcessId),
                                                        WmiProcess = processes.GetValueOrDefault(s.ProcessId)
                                                    });

        return serviceProcesses;
    }

    public static IEnumerable<ServiceProcess> GetProcessesAndServices()
    {
        using var query = CreateWmiQuery();
        var services = query.GetAll<WindowsManagementService>();
        var servicesByProcessId = new Dictionary<ProcessProperties, List<WindowsManagementService>>();
        var processes = query.GetAll<WindowsManagementProcess>().ToDictionary(p => p.ProcessId, p => p);
        var serviceProcesses = services.Select(s => new ServiceProcess
                                                    {
                                                        Service = s,
                                                        ServiceController = new ServiceController(s.Name!),
                                                        Process = Process.GetProcessById(s.ProcessId),
                                                        WmiProcess = processes.GetValueOrDefault(s.ProcessId)
                                                    });

        return serviceProcesses;
    }

    public static IEnumerable<ProcessProperties> GetProcesses()
    {
        using var query = CreateWmiQuery();
        var processes = query.GetAll<WindowsManagementProcess>().ToDictionary(p => p.ProcessId, p => p);

        return processes.Values.Select(p => p.ToProcessProperties(processes)).ToArray();
    }

    public static ProcessProperties ToProcessProperties(this WindowsManagementProcess process, Dictionary<int, WindowsManagementProcess> processes) =>
        new(process.ProcessId,
            process.Caption,
            process.CommandLine,
            process.Description,
            process.Name!,
            process.ExecutablePath,
            process.ParentProcessId != 0 ? processes.GetValueOrDefault(process.ParentProcessId)?.ToProcessProperties(processes) : null);

    public static IEnumerable<WindowsManagementProcess> GetAllProcesses()
    {
        using var query = CreateWmiQuery();

        return query.GetAll<WindowsManagementProcess>();
    }

    public static Dictionary<string, WindowsManagementService> GetServices()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());
        var services = queryFactory.Create().GetAll<WindowsManagementService>();

        return services.ToDictionary(s => s.Name!, s => s);
    }

    public static Dictionary<int, WindowsManagementService> GetServicesByProcessId()
    {
        var processes = GetProcessesById();
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());
        var wmiServices = queryFactory.Create().GetAll<WindowsManagementService>();
        var startedServices = wmiServices.Where(s => s.Started).ToArray();
        var nonZeroProcess = wmiServices.Where(s => s.ProcessId != 0).ToArray();
        var running = wmiServices.Where(s => s.State == ServiceState.Running);
        var services = wmiServices.Where(s => s.ProcessId != 0).ToArray();

        var result = new Dictionary<int, WindowsManagementService>();
        var resultMulti = new Dictionary<ProcessProperties, List<WindowsManagementService>>();
        foreach (var wmiService in startedServices)
        {
            var processProperties = processes.GetValueOrDefault(wmiService.ProcessId)?.ToProcessProperties(processes);
            if (processProperties != null)
            {
                if (!resultMulti.ContainsKey(processProperties))
                {
                    resultMulti.Add(processProperties, new List<WindowsManagementService>());
                }

                resultMulti[processProperties].Add(wmiService);
            }

            var added = result.TryAdd(wmiService.ProcessId, wmiService);
            if (!added)
            {
                Holder.Output.WriteLine($"Duplicate process ID {wmiService.ProcessId} found for service {wmiService.Name}");
            }
        }

        return result;
    }

    public static Dictionary<ProcessProperties, List<WindowsManagementService>> GetProcessToServicesMapping()
    {
        var resultMulti = new Dictionary<ProcessProperties, List<WindowsManagementService>>();
        //var runningServicesByProcessId =
        foreach (var processProperties in GetProcesses())
        { }

        throw new NotImplementedException();
    }

    public static ServiceProcess GetServiceProcess(WindowsManagementService service, ServiceController serviceController)
    {
        using var query = CreateWmiQuery();
        var process = query.GetFirstOrDefault<WindowsManagementProcess, int>(q => q.ProcessId, service.ProcessId);

        return new ServiceProcess
               {
                   Service = service, ServiceController = serviceController, Process = Process.GetProcessById(service.ProcessId), WmiProcess = process
               };
    }

    public static ServiceProcess GetServiceProcess(WindowsManagementService service) =>
        new() { Service = service, ServiceController = new ServiceController(service.Name!), Process = Process.GetProcessById(service.ProcessId) };

    public static ServiceProcess GetServiceProcess(ServiceController dependentService)
    {
        using var query = CreateWmiQuery();
        var service = query.GetFirstOrDefault<WindowsManagementService, string>(q => q.Name!, dependentService.ServiceName)!;
        var process = query.GetFirstOrDefault<WindowsManagementProcess, int>(q => q.ProcessId, service.ProcessId);

        return new ServiceProcess
               {
                   Service = service, ServiceController = dependentService, Process = Process.GetProcessById(service.ProcessId), WmiProcess = process
               };
    }

    public static WindowsManagementProcess? GetProcess(int processId)
    {
        using var query = CreateWmiQuery();
        var process = query.GetFirstOrDefault<WindowsManagementProcess, int>(q => q.ProcessId, processId);

        return process;
    }

    public static Dictionary<int, WindowsManagementProcess> GetProcessesById() => GetAllProcesses().ToDictionary(p => p.ProcessId, p => p);
}
