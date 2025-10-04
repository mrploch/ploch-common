using System;
using System.IO;
using System.Linq;
using System.Text;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.IO;

/// <summary>
///     Utility class for manipulating path-related operations.
/// </summary>
public static class PathUtils
{
    /// <summary>
    ///     Gets the (short) name of the directory.
    /// </summary>
    /// <param name="directoryPath">Directory path.</param>
    /// <returns>The name of the directory.</returns>
    public static string GetDirectoryName(this string directoryPath)
    {
        directoryPath.NotNullOrEmpty(nameof(directoryPath));

        return new DirectoryInfo(directoryPath).Name;
    }

    /// <summary>
    ///     Converts the input string to a safe file name by replacing invalid characters with an underscore.
    /// </summary>
    /// <param name="input">The input string to be transformed into a safe file name. Must not be null or empty.</param>
    /// <returns>The transformed string that is safe to use as a file name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="input" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="input" /> is empty.</exception>
    public static string ToSafeFileName(string input)
    {
        input.NotNullOrEmpty(nameof(input));

        var invalidChars = Path.GetInvalidFileNameChars().ToList();
        var builder = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            builder.Append(invalidChars.Contains(c) ? '_' : c);
        }

        return builder.ToString();
    }

    /// <summary>
    ///     Normalizes a given path and ensures it includes a trailing directory separator.
    /// </summary>
    /// <param name="path">The path to normalize, which must not be null or empty.</param>
    /// <returns>The normalized path, always ending with a trailing directory separator.</returns>
    public static string NormalizePathWithTrailingSeparator(string path)
    {
        path.NotNullOrEmpty(nameof(path));

        var normalizedPath = Path.GetFullPath(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        return $"{normalizedPath}{Path.DirectorySeparatorChar}";
    }

    /// <summary>
    ///     Normalizes the given path and removes any trailing directory separator characters.
    /// </summary>
    /// <remarks>
    ///     Besides removing the trailing directory separator, on a Windows platform, it would also normalize separators to the backslash,
    ///     in case they are mixed in the path.
    ///     For example, on Windows it would change:
    ///     <code>
    /// c:\mypath/somefolder\some-other-folder
    /// </code>
    ///     to:
    ///     <code>
    /// c:\mypath\somefolder\some-other-folder
    /// </code>
    ///     On a platform like macOS, forward slash (<c>/</c>) is a legal character in directory or file names, which means
    ///     they would not be changed.
    /// </remarks>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path without trailing directory separator characters.</returns>
    public static string NormalizePathWithoutTrailingSeparator(string path)
    {
        path.NotNullOrEmpty(nameof(path));

        return Path.GetFullPath(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    /// <summary>
    ///     Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fromPath" /> or <paramref name="toPath" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException"><paramref name="fromPath" /> or <paramref name="toPath" /> is empty or not a valid path string.</exception>
    public static string GetRelativePath(string fromPath, string toPath)
    {
        fromPath.NotNullOrEmpty(nameof(fromPath));
        toPath.NotNullOrEmpty(nameof(toPath));

        var fromUri = new Uri(NormalizePathWithTrailingSeparator(fromPath));
        var toUri = new Uri(toPath);

        if (fromUri.Scheme != toUri.Scheme)
        {
            return toPath;
        }

        var relativeUri = fromUri.MakeRelativeUri(toUri);
        var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        return relativePath;
    }
}
