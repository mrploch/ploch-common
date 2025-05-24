using System.Diagnostics;

namespace Ploch.Common.Windows.SystemApplications;

/// <summary>
///     Represents information about a system process.
/// </summary>
/// <param name="Id">The unique identifier of the process.</param>
/// <param name="Name">The name of the process.</param>
/// <param name="DisplayName">The display name of the process, if available.</param>
/// <param name="CommandLineInfo">Information about the command line used to start the process, if available.</param>
public record ProcessInfo(int Id, string Name, string? DisplayName, CommandLineInfo? CommandLineInfo) : SystemApplicationInfo(Name, DisplayName)
{
    /// <summary>
    ///     Gets or initializes the identifier of the parent process, if available.
    /// </summary>
    public int? ParentId { get; init; }

    /// <summary>
    ///     Gets or initializes the name of the parent process, if available.
    /// </summary>
    public string? ParentName { get; init; }

    /// <summary>
    ///     Gets or initializes the process start information, if available.
    /// </summary>
    public ProcessStartInfo? StartInfo { get; init; }

    /// <summary>
    ///     Gets or sets additional comments about the process.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    ///     Gets or sets the name of the company that created the process.
    /// </summary>
    public string? Company { get; set; }
}
