namespace Ploch.Common.Windows.SystemApplications;

public class ProcessResouresInfo
{
    public int Id { get; set; }

    public int ProcessId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? CommandLine { get; set; }

    public int ParentId { get; set; }

    public int ThreadCount { get; set; }

    public int HandleCount { get; set; }
}
