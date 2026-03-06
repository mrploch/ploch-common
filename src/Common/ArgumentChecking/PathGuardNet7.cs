// ReSharper disable RedundantUsingDirective

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

namespace Ploch.Common.ArgumentChecking;

// ReSharper disable once MismatchedFileName - this file is specifically for .NET 7.0 and later versions, file name is changed intentionally.
/// <summary>
///     Provides utilities for validating file system paths and their properties.
/// </summary>
public static partial class PathGuard
{
    private const string InvalidPathMessageFormat = "The provided path contains invalid characters: {0}, parameter name: {1}";
#if NET7_0_OR_GREATER
    private const string PathDoesNotExistMessageFormat = "The path does not exist: {0}";

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
    public static string IsValidPath(this string? path, [CallerArgumentExpression(nameof(path))] string? parameterName = null)
    {
        path.NotNullOrEmpty(parameterName);

        // Check for invalid path characters
        if (!CheckIsValidPath(path))
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, InvalidPathMessageFormat, path, parameterName));
        }

        return path;
    }

    public static string RequiredIsValidPath(this string? path, [CallerArgumentExpression(nameof(path))] string? parameterName = null)
    {
        path.RequiredNotNullOrEmpty();

        if (!CheckIsValidPath(path))
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, InvalidPathMessageFormat, path, parameterName));
        }

        return path;
    }

    /// <summary>
    ///     Validates that the provided string is a valid file system path and that a file exists at that location.
    /// </summary>
    /// <param name="path">The path string to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated (automatically captured).</param>
    /// <returns>The original path string if it is valid and a file exists at the specified location.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the path is invalid or when no file exists at the specified path.
    /// </exception>
    public static string EnsureFileExists(this string? path, [CallerArgumentExpression(nameof(path))] string? parameterName = null)
    {
        if (!File.Exists(path.IsValidPath(parameterName)))
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, PathDoesNotExistMessageFormat, path), parameterName);
        }
#pragma warning disable CS8603 // Possible null reference return - false positive

        return path;
#pragma warning restore CS8603
    }

    /// <summary>
    ///     Validates that the provided string is a valid file system path and throws an <see cref="InvalidOperationException" /> if invalid.
    /// </summary>
    /// <param name="path">The path string to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated (automatically captured).</param>
    /// <returns>The original path string if it is valid.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when the path is null, empty, consists only of white-space characters,
    ///     or contains invalid characters or is in an invalid format.
    /// </exception>
    public static string RequiredFileExists(this string? path, [CallerArgumentExpression(nameof(path))] string? parameterName = null)
    {
        if (!File.Exists(path.IsValidPath(parameterName)))
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, PathDoesNotExistMessageFormat, path));
        }
#pragma warning disable CS8603 // Possible null reference return - false positive

        return path;
#pragma warning restore CS8603
    }

#endif
}
