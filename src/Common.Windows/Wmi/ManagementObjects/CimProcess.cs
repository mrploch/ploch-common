namespace Ploch.Common.Windows.Wmi.ManagementObjects;

/// <summary>
///     Represents an abstract base class for Common Information Model (CIM) processes.
/// </summary>
/// <remarks>
///     This class provides properties that describe process-related information, such as memory usage, execution state, and process metadata.
/// </remarks>
public abstract class CimProcess : CimLogicalElement
{
    /// <summary>
    ///     The scoping computer system's creation class name.
    /// </summary>
    public string? CreationClassName { get; set; }

    /// <summary>
    ///     The date and time the process began executing.
    /// </summary>
    public DateTime? CreationDate { get; set; }

    /// <summary>
    ///     The scoping computer system's creation class name.
    /// </summary>
    public string? CSCreationClassName { get; set; }

    /// <summary>
    ///     The scoping computer system's name.
    /// </summary>
    public string? CSName { get; set; }

    /// <summary>
    ///     The current operating condition of the process.
    /// </summary>
    public int ExecutionState { get; set; }

    /// <summary>
    ///     The process identifier (PID) assigned to this process.
    /// </summary>
    public string? Handle { get; set; }

    /// <summary>
    ///     The amount of time the process has executed in kernel mode.
    /// </summary>
    public long KernelModeTime { get; set; }

    /// <summary>
    ///     The operating system's creation class name.
    /// </summary>
    public string? OSCreationClassName { get; set; }

    /// <summary>
    ///     The name of the operating system.
    /// </summary>
    public string? OSName { get; set; }

    /// <summary>
    ///     The priority of the process.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    ///     The date and time the process was terminated.
    /// </summary>
    public DateTime? TerminationDate { get; set; }

    /// <summary>
    ///     The amount of time the process has executed in user mode.
    /// </summary>
    public long UserModeTime { get; set; }

    /// <summary>
    ///     The current size, in bytes, of the working set of the process.
    /// </summary>
    public long WorkingSetSize { get; set; }
}
