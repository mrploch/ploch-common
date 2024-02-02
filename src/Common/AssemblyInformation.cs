using System.Reflection;

namespace Ploch.Common;

public class AssemblyInformation(string product, string description, string version)
{
    public static readonly AssemblyInformation Current = new(typeof(AssemblyInformation).Assembly);

    public AssemblyInformation(Assembly assembly) : this(assembly.GetCustomAttribute<AssemblyProductAttribute>()!.Product,
                                                         assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()!.Description,
                                                         assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()!.Version)
    { }

    public string Product { get; } = product;

    public string Description { get; } = description;

    public string Version { get; } = version;
}