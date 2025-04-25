namespace Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

/// <summary>
///     Represents an attribute used to specify the Windows Management Instrumentation (WMI) class name for a C# class.
/// </summary>
/// <remarks>
///     This attribute is used to associate a C# class with a specific WMI class, allowing for easier mapping and querying of WMI objects.
/// </remarks>
/// <param name="className">The name of the WMI class to associate with the decorated C# class.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class WindowsManagementClassAttribute(string className) : Attribute
{
    /// <summary>
    ///     Gets the name of the WMI class associated with this attribute.
    /// </summary>
    /// <value>A string representing the WMI class name.</value>
    public string ClassName { get; } = className.NotNull();

    /// <summary>
    ///     Gets or sets the namespace of the associated WMI class.
    /// </summary>
    /// <value>A string representing the namespace of the WMI class.</value>
    public string? Namespace { get; set; }
}
