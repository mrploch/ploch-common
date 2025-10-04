using System;
using System.Collections.Generic;
using System.Linq;

namespace Ploch.Common.IO;

/// <summary>
///     Represents information about a command-line invocation, including the application path and its arguments.
/// </summary>
/// <remarks>
///     This class provides a structured representation of a command-line string,
///     separating the application path from its arguments for easier processing and usage.
/// </remarks>
public readonly struct CommandLineInfo(string? applicationPath, IEnumerable<string> arguments) : IEquatable<CommandLineInfo>
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

    /// <summary>
    ///     Determines whether two CommandLineInfo instances are equal.
    /// </summary>
    /// <param name="left">The first CommandLineInfo to compare.</param>
    /// <param name="right">The second CommandLineInfo to compare.</param>
    /// <returns>true if the CommandLineInfo instances are equal; otherwise, false.</returns>
    public static bool operator ==(CommandLineInfo left, CommandLineInfo right) => left.Equals(right);

    /// <summary>
    ///     Determines whether two CommandLineInfo instances are not equal.
    /// </summary>
    /// <param name="left">The first CommandLineInfo to compare.</param>
    /// <param name="right">The second CommandLineInfo to compare.</param>
    /// <returns>true if the CommandLineInfo instances are not equal; otherwise, false.</returns>
    public static bool operator !=(CommandLineInfo left, CommandLineInfo right) => !left.Equals(right);

    /// <summary>
    ///     Determines whether the specified object is equal to the current CommandLineInfo.
    /// </summary>
    /// <param name="obj">The object to compare with the current CommandLineInfo.</param>
    /// <returns>true if the specified object is equal to the current CommandLineInfo; otherwise, false.</returns>
    public override bool Equals(object? obj) => obj is CommandLineInfo other && Equals(other);

    /// <summary>
    ///     Determines whether the specified CommandLineInfo is equal to the current CommandLineInfo.
    /// </summary>
    /// <param name="other">The CommandLineInfo to compare with the current CommandLineInfo.</param>
    /// <returns>true if the specified CommandLineInfo is equal to the current CommandLineInfo; otherwise, false.</returns>
    public bool Equals(CommandLineInfo other) => ApplicationPath == other.ApplicationPath && Arguments.SequenceEqual(other.Arguments);

    /// <summary>
    ///     Returns a hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    public override int GetHashCode()
    {
        unchecked
        {
            return ((ApplicationPath != null ? ApplicationPath.GetHashCode() : 0) * 397) ^ (Arguments != null ? Arguments.GetHashCode() : 0);
        }
    }
}
