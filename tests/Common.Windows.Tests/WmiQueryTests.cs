using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using Ploch.Common.Collections;
using Ploch.Common.Windows.SystemApplications;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects;
using WmiLight;
using Xunit.Abstractions;

namespace Ploch.Common.Windows.Tests;

public class WmiQueryTests
{
    private readonly ITestOutputHelper _output;

    public WmiQueryTests(ITestOutputHelper output)
    {
        _output = output;
        Holder.Output = output;
    }

    [Fact]
    public void MyMethod()
    {
        using var con = new WmiConnection();
        foreach (var process in con.CreateQuery("SELECT * FROM Win32_Process"))
        {
            Holder.Output.WriteLine(process["Name"].ToString());
            var propertyNames = process.GetPropertyNames();
            foreach (var propertyName in propertyNames)
            {
                Holder.Output.WriteLine($"{propertyName}: {process[propertyName]}");
            }
        }
    }

    [Fact]
    public void GetProcesses1()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());
        var processes = queryFactory.Create().GetAll<WindowsManagementProcess>();

        var cmdEmpty = processes.Where(p => p.CommandLine.IsNullOrWhiteSpace()).ToArray();

        var array = processes.Where(p => !p.CommandLine.IsNullOrWhiteSpace())
                             .Select(p => CommandLineParser.GetCommandLine(p.CommandLine))
                             .Select(p => new
                                          {
                                              CommandLine = p!.Value,
                                              FileVersionInfo = File.Exists(p.Value.ApplicationPath)
                                                  ? FileVersionInfo.GetVersionInfo(p!.Value.ApplicationPath)
                                                  : null
                                          })
                             .ToArray();

        var execPathEmpty = processes.Where(p => p.ExecutablePath.IsNullOrWhiteSpace()).ToArray();
        var p1 = processes.Where(p => p.Caption != p.Description).ToArray();
        foreach (var process in p1)
        {
            _output.WriteLine($"{process.Name} ({process.ProcessId}), Description: {process.Description}");
        }

        foreach (var process in processes)
        {
            _output.WriteLine($"{process.Name} ({process.ProcessId}), Description: {process.Description}");
            _output.WriteLine(process.Caption ?? "N/A");
            _output.WriteLine(process.CreationClassName ?? "N/A");
            _output.WriteLine(process.CSName ?? "N/A");
            _output.WriteLine(process.CommandLine ?? "N/A");
            _output.WriteLine(process.ExecutablePath ?? "N/A");

            var winProcess = Process.GetProcessById(process.ProcessId);
            _output.WriteLine($"Process ID: {winProcess.Id}, Process Name: {winProcess.ProcessName}");
        }
    }

    [Fact]
    public void GetServices()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());

        var services = queryFactory.Create().GetAll<WindowsManagementService>();

        //  var startModes = new HashSet<ServiceStartMode?>();
        //var serviceTypes = new HashSet<string?>();
        var states = new HashSet<ServiceState?>();
        var strings = services.Select(s => s.Status).Distinct().ToArray();

        var windowsManagementServices = services.Where(s => !s.Started).ToArray();
        //   var startNames = new HashSet<string?>();
        var captionDisplayNamesDescriptions = new List<(string?, string?, string?)>();
        var paths = services.Select(s => s.PathName).Distinct().ToArray();
        var allPathsString = string.Join("\n", paths);
        foreach (var service in services.OrderBy(s => s.DisplayName))
        {
            _output.WriteLine($"{service.DisplayName} ({service.Name}), Description: {service.Description}");

            try
            {
                var file = Path.GetFileName(service.PathName);
                var fullPath = Path.GetFullPath(service.PathName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var serviceProcess = Process.GetProcessById(service.ProcessId);

            var processPathName = service.PathName;
            var processState = service.State;

            var processDescription = service.Description;
            service.DisplayName = service.DisplayName;

            //   startModes.Add(service.StartMode);
            //   serviceTypes.Add(service.ServiceType);
            //  states.Add(service.State);
            // startNames.Add(service.StartName);
            captionDisplayNamesDescriptions.Add((service.Caption, service.DisplayName, processDescription));
            Holder.Output.WriteLine($"Service ID: {service.ProcessId}, Name: {service.Name}");
        }

        Holder.Output.WriteLine("using Ploch.Common.Windows.Wmi.ManagementObjects;");
        //  PrintStringsAsEnum(nameof(startModes), startModes);
        // PrintStringsAsEnum(nameof(serviceTypes), serviceTypes);
        // //  PrintStringsAsEnum(nameof(states), states);
        // PrintStringsAsEnum(nameof(startNames), startNames);

        Holder.Output.WriteLine(string.Empty);
        Holder.Output.WriteLine("Caption, Display Name, Description");
        foreach (var (caption, displayName, description) in captionDisplayNamesDescriptions)
        {
            if (caption == displayName)
            {
                continue;
            }

            Holder.Output.WriteLine($"Caption: {caption}\nDisplay Name: {displayName}\nDescription: {description}");
            Holder.Output.WriteLine(string.Empty);
        }
    }

    private static void PrintStringsAsEnum(string setName, IEnumerable<string?> strings)
    {
        var mappings = new Dictionary<string, List<string?>>(StringComparer.OrdinalIgnoreCase);
        foreach (var str in strings)
        {
            var camelCase = !str.IsNullOrEmpty() ? str!.ToPascalCase() : "Empty";
            var str2 = str?.Replace(@"\", @"\\");
            if (mappings.ContainsKey(camelCase))
            {
                mappings[camelCase].Add(str2);
            }
            else
            {
                mappings[camelCase] = new List<string?> { str2 };
            }
        }

        Holder.Output.WriteLine($"public enum {setName.ToPascalCase()}");
        Console.WriteLine("{");

        foreach (var (enumEntry, enumValues) in mappings)
        {
            if ((enumValues.Count == 1 && enumValues[0] == null) ||
                (enumValues.Count == 1 && !enumValues[0].Equals(enumEntry, StringComparison.OrdinalIgnoreCase)) || enumValues.Count > 1)
            {
                Console.Write("    ");
                Console.WriteLine($"[WindowsManagementObjectEnumMapping({enumValues.Select(v => v != null ? $"\"{v}\"" : "null").Join(", ")})]");
            }

            Console.Write("    ");
            Console.WriteLine($"{enumEntry},");
        }

        Console.WriteLine("}");
        Console.WriteLine();
    }

    [Fact]
    public void GetProcesses()
    {
        var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());

        var processes = queryFactory.Create().GetAll<WindowsManagementProcess>();

        var diff = processes.Select(p => p.Caption).ToArray();

        var diagnosticProcessesDict = Process.GetProcesses().ToDictionary(p => p.Id, p => p);

        var processInfos = new List<ProcessInfo>();
        foreach (var process in processes)
        {
            Holder.Output.WriteLine($"Process ID: {process.ProcessId}, Name: {process.Name}");
            var diagnosticProcess = diagnosticProcessesDict.GetValueOrDefault(process.ProcessId);

            // var proccessInfo = new ProcessInfo
            //                    {
            //                        Id = process.ProcessId,
            //                        ParentId = process.ParentProcessId,
            //                        Name = process.Name!,
            //                        Description = process.Description ?? process.Caption,
            //                        CommandLine = process.CommandLine,
            //                        FilePath = process.ExecutablePath,
            //                        FileVersionInfo = diagnosticProcess.TryGetMainModule().FileVersionInfo,
            //                        ApplicationStatus = SystemApplicationStatus.Running,
            //                        StartInfo = diagnosticProcess.TryGetProcessStartInfo()
            //                    };

            // processInfos.Add(proccessInfo);
        }
    }

    private static ProcessStartInfo TryGetProcessStartInfo(Process process)
    {
        try
        {
            return process.StartInfo;
        }
        catch (Win32Exception ex)
        {
            Holder.Output.WriteLine($"Failed to get process start info for process ID {process.Id}: {ex.Message}, {ex.NativeErrorCode}");
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Failed to get process start info for process ID {process.Id}: {ex.Message}");
        }

        return null;
    }

    [Fact]
    public void GetServiceProcesses()
    {
        var serviceProcesses = ServiceProcessLister.GetServiceProcesses().ToArray();
        foreach (var service in serviceProcesses)
        {
            var serviceProcess = service.Process;
            var wmiProcess = service.WmiProcess;
            var wmiService = service.Service;
            var serviceController = service.ServiceController;
            Holder.Output.WriteLine($"Process ID: {serviceProcess.Id}, Name: {serviceProcess.ProcessName}, {wmiProcess.Caption}");
            Holder.Output.WriteLine($"Service Name: {wmiService.Name}, Description: {wmiService.Description}, State: {wmiService.State}, Start Mode: {wmiService.StartMode}");
        }
    }

    [Fact]
    public async Task StopServices()
    {
        Holder.Output = _output;

        Holder.DebugTool.Start();

        var managementServices = ServiceUtilities.GetServices();
        var serviceControllers = ServiceController.GetServices().ToArray();

        var serviceTypes = managementServices.Values.Select(s => s.ServiceType).Distinct().ToArray();
        _output.WriteLine("public enum ServiceType {");
        foreach (var serviceType in serviceTypes)
        {
            _output.WriteLine($"{serviceType},");
        }

        _output.WriteLine("}");

        Holder.AllServices = managementServices.ToDictionary(s => s.Key,
                                                             s => new ServiceInfo
                                                                  {
                                                                      Service = s.Value, ServiceController = new ServiceController(s.Value.Name)
                                                                  });

        var runningServices = managementServices.Where(s => s.Value.State == ServiceState.Running).ToArray();

        var runningCount = runningServices.Length;

        var processesCount = Process.GetProcesses().Length;
        foreach (var windowsManagementService in runningServices)
        {
            try
            {
                var serviceProcess = ServiceProcessLister.GetServiceProcess(windowsManagementService.Value);
                var stopped = await ServiceUtilities.StopServiceAndDependencies(serviceProcess, managementServices);

                if (!stopped)
                {
                    var paused = ServiceUtilities.TryPause(serviceProcess.ServiceController);

                    if (!paused)
                    {
                        Holder.Output.WriteLine($"Service '{windowsManagementService.Value.Name}' could not be paused.");

                        var process = Process.GetProcessById(windowsManagementService.Value.ProcessId);

                        process.PriorityClass = ProcessPriorityClass.Idle;
                    }
                }

                //Process.GetProcessById(windowsManagementService.Value.ProcessId);
            }
            catch (Exception ex)
            {
                Holder.Output.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            }
        }

        var runningServicesEnd = managementServices.Where(s => s.Value.State == ServiceState.Running).ToArray();

        var runningCountEnd = runningServicesEnd.Length;

        var stoppedCount = runningCount - runningCountEnd;

        var processesEndCount = processesCount - Process.GetProcesses().Length;
        _output.WriteLine($"Stopped {stoppedCount} services and {processesEndCount} processes.");
        //
        // var queryFactory = new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());
        //
        // var services = queryFactory.Create().GetAll<WindowsManagementService>();
        //
        // var nullName = services.GetWithEmptyProperty(s => s.Name).ToArray();
        // var nullDisplayName = services.GetWithEmptyProperty(s => s.DisplayName).ToArray();
        // var nullDesc = services.GetWithEmptyProperty(s => s.Description).ToArray();
        // var nullCap = services.GetWithEmptyProperty(s => s.Caption).ToArray();

        // foreach (var service in services)
        // {
        //     OutputHolder.Output.WriteLine($"Service ID: {service.ProcessId}, Name: {service.Name}");
        //     new ServiceController(service.Name).Refresh();
        // }
    }
}