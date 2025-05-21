using System;
using System.IO;
using System.Security.Cryptography;

namespace Ploch.Common.Cryptography;

/// <summary>
///     Provides hashing utility methods for strings.
/// </summary>
public static class Hashing
{
    /// <summary>
    ///     Converts the content of a stream into a hash string using the specified hash algorithm.
    /// </summary>
    /// <param name="stream">The stream whose content will be hashed.</param>
    /// <param name="algorithm">The hash algorithm to be used.</param>
    /// <returns>A string representation of the computed hash.</returns>
    public static string ToHashString(this Stream stream, HashAlgorithm algorithm)
    {
        var hashBytes = algorithm.ComputeHash(stream);

        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }

    /// <summary>
    ///     Converts the contents of a Stream into a MD5 hash string.
    /// </summary>
    /// <param name="stream">The input Stream to convert.</param>
    /// <returns>The MD5 hash string of the input Stream.</returns>
    public static string ToMD5HashString(this Stream stream)
    {
#pragma warning disable CA5351 // Do not use insecure cryptographic algorithm MD5 - it's not supposed to be secure here.
        var hashBytes = MD5.Create().ComputeHash(stream);
#pragma warning restore CA5351

        return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
    }
}