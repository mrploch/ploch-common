using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Ploch.Common.Windows.Processes;

[SupportedOSPlatform("windows")]
public class CriticalProcessChecker
{
    private const int ProcessProtectionInformation = 61;

    public static bool TryGetIsCriticalProcess(Process process, out bool isCriticalProcess)
    {
        try
        {
            isCriticalProcess = IsCriticalProcess(process);

            return true;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            // Access denied, likely due to insufficient privileges
            isCriticalProcess = false;

            return false;
        }
    }

    public static bool IsCriticalProcess(Process process)
    {
        var protectionInfo = new PROCESS_PROTECTION_LEVEL_INFORMATION();

        var status = NtQueryInformationProcess(process.Handle,
                                               ProcessProtectionInformation,
                                               ref protectionInfo,
                                               Marshal.SizeOf(protectionInfo),
                                               out var returnLength);

        if (status != 0)
        {
            throw new Win32Exception(status);
        }

        // Check if the process has a protection level indicating it's critical
        return protectionInfo.ProtectionLevel > 0;
    }

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle,
                                                        int processInformationClass,
                                                        ref PROCESS_PROTECTION_LEVEL_INFORMATION processInformation,
                                                        int processInformationLength,
                                                        out int returnLength);

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_PROTECTION_LEVEL_INFORMATION
    {
        public byte ProtectionLevel;
        public byte Signer;
        public byte Reserved1;
        public byte Reserved2;
    }
}
