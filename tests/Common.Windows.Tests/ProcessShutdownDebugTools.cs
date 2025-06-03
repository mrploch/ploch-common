using System.Diagnostics;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using Ploch.Common.Windows.Processes;
using Vanara.Extensions;

namespace Ploch.Common.Windows.Tests;

public static class ProcessShutdownDebugTools
{
    public static void DumpAllProcesses(Guid runId)
    {
        var processes = Process.GetProcesses()
                               .Select(p => new DumpEntry
                                            {
                                                ProcessName = p.ProcessName,
                                                ProcessDisplayName = p.TryGetMainModule().Value.ModuleName,
                                                Path = p.GetImageFilePath(),
                                                ProcessParentName = p.GetParentProcess().ProcessName
                                            })
                               .ToArray();

        using (var writer = new StreamWriter($"run-{runId}-all-processes.cssv"))
        using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            csv.WriteRecords(processes);
        }

        File.WriteAllLines($"run-{runId}-all-processes.txt", processes.Select(p => p.ProcessName));
    }

    public static void AddStoppedProcess(Guid runId,
                                         string processName,
                                         string processDisplayName,
                                         string? path,
                                         string serviceName,
                                         string serviceDisplayName)
    {
        var processes = Process.GetProcessesByName(processName);

        File.AppendAllLines($"run-{runId}-stopped-processes.txt", [processName]);
    }

    public static Guid Start(out StreamWriter writer, out CsvWriter csv)
    {
        var runId = Guid.NewGuid();
        var sb = new StringBuilder();

        sb.AppendLine($"Process shutdown debugging run id {runId}");
        sb.AppendLine(DateTime.Now.ToString());

        File.WriteAllText($"run-{runId}.txt", sb.ToString());

        writer = new StreamWriter($"run-{runId}-all-processes.cssv");
        csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        return runId;
    }
}
