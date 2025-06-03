using System.Runtime.InteropServices;
using System.Text;

namespace Ploch.Common.Windows.Processes;

/// <summary>
///     Provides helper methods for retrieving information related to process window titles.
/// </summary>
public static class ProcessWindowTitleHelper
{
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    /// <summary>
    ///     Retrieves the title of the main window for a specified process.
    /// </summary>
    /// <param name="processId">The ID of the process for which to find the main window title.</param>
    /// <returns>
    ///     The title of the main window if found; otherwise, <see langword="null" /> if the process
    ///     has no visible main window or if the window title could not be retrieved.
    /// </returns>
    /// <remarks>
    ///     This method enumerates all top-level windows and finds the first visible window
    ///     that belongs to the specified process.
    /// </remarks>
    public static string? GetMainWindowTitle(int processId)
    {
        var mainWindowHandle = IntPtr.Zero;

        EnumWindows(delegate(IntPtr hWnd, IntPtr lParam)
                    {
                        GetWindowThreadProcessId(hWnd, out var windowProcessId);
                        if (windowProcessId == processId && IsWindowVisible(hWnd))
                        {
                            // Optionally check if window is main window (no owner)
                            mainWindowHandle = hWnd;

                            return false; // stop enumerating
                        }

                        return true; // continue enumerating
                    },
                    IntPtr.Zero);

        if (mainWindowHandle == IntPtr.Zero)
        {
            return null;
        }

        var sb = new StringBuilder(1024);
        GetWindowText(mainWindowHandle, sb, sb.Capacity);

        return sb.ToString();
    }

    // Delegate for EnumWindows
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
}
