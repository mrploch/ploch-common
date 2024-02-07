using System.Reflection;

namespace Ploch.Common;

/// <summary>
/// Represents information about an assembly.
/// </summary>
public class AssemblyInformation(string product, string description, string version)
{
    /// <summary>
    /// Initializes a new instance of the AssemblyInformation class with the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly to retrieve information from.</param>
    public AssemblyInformation(Assembly assembly) : this(assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product,
                                                         assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()!.Description,
                                                         assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version)
    { }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    /// <value>
    /// The product name.
    /// </value>
    public string Product { get; } = product;

    /// <summary>
    /// Gets the description of the property.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; } = description;

    /// <summary>
    /// Gets the version of the software.
    /// </summary>
    /// <value>
    /// The version of the software.
    /// </value>
    public string Version { get; } = version;
}