using System.Diagnostics;

namespace Ploch.Common.Windows.Tests;

public static class ProcessShutdownHelper
{
    public static async Task<bool> TryGracefulShutdownAsync(int processId, CancellationToken cancellationToken = default)
    {
        var process = Process.GetProcessById(processId);

        return await TryGracefulShutdownAsync(process, cancellationToken);
    }

    public static async Task<bool> TryGracefulShutdownAsync(Process process, CancellationToken cancellationToken = default)
    {
        try
        {
            if (process.HasExited)
            {
                return true;
            }

            if (process.CloseMainWindow())
            {
                // Wait for the process to exit gracefully
                await process.WaitForExitAsync(cancellationToken);

                return true;
            }

            // No main window to close, cannot shut down gracefully
            return false;
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Error during graceful shutdown of process {process.Id}: {ex.Message}");

            return false;
        }
    }

    public static void KillProcess(int processId)
    {
        try
        {
            var process = Process.GetProcessById(processId);
            if (!process.HasExited)
            {
                process.Kill(true);
            }
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Error killing process with ID '{processId}': {ex.Message}");
        }
    }

    public static bool KillProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill(true);

                return true;
            }
        }
        catch (Exception ex)
        {
            Holder.Output.WriteLine($"Error killing process '{process.ProcessName}': {ex.Message}");
        }

        return false;
    }
}