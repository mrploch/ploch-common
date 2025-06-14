using System.Collections.Generic;

namespace Ploch.Common;

/// <summary>
///     Represents information about a command-line invocation, including the application path and its arguments.
/// </summary>
/// <remarks>
///     This class provides a structured representation of a command-line string,
///     separating the application path from its arguments for easier processing and usage.
/// </remarks>
public readonly struct CommandLineInfo(string? applicationPath, IEnumerable<string> arguments)
{
    /// <summary>
    ///     Gets the path to the executable file that was used to start the application.
    /// </summary>
    /// <remarks>
    ///     This property represents the full file path of the executable that initiated the process.
    ///     It is commonly used to identify or verify the application being executed, particularly in scenarios
    ///     where processes are being monitored, logged, or analyzed. The value may be null if the application path
    ///     cannot be determined.
    /// </remarks>
    /// <value>
    ///     A <see cref="string" /> containing the full path of the application executable, or <c>null</c>
    ///     if the path is unavailable.
    /// </value>
    /// <example>
    ///     The following demonstrates how this property might be utilized:
    ///     Suppose the command-line string is:
    ///     <c>"C:\Test\MyApp.exe" -arg1 value1</c>.
    ///     In this case, the <c>ApplicationPath</c> property will return:
    ///     <c>"C:\Test\MyApp.exe"</c>.
    /// </example>
    public string? ApplicationPath => applicationPath;

    /// <summary>
    ///     Gets the collection of arguments extracted from the command-line invocation.
    /// </summary>
    /// <remarks>
    ///     The property returns all command-line arguments passed to the application, excluding the path to the application itself.
    ///     These arguments are typically used to modify the behavior or configuration of the application at runtime.
    ///     The returned collection provides a structured way to access the arguments, making it easier for developers
    ///     to process and utilize them within their application logic.
    /// </remarks>
    /// <value>
    ///     An <see cref="IEnumerable{T}" /> containing strings that represent the arguments supplied to the application.
    ///     If no arguments were provided, the returned collection will be empty.
    /// </value>
    /// <example>
    ///     The following example demonstrates accessing the arguments property:
    ///     Suppose the command-line string is:
    ///     <c>"C:\Program Files\MyApp\MyApp.exe" -arg1 value1 -arg2 value2</c>.
    ///     In this case:
    ///     <c>ApplicationPath</c> will be <c>"C:\Program Files\MyApp\MyApp.exe"</c>,
    ///     and the <c>Arguments</c> property will contain <c>["-arg1", "value1", "-arg2", "value2"]</c>.
    /// </example>
    public IEnumerable<string> Arguments => arguments;
}
