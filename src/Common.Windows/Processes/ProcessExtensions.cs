using System.ComponentModel;
using System.Diagnostics;
using Ardalis.Result;

namespace Ploch.Common.Windows.Processes;

public static class ProcessExtensions
{
    public static Result<ProcessModule?> TryGetMainModule(this Process process)
    {
        try
        {
            var module = process.MainModule;

            return module;
        }
        catch (Win32Exception ex)
        {
            Debug.WriteLine($"Failed to get main module for process ID {process.Id}: {ex.Message}, {ex.NativeErrorCode}");

            return Result.Error(ex.Message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            return Result.Error(ex.Message);
        }
    }

    public static Result<ProcessStartInfo?> TryGetProcessStartInfo(this Process process)
    {
        try
        {
            var startInfo = process.StartInfo;

            return startInfo;
        }
        catch (Win32Exception ex)
        {
            Debug.WriteLine($"Failed to get main module for process ID {process.Id}: {ex.Message}, {ex.NativeErrorCode}");
            Result.Error($"Failed to get process start info for process ID {process.Id}: {ex.Message}, {ex.NativeErrorCode}");
        }
        catch (InvalidOperationException ex)
        {
            return Result.Error($"Failed to get main module for process ID {process.Id}: {ex.Message!}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);

            return Result.Error($"Failed  to get main module for process ID {process.Id}: {ex.Message!}");
        }

        return Result.Error("");
    }
}
