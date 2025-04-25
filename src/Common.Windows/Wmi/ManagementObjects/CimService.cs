namespace Ploch.Common.Windows.Wmi.ManagementObjects;

/// <summary>
///     Represents a service in the Common Information Model (CIM).
/// </summary>
/// <remarks>
///     This class provides properties that describe a service, including its state, start mode, and system associations.
///     It is based on the CIM_Service class.
/// </remarks>
public class CimService : CimLogicalElement
{
    /// <summary>
    ///     The scoping computer system's creation class name.
    /// </summary>
    public string? CreationClassName { get; set; }

    /// <summary>
    ///     Indicates whether the service has been started.
    /// </summary>
    public bool Started { get; set; }

    /// <summary>
    ///     The start mode of the service (e.g., Auto, Manual, Disabled).
    /// </summary>
    public ServiceStartMode? StartMode { get; set; }

    /// <summary>
    ///     The creation class name of the scoping system.
    /// </summary>
    public string? SystemCreationClassName { get; set; }

    /// <summary>
    ///     The name of the scoping system.
    /// </summary>
    public string? SystemName { get; set; }
}
