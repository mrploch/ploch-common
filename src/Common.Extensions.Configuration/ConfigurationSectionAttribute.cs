namespace Ploch.Common.Extensions.Configuration;

/// <summary>
///     Represents an attribute that specifies the configuration section name
///     for a class to associate it with a configuration section.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ConfigurationSectionAttribute(string sectionName) : Attribute
{
    /// <summary>
    ///     Gets the name of the configuration section that this attribute is associated with.
    /// </summary>
    /// <remarks>
    ///     This property retrieves the section name specified when the <see cref="ConfigurationSectionAttribute" />
    ///     is applied to a class. It is used to map a class to a specific section in the configuration file.
    /// </remarks>
    public string SectionName => sectionName;
}
