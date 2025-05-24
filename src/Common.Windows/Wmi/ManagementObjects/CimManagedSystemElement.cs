namespace Ploch.Common.Windows.Wmi.ManagementObjects;

/// <summary>
///     Represents the base class for all system elements managed by the Common Information Model (CIM).
/// </summary>
/// <remarks>
///     This class provides common properties for system elements, such as name, description, and status.
///     It serves as a foundation for more specific managed system element classes.
/// </remarks>
public abstract class CimManagedSystemElement
{
    /// <summary>
    ///     A short textual description of the object.
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    ///     A textual description of the object.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The date and time the object was installed.
    /// </summary>
    public DateTime? InstallDate { get; set; }

    /// <summary>
    ///     The name of the object.
    /// </summary>
    public string Name { get; set; } = null!;
}
