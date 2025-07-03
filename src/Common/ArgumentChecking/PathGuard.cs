// ReSharper disable RedundantUsingDirective

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Ploch.Common.ArgumentChecking;

/// <summary>
///     Provides methods for validating and ensuring the validity of file system paths.
/// </summary>
/// <remarks>
///     This class contains utility methods to validate paths, ensuring they are not null, empty,
///     contain invalid characters, or are not rooted. It is designed to simplify and standardize
///     path validation logic across the application.
/// </remarks>
public static partial class PathGuard
{
    private static bool CheckIsValidPath(string path) => path.IndexOfAny(Path.GetInvalidPathChars()) < 0;
#if NETSTANDARD2_0
    /// <summary>
    ///     Ensures that the specified path is valid, throwing an exception if the path is null, empty, not rooted, or contains invalid characters.
    /// </summary>
    /// <param name="path">The path to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>The validated path.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="path" /> is null or empty.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the <paramref name="path" /> is not rooted or contains invalid characters.
    /// </exception>
    public static string RequireValidPath(this string? path, string parameterName)
    {
        path.RequiredNotNullOrEmpty(nameof(path));

        if (!Path.IsPathRooted(path))
        {
            throw new ArgumentException("Path must be rooted.", parameterName);
        }

        if (Path.GetInvalidPathChars().Any(path.Contains))
        {
            throw new ArgumentException("Path contains invalid characters.", parameterName);
        }
#pragma warning disable CS8603 // Possible null reference return - false positive

        return path;
#pragma warning restore CS8603
    }

    /// <summary>
    ///     Ensures that the specified file path corresponds to an actual existing file, throwing an exception if the file does not exist.
    /// </summary>
    /// <param name="path">The file path to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>The validated file path.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="path" /> is null or empty.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the <paramref name="path" /> does not exist or the path is not rooted or contains invalid characters.
    /// </exception>
    public static string EnsureFileExists(this string? path, string parameterName)
    {
        path.RequireValidPath(parameterName);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"The file at the provided path does not exist: {path}", parameterName);
        }
#pragma warning disable CS8603 // Possible null reference return - false positive

        return path;
#pragma warning restore CS8603
    }

    /// <summary>
    ///     Validates that the provided string is a valid file system path.
    /// </summary>
    /// <param name="path">The path string to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated (automatically captured).</param>
    /// <returns>The original path string if it is valid.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the path is null, empty, consists only of white-space characters,
    ///     or contains invalid characters or is in an invalid format.
    /// </exception>
    public static string IsValidPath(this string? path, string parameterName)
    {
        path.NotNullOrEmpty(parameterName);

        // Check for invalid path characters
        if (!CheckIsValidPath(path!))
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, InvalidPathMessageFormat, path));
        }

        // Optionally, check for reserved device names or other platform-specific rules here
#pragma warning disable CS8603 // Possible null reference return - false positive
        return path;
#pragma warning restore CS8603
    }
#endif
}
