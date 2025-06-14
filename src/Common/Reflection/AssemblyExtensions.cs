using System;
using System.IO;
using System.Reflection;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.Reflection;

/// <summary>
///     Extension utility methods for an <see cref="Assembly" />.
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    ///     Gets the directory name where assembly is located
    /// </summary>
    /// <param name="assembly">The assembly</param>
    /// <returns>The directory name where assembly is located.</returns>
    public static string? GetAssemblyDirectory(this Assembly assembly)
    {
        assembly.NotNull(nameof(assembly));

        var agentAssemblyPath = assembly.Location ?? throw new InvalidOperationException("Assembly location is null.");

        return Path.GetDirectoryName(agentAssemblyPath);
    }
}
