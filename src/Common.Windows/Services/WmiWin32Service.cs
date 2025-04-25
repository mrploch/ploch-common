namespace Ploch.Common.Windows.Services;

public class WmiWin32Service
{
    public const string ClassName = "Win32_Service";

    public bool AcceptPause { get; set; }

    public bool AcceptStop { get; set; }

    public string? Caption { get; set; }

    public int CheckPoint { get; set; }

    public string? CreationClassName { get; set; }

    public bool DelayedAutoStart { get; set; }

    public string? Description { get; set; }

    public bool DesktopInteract { get; set; }

    public string? DisplayName { get; set; }

    public string? ErrorControl { get; set; }

    public int ExitCode { get; set; }

    public DateTime InstallDate { get; set; }

    public string? Name { get; set; }

    public string? PathName { get; set; }

    public int ProcessId { get; set; }

    public int ServiceSpecificExitCode { get; set; }

    public string? ServiceType { get; set; }

    public bool Started { get; set; }

    public string? StartMode { get; set; }

    public string? StartName { get; set; }

    public string? State { get; set; }

    public string? Status { get; set; }

    public string? SystemCreationClassName { get; set; }

    public string? SystemName { get; set; }

    public int TagId { get; set; }

    public int WaitHint { get; set; }
}
