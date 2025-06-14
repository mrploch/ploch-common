using System.IO;
using System.Linq;
using System.Text;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.IO;

/// <summary>
///     Utility class for manipulating path related operations.
/// </summary>
public static class PathUtils
{
    /// <summary>
    ///     Gets the (short) name of the directory.
    /// </summary>
    /// <param name="directoryPath">Directory path.</param>
    /// <returns>The name of the directory.</returns>
    public static string GetDirectoryName(string directoryPath)
    {
        directoryPath.NotNullOrEmpty(nameof(directoryPath));

        return new DirectoryInfo(directoryPath).Name;
    }

    /// <summary>
    ///     Converts the input string to a safe file name by replacing invalid characters with an underscore.
    /// </summary>
    /// <param name="input">The input string to be transformed into a safe file name.</param>
    /// <returns>The transformed string that is safe to use as a file name.</returns>
    public static string ToSafeFileName(string input)
    {
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

        return normalizedPath + Path.DirectorySeparatorChar;
    }

    /// <summary>
    ///     Normalizes the given path and removes any trailing directory separator characters.
    /// </summary>
    /// <param name="path">The path to normalize.</param>
    /// <returns>The normalized path without trailing directory separator characters.</returns>
    public static string NormalizePathWithoutTrailingSeparator(string path)
    {
        path.NotNullOrEmpty(nameof(path));

        return Path.GetFullPath(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }
}
