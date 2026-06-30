using System.Collections.Generic;
using System.IO;
using Ploch.Common.ArgumentChecking;

namespace Ploch.Common.IO;

/// <summary>
///     A static class providing extension methods for Stream objects.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    ///     Converts the contents of a stream to a byte array.
    /// </summary>
    /// <param name="stream">The stream to convert.</param>
    /// <returns>An enumerable byte array representing the contents of the stream.</returns>
    public static IEnumerable<byte> ToBytes(this Stream stream)
    {
        stream.NotNull(nameof(stream));

        using var ms = new MemoryStream();
        stream.Position = 0;
        stream.CopyTo(ms);

        return ms.ToArray();
    }
}
