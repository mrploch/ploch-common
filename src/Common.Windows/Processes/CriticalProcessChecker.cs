using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ploch.Common.Windows;

public class CriticalProcessChecker
{
    private const int ProcessProtectionInformation = 61;

    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(IntPtr processHandle,
                                                        int processInformationClass,
                                                        ref PROCESS_PROTECTION_LEVEL_INFORMATION processInformation,
                                                        int processInformationLength,
                                                        out int returnLength);

    public static bool? TryGetIsCriticalProcess(Process process)
    {
        try
        {
            return IsCriticalProcess(process);
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            // Access denied, likely due to insufficient privileges
            return null;
        }
    }

    public static bool IsCriticalProcess(Process process)
    {
        var protectionInfo = new PROCESS_PROTECTION_LEVEL_INFORMATION();
        int returnLength;

        var status = NtQueryInformationProcess(process.Handle,
                                               ProcessProtectionInformation,
                                               ref protectionInfo,
                                               Marshal.SizeOf(protectionInfo),
                                               out returnLength);

        if (status != 0)
        {
            throw new Win32Exception(status);
        }

        // Check if the process has a protection level indicating it's critical
        return protectionInfo.ProtectionLevel > 0;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PROCESS_PROTECTION_LEVEL_INFORMATION
    {
        public byte ProtectionLevel;
        public byte Signer;
        public byte Reserved1;
        public byte Reserved2;
    }
}
