namespace Ploch.Common.Windows.Tests;

public class DumpEntry
{
    public string ProcessName { get; set; } = null!;

    public string? ProcessDisplayName { get; set; }

    public string? Path { get; set; }

    public string? ServiceName { get; set; }

    public string? ServiceDisplayName { get; set; }

    public string? ProcessParentName { get; set; }

    public string? ParentProcessDisplayName { get; set; }

    public string? ParentProcessPath { get; set; }

    public ActionType? ActionType { get; set; }

    public bool? IsCritical { get; set; }

    public override string ToString() =>
        $"{nameof(ProcessName)}: {ProcessName}, {nameof(ProcessDisplayName)}: {ProcessDisplayName}, {nameof(Path)}: {Path}, {nameof(ServiceName)}: {ServiceName}, {nameof(ServiceDisplayName)}: {ServiceDisplayName}, {nameof(ProcessParentName)}: {ProcessParentName}, {nameof(ParentProcessDisplayName)}: {ParentProcessDisplayName}, {nameof(ParentProcessPath)}: {ParentProcessPath}, {nameof(ActionType)}: {ActionType}, {nameof(IsCritical)}: {IsCritical}";
}