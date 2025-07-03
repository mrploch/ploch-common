using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Ploch.Common;

/// <summary>
///     Utility class for common operations related to the environment.
/// </summary>
public static class EnvironmentUtilities
{
    /// <summary>
    ///     Returns the current application's directory path.
    /// </summary>
    /// <returns>The current application's directory path.</returns>
    public static string GetCurrentAppPath()
    {
        return Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location ?? AppDomain.CurrentDomain.BaseDirectory) ??
               throw new InvalidOperationException("Could not get entry assembly name, one of the components was null");
    }

    /// <summary>
    ///     Retrieves the command line arguments passed to the application.
    /// </summary>
    /// <param name="includeApplication">
    ///     If set to true, includes the name of the application in the returned argument list.
    ///     Default is false.
    /// </param>
    /// <returns>An enumerable collection of command line arguments.</returns>
    public static IEnumerable<string> GetEnvironmentCommandLine(bool includeApplication = false)
    {
        var args = Environment.CommandLine.Split(Chars.Space);

        return includeApplication ? args : args.Skip(1);
    }
}
