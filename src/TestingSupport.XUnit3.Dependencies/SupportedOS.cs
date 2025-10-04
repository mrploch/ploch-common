namespace Ploch.TestingSupport.XUnit3.Dependencies;

/// <summary>
///     Represents an enumeration of operating systems that can be explicitly targeted for compatibility or support.
///     This enumeration is used to specify a target platform, for example, when controlling test execution
///     based on the operating system on which the test is running.
/// </summary>
public enum SupportedOS
{
    FreeBSD = 1,
    Linux = 2,
    macOS = 3,
    Windows = 4
}
