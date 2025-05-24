namespace Ploch.Common.Windows.Processes;

/// <summary>
///     Represents the properties of a Windows process, providing detailed information
///     such as its caption, command line, description, name, and executable path.
/// </summary>
public record ProcessProperties(
    int Id,
    string? Caption,
    string? CommandLine,
    string? Description,
    string Name,
    string? ExecutablePath,
    ProcessProperties? ParentProcess);
