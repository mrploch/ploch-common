using System.Collections.Generic;
using System.IO.Abstractions;

namespace TestingSupport.FluentAssertions.IOAbstractions;

/// <summary>
///     Provides extension methods for collections of file system information objects to enable fluent assertions.
/// </summary>
public static class FileSystemInfoEnumerableExtensions
{
    /// <summary>
    ///     Returns a <see cref="FileSystemInfoAssertions{T}" /> object that can be used to assert the
    ///     state of the given collection of file system information objects.
    /// </summary>
    /// <param name="fileSystemInfos">The collection of file system information objects to verify.</param>
    /// <returns>A <see cref="FileSystemInfoAssertions{T}" /> object for assertion chaining.</returns>
    public static FileSystemInfoAssertions<IFileSystemInfo> Should(this IEnumerable<IFileSystemInfo> fileSystemInfos) => new(fileSystemInfos);

    /// <summary>
    ///     Returns a <see cref="FileSystemInfoAssertions{T}" /> object that can be used to assert the
    ///     state of the given collection of directory information objects.
    /// </summary>
    /// <param name="directoryInfos">The collection of directory information objects to verify.</param>
    /// <returns>A <see cref="FileSystemInfoAssertions{T}" /> object for assertion chaining.</returns>
    public static FileSystemInfoAssertions<IDirectoryInfo> Should(this IEnumerable<IDirectoryInfo> directoryInfos) => new(directoryInfos);

    /// <summary>
    ///     Returns a <see cref="FileSystemInfoAssertions{T}" /> object that can be used to assert the
    ///     state of the given collection of file information objects.
    /// </summary>
    /// <param name="fileInfos">The collection of file information objects to verify.</param>
    /// <returns>A <see cref="FileSystemInfoAssertions{T}" /> object for assertion chaining.</returns>
    public static FileSystemInfoAssertions<IFileInfo> Should(this IEnumerable<IFileInfo> fileInfos) => new(fileInfos);
}
