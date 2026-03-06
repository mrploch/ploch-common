namespace Ploch.Common;

/// <summary>
///     Provides constants and utility methods for working with content sizes.
/// </summary>
public static class ContentSizes
{
    /// <summary>
    ///     Represents the size of a kilobyte in bytes (1024).
    /// </summary>
    public const int KiloByte = 1024;

    /// <summary>
    ///     Represents the size of a megabyte in bytes (1,048,576).
    /// </summary>
    public const int MegaByte = KiloByte * KiloByte;

    /// <summary>
    ///     Converts a size in kilobytes to bytes.
    /// </summary>
    /// <param name="kilobytes">The size in kilobytes to convert.</param>
    /// <returns>The equivalent size in bytes.</returns>
    public static long KilobytesToBytes(long kilobytes) => kilobytes * KiloByte;

    /// <summary>
    ///     Converts a size in megabytes to bytes.
    /// </summary>
    /// <param name="megabytes">The size in megabytes to convert.</param>
    /// <returns>The equivalent size in bytes.</returns>
    public static long MegabytesToBytes(long megabytes) => megabytes * MegaByte;
}
