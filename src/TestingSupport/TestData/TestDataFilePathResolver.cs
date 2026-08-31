using System;
using System.IO;

namespace Ploch.TestingSupport.TestData;

/// <summary>
///     Resolves the path of a test data file supplied to a data attribute.
/// </summary>
/// <remarks>
///     This file is compiled into both <c>Ploch.TestingSupport</c> and <c>Ploch.TestingSupport.XUnit3</c> - the XUnit3
///     project links it rather than keeping a copy, so the two packages cannot drift apart on how a path is resolved.
/// </remarks>
internal static class TestDataFilePathResolver
{
    private static readonly bool IsWindowsPathSyntax = Path.DirectorySeparatorChar != '/';

    /// <summary>
    ///     Resolves <paramref name="filePath" /> to an absolute path.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Only a <em>fully qualified</em> path is used as given. Every other form is anchored to
    ///         <see cref="AppContext.BaseDirectory" /> - the directory the consuming test assembly was loaded from, which
    ///         is where content files marked <c>CopyToOutputDirectory</c> are placed. Anchoring to the process working
    ///         directory instead would make data loading depend on how the test host happened to be launched.
    ///     </para>
    ///     <para>
    ///         "Fully qualified" is a stricter test than "rooted". On Windows <c>"\TestData\cases.txt"</c>,
    ///         <c>"/TestData/cases.txt"</c> and the drive-relative <c>"C:cases.txt"</c> are all rooted yet still depend on
    ///         ambient state - the current drive and that drive's current directory - so they are treated as partially
    ///         rooted: the leading root is stripped and the remainder is anchored to the assembly directory. On
    ///         Unix-like platforms rooted and fully qualified mean the same thing, so a path starting with <c>/</c> is
    ///         used as given.
    ///     </para>
    /// </remarks>
    /// <param name="filePath">The path to the test data file. Fully qualified, or relative to the test assembly's directory.</param>
    /// <returns>The normalised absolute path to the test data file.</returns>
    public static string Resolve(string filePath)
    {
        if (IsPathFullyQualified(filePath))
        {
            return Path.GetFullPath(filePath);
        }

        return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, StripPartialRoot(filePath)));
    }

    private static bool IsPathFullyQualified(string filePath)
    {
#if NETSTANDARD2_0

        // Path.IsPathFullyQualified does not exist on netstandard2.0; this mirrors the framework implementation.
        if (!IsWindowsPathSyntax)
        {
            // On Unix-like platforms a path is fully qualified exactly when it is rooted.
            return Path.IsPathRooted(filePath);
        }

        if (filePath.Length < 2)
        {
            // "" and a single character (including "\" and "C") always depend on ambient state.
            return false;
        }

        if (IsDirectorySeparator(filePath[0]))
        {
            // UNC ("\\server\share") and device ("\\?\C:\") paths; a single leading separator is drive-relative.
            return filePath[1] == '?' || IsDirectorySeparator(filePath[1]);
        }

        return filePath.Length >= 3 && filePath[1] == Path.VolumeSeparatorChar && IsDirectorySeparator(filePath[2]) && IsValidDriveChar(filePath[0]);
#else
        return Path.IsPathFullyQualified(filePath);
#endif
    }

    /// <summary>
    ///     Removes the ambient-state-dependent root - a drive qualifier and any leading separators - from a path that is
    ///     rooted but not fully qualified, so that what is left can be anchored to the assembly directory.
    /// </summary>
    /// <param name="filePath">The path to strip.</param>
    /// <returns>The path with any ambient-state-dependent root removed.</returns>
    private static string StripPartialRoot(string filePath)
    {
        if (!IsWindowsPathSyntax)
        {
            // A path that reaches here on a Unix-like platform is genuinely relative: a leading "\" is an ordinary
            // filename character there and must not be stripped.
            return filePath;
        }

        var start = 0;
        if (filePath.Length >= 2 && filePath[1] == Path.VolumeSeparatorChar && IsValidDriveChar(filePath[0]))
        {
            start = 2;
        }

        while (start < filePath.Length && IsDirectorySeparator(filePath[start]))
        {
            start++;
        }

        return start == 0 ? filePath : filePath.Substring(start);
    }

    private static bool IsDirectorySeparator(char character) => character == Path.DirectorySeparatorChar || character == Path.AltDirectorySeparatorChar;

    private static bool IsValidDriveChar(char character) => (character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z');
}
