using System.Diagnostics;
using Ploch.Common.ArgumentChecking;
using Ploch.Common.Windows.Processes;
using Ploch.Common.Windows.Wmi.ManagementObjects;

namespace Ploch.Common.Windows.SystemApplications;

public static class ProcessInfoBuilder
{
    public static ProcessInfo Create(WindowsManagementProcess process)
    {
        var commandLine = process.CommandLine.IsNullOrWhiteSpace() ? null : CommandLineParser.GetCommandLine(process.CommandLine!);

        var fileVersionInfo = commandLine.HasValue && File.Exists(commandLine.Value.ApplicationPath) ?
                                  FileVersionInfo.GetVersionInfo(commandLine.Value.ApplicationPath) :
                                  null;

        var description = fileVersionInfo?.FileDescription ?? process.Description;

        var processInfo = new ProcessInfo(process.ProcessId, process.Name.RequiredNotNullOrEmpty(), description, commandLine)
                          {
                              ParentId = process.ParentProcessId, Description = description
                          };

        return processInfo;
    }

    public static ProcessInfo Create(Process process, Process? parentProcess)
    {
        var startInfoResult = process.TryGetProcessStartInfo();
        ProcessStartInfo? startInfo = null;
        if (startInfoResult.IsSuccess)
        {
            startInfo = startInfoResult.Value;
        }

        var mainModule = process.TryGetMainModule();
        FileVersionInfo? fileVersionInfo = null;
        if (mainModule.IsSuccess)
        {
            fileVersionInfo = mainModule.Value.RequiredNotNull().FileVersionInfo;
        }

        int? parentId = null;
        string? parentName = null;
        try
        {
            parentId = parentProcess?.Id;
            parentName = parentProcess?.ProcessName;
        }
        catch (Exception ex)
        { }

        CommandLineInfo? commandLineInfo = null;
        if (startInfo != null && fileVersionInfo != null)
        {
            commandLineInfo = CommandLineParser.GetCommandLine(mainModule.Value.RequiredNotNull().FileName);
        }

        var processInfo = new ProcessInfo(process.Id, process.ProcessName, fileVersionInfo?.ProductName, commandLineInfo)
                          {
                              Description = fileVersionInfo?.FileDescription,
                              Company = fileVersionInfo?.CompanyName,
                              Comments = fileVersionInfo?.Comments,
                              ParentId = parentId,
                              ParentName = parentName
                          };

        return processInfo;
    }
}
