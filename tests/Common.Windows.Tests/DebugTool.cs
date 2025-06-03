using System.Diagnostics;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Ploch.Common.Windows.Processes;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects;
using Vanara.Extensions;

namespace Ploch.Common.Windows.Tests;

public class DebugTool : IDisposable
{
    private readonly CsvWriter _csv;
    private readonly StreamWriter _writer;

    public DebugTool()
    {
        RunId = Guid.NewGuid();
        _writer = new StreamWriter($"run-{RunId}-stopped-processes.csv") { AutoFlush = true };
        _csv = new CsvWriter(_writer, new CsvConfiguration(CultureInfo.InvariantCulture));
    }

    public static DumpEntry ToDumpEntry(ProcessProperties process,
                                        Dictionary<int, WindowsManagementService> servicesByProcessId,
                                        Process? diagnosticProcess = null)
    {
        diagnosticProcess ??= Process.GetProcessById(process.Id);

        servicesByProcessId.TryGetValue(process.Id, out var service);

        // var service = servicesByProcessId.GetValueOrDefault(process.Id);

        return new DumpEntry
               {
                   ProcessName = process.Name,
                   ProcessDisplayName = process.Caption,
                   Path = process.ExecutablePath,
                   ProcessParentName = process.ParentProcess?.Name,
                   ParentProcessDisplayName = process.ParentProcess?.Caption,
                   ParentProcessPath = process.ParentProcess?.ExecutablePath,
                   ServiceName = service?.Name,
                   ServiceDisplayName = service?.DisplayName,
                   IsCritical = CriticalProcessChecker.TryGetIsCriticalProcess(diagnosticProcess, out var isCritical) && isCritical
               };
    }

    public void Dispose()
    {
        try
        {
            _csv.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public Guid RunId { get; }

    private string StoppedProcessesTxt => $"run-{RunId}-stopped-processes.txt";

    public void Start()
    {
        var processes = ServiceProcessLister.GetProcesses().ToArray();
        var servicesByProcessId = ServiceProcessLister.GetServicesByProcessId();
        var dumpEntries = processes.Select(p => ToDumpEntry(p, servicesByProcessId)).ToArray();
        using (var writer = new StreamWriter($"run-{RunId}-all-processes.csv"))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(dumpEntries);
        }

        _csv.WriteHeader<DumpEntry>();
        _csv.Flush();
        _writer.Flush();
        File.WriteAllLines($"run-{RunId}-all-processes.txt", processes.Select(p => p.Name));
    }

    public void AddDumpEntry(DumpEntry dumpEntry)
    {
        File.AppendAllLines(StoppedProcessesTxt, [dumpEntry.ToString()]);

        _csv.WriteRecord(dumpEntry);
        _csv.Flush();
        _writer.Flush();
    }

    public void AddCriticalServiceEntry(ServiceProcess serviceProcess, ActionType actionType)
    {
        var dumpEntry = GetDumpEntry(serviceProcess, actionType);
        File.AppendAllLines(StoppedProcessesTxt, [dumpEntry.ProcessName]);
        AddDumpEntry(dumpEntry);
    }

    public void AddServiceEntry(ServiceProcess serviceProcess, ActionType actionType) => AddDumpEntry(GetDumpEntry(serviceProcess, actionType));

    public DumpEntry GetDumpEntry(ServiceProcess serviceProcess, ActionType actionType) =>
        new()
        {
            ActionType = actionType,
            IsCritical = true,
            Path = serviceProcess.Process?.StartInfo?.FileName ?? serviceProcess.Service.PathName,
            ProcessDisplayName = serviceProcess.Process?.ProcessName,
            ProcessName = serviceProcess.Process?.ProcessName ?? serviceProcess.Service.Name!,
            ServiceDisplayName = serviceProcess.Service.DisplayName,
            ProcessParentName = serviceProcess.Process?.GetParentProcess()?.ProcessName
        };

    private static IWmiQuery CreateWmiQuery(IWmiObjectQueryFactory? queryFactory = null)
    {
        queryFactory ??= new WmiObjectQueryFactory(new DefaultWmiConnectionFactory());

        return queryFactory.Create();
    }
}