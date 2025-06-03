namespace Ploch.Common.Windows.Processes;

/// <summary>
///     Represents the properties of a Windows process.
/// </summary>
/// <param name="Id">The unique identifier of the process.</param>
/// <param name="Caption">The caption or title of the process window, if available.</param>
/// <param name="CommandLine">The command line used to start the process, if available.</param>
/// <param name="Description">The description of the process, if available.</param>
/// <param name="Name">The name of the process.</param>
/// <param name="ExecutablePath">The full path to the executable file of the process, if available.</param>
/// <param name="ParentProcess">The properties of the parent process, if available.</param>
public record ProcessProperties(int Id,
                                string? Caption,
                                string? CommandLine,
                                string? Description,
                                string Name,
                                string? ExecutablePath,
                                ProcessProperties? ParentProcess);
