using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Wmi.ManagementObjects;

public enum ServiceAccontType
{
    [WindowsManagementObjectEnumMapping("LocalSystem")]
    LocalSystem,

    [WindowsManagementObjectEnumMapping("NT AUTHORITY\\LocalService")]
    NtAuthorityLocalService,

    [WindowsManagementObjectEnumMapping("NT AUTHORITY\\NetworkService")]
    NtAuthorityNetworkService,

    [WindowsManagementObjectEnumMapping("NT SERVICE\\himds")]
    NtServicEhimds,

    [WindowsManagementObjectEnumMapping("NT Service\\MsDtsServer160")]
    NtServiceMsDtsServer160,

    [WindowsManagementObjectEnumMapping("NT Service\\MSSQLFDLauncher")]
    NtServiceMssqlfdLauncher,

    [WindowsManagementObjectEnumMapping("NT Service\\MSSQLLaunchpad")]
    NtServiceMssqlLaunchpad,

    [WindowsManagementObjectEnumMapping("NT Service\\MSSQLSERVER")]
    NtServiceMssqlserver,

    [WindowsManagementObjectEnumMapping("NT Service\\SQLSERVERAGENT")]
    NtServiceSqlserveragent,

    [WindowsManagementObjectEnumMapping("NT Service\\SQLTELEMETRY")]
    NtServiceSqltelemetry,

    [WindowsManagementObjectEnumMapping("NT Service\\SSISTELEMETRY160")]
    NtServiceSsistelemetry160,

    [WindowsManagementObjectEnumMapping]
    Empty
}
