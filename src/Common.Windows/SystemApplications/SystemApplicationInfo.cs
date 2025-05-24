using System.Diagnostics;

namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     Represents base information about a system application such as processes or services.
/// </summary>
/// <param name="Name">The name of the system application.</param>
/// <param name="DisplayName">The display name of the system application, if available.</param>
public record SystemApplicationInfo(string Name, string? DisplayName)
{
    /// <summary>
    ///     Gets or initializes the file path of the system application, if available.
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    ///     Gets or initializes the description of the system application, if available.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Gets or initializes the file version information of the system application, if available.
    /// </summary>
    public FileVersionInfo? FileVersionInfo { get; init; }

    /// <summary>
    ///     Gets or initializes a value indicating whether the system application is currently running.
    /// </summary>
    public bool IsRunning { get; init; }
}
