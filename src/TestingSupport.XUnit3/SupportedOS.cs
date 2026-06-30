using System.Diagnostics.CodeAnalysis;

namespace Ploch.TestingSupport.XUnit3;

/// <summary>
///     Represents an enumeration of operating systems that can be explicitly targeted for compatibility or support.
///     This enumeration is used to specify a target platform, for example, when controlling test execution
///     based on the operating system on which the test is running.
/// </summary>
public enum SupportedOS
{
    /// <summary>
    ///     The FreeBSD operating system.
    /// </summary>
    FreeBSD = 1,

    /// <summary>
    ///     The Linux operating system.
    /// </summary>
    Linux = 2,

    /// <summary>
    ///     The macOS operating system.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300", Justification = "macOS is the canonical casing of the platform name; renaming would break the public API.")]
    macOS = 3,

    /// <summary>
    ///     The Windows operating system.
    /// </summary>
    Windows = 4
}
