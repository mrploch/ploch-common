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

    /// <summary>
    ///     Appends or replaces the file extension of the specified path.
    /// </summary>
    /// <param name="path">The file path for which the extension will be appended or replaced.</param>
    /// <param name="extension">
    ///     The extension to append. This should include a leading dot (e.g., ".txt"). If the leading dot is omitted, it will be added
    ///     automatically.
    /// </param>
    /// <param name="replaceExistingExtension">Indicates whether to replace the existing file extension, if present. Defaults to <c>false</c>.</param>
    /// <param name="comparison">A <see cref="StringComparison" /> value used for extension matching. Defaults to <c>StringComparison.OrdinalIgnoreCase</c>.</param>
    /// <returns>The modified file path with the specified extension appended or replaced.</returns>
    public static string WithExtension(this string path,
                                       string extension,
                                       bool replaceExistingExtension = true,
                                       StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        path.NotNull(nameof(path));
        extension.NotNull(nameof(extension));

        // Cannot use patterns here because this library is built for .NET Standard 2.0
        // ReSharper disable once UsePattern
        if (!extension.StartsWith(".", StringComparison.Ordinal))
        {
            extension = "." + extension;
        }

        var currentExtension = Path.GetExtension(path);

        if (!currentExtension.Equals(extension, comparison) && replaceExistingExtension)
        {
            path = GetFullPathWithoutExtension(path);
        }

        // Cannot use patterns here because this library is built for .NET Standard 2.0
        // ReSharper disable once UsePattern
        if (path.EndsWith(".", StringComparison.Ordinal))
        {
            path = path.Substring(0, path.Length - 1);
        }

        return $"{path}{extension}";
    }

    /// <summary>
    ///     Gets the full path of a file without its extension.
    /// </summary>
    /// <remarks>
    ///     The result contains a full directory name (path) and the file name without the extension.
    ///     If provided path is null or empty, the result will be the same.
    /// </remarks>
    /// <param name="path">The full path of the file. Must not be null or empty.</param>
    /// <returns>The full path of the file without its extension.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path" /> is empty.</exception>
    public static string GetFullPathWithoutExtension(string path)
    {
        var directory = Path.GetDirectoryName(path);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);

        return string.IsNullOrEmpty(directory) ? fileNameWithoutExt : Path.Combine(directory, fileNameWithoutExt);
    }
}
