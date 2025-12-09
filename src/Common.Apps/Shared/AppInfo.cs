namespace Ploch.CommandLine.Spectre;

/// <summary>
///     Represents information about a console application, including its name, description, version, and display settings.
/// </summary>
/// <remarks>
///     Initializes a new instance of the <see cref="AppInfo" /> class.
/// </remarks>
/// <param name="args">Optional command-line arguments passed to the application.</param>
public class AppInfo(params IEnumerable<string>? args)
{
    /// <summary>
    ///     Gets or sets the name of the application.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the application.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the version of the application.
    /// </summary>
    public Version? Version { get; set; }

    /// <summary>
    ///     Gets the command-line arguments passed to the application.
    /// </summary>
    public IEnumerable<string>? Args { get; } = args;
}
