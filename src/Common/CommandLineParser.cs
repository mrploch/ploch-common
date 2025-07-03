using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common;

/// <summary>
///     Provides functionality for parsing command line strings into application paths and arguments.
/// </summary>
public static class CommandLineParser
{
    /// <summary>
    ///     Extracts the application path from a command line string.
    /// </summary>
    /// <param name="commandLineString">The command line string to parse.</param>
    /// <returns>The application path if found; otherwise, null.</returns>
    public static string? GetApplicationPath(string commandLineString)
    {
        var commandLine = GetCommandLine(commandLineString);

        return commandLine?.ApplicationPath;
    }

    /// <summary>
    ///     Parses a command line string treating the first part as an argument rather than the application path.
    /// </summary>
    /// <param name="commandLineString">The command line string to parse.</param>
    /// <returns>A <see cref="CommandLineInfo" /> object containing the parsed command line information; or null if parsing fails.</returns>
    public static CommandLineInfo? GetAsArguments(string commandLineString)
    {
        commandLineString.NotNull(nameof(commandLineString));

        var parsingContext = new ParsingContext { PathPartNumber = 1 };

        return GetCommandLine(commandLineString, parsingContext);
    }

    /// <summary>
    ///     Parses a command line string into application path and arguments.
    /// </summary>
    /// <param name="commandLineString">The command line string to parse.</param>
    /// <returns>A <see cref="CommandLineInfo" /> object containing the parsed command line information; or null if parsing fails.</returns>
    public static CommandLineInfo? GetCommandLine(string commandLineString) =>
        GetCommandLine(commandLineString.NotNull(nameof(commandLineString)), new ParsingContext());

    /// <summary>
    ///     Internal method that parses a command line string using the provided parsing context.
    /// </summary>
    /// <param name="commandLineString">The command line string to parse.</param>
    /// <param name="parsingContext">The context object used during parsing.</param>
    /// <returns>A <see cref="CommandLineInfo" /> object containing the parsed command line information; or null if parsing fails.</returns>
    private static CommandLineInfo? GetCommandLine(string commandLineString, ParsingContext parsingContext)
    {
        foreach (var c in commandLineString)
        {
            switch (c)
            {
                case '"':
                    parsingContext.Quoted = !parsingContext.Quoted;

                    continue;
                case ' ' when !parsingContext.Quoted:
                    ProcessCurrentPart(parsingContext);

                    continue;
                default:
                    parsingContext.CurrentPart.Append(c);

                    break;
            }
        }

        if (parsingContext.CurrentPart.Length > 0 || parsingContext.ApplicationFullPath != null || parsingContext.Arguments.Any())
        {
            ProcessCurrentPart(parsingContext);
        }

        if (parsingContext.ApplicationFullPath.IsNullOrEmpty() && !parsingContext.Arguments.Any())
        {
            return null;
        }

        return new CommandLineInfo(parsingContext.ApplicationFullPath, parsingContext.Arguments);
    }

    /// <summary>
    ///     Processes the current part being built during command line parsing.
    /// </summary>
    /// <param name="parsingContext">The context object used during parsing.</param>
    private static void ProcessCurrentPart(ParsingContext parsingContext)
    {
        if (parsingContext.CurrentPart.Length == 0)
        {
            return;
        }

        var partValue = parsingContext.CurrentPart.ToString().Trim();
        if (parsingContext.PathPartNumber == 0)
        {
            parsingContext.ApplicationFullPath = partValue;
        }
        else
        {
            parsingContext.Arguments.Add(partValue);
        }

        parsingContext.CurrentPart = new StringBuilder();
        parsingContext.PathPartNumber++;
    }

    /// <summary>
    ///     Represents the context used during command line parsing.
    /// </summary>
    private sealed class ParsingContext
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the parser is currently within quoted text.
        /// </summary>
        public bool Quoted { get; set; }

        /// <summary>
        ///     Gets or sets the full path to the application extracted from the command line.
        /// </summary>
        public string? ApplicationFullPath { get; set; }

        /// <summary>
        ///     Gets the list of arguments extracted from the command line.
        /// </summary>
        public IList<string> Arguments { get; } = [];

        /// <summary>
        ///     Gets or sets the current part number being processed (0 for application path, 1+ for arguments).
        /// </summary>
        public int PathPartNumber { get; set; }

        /// <summary>
        ///     Gets or sets the string builder used to accumulate the current part being parsed.
        /// </summary>
        public StringBuilder CurrentPart { get; set; } = new();
    }
}
