using System.Collections.Generic;
using System.IO.Abstractions;

namespace TestingSupport.FluentAssertions.IOAbstractions
{
    public static class FileSystemInfoEnumerableExtensions
    {
        public static FileSystemInfoAssertions<IFileSystemInfo> Should(this IEnumerable<IFileSystemInfo> fileSystemInfos)
        {
            return new(fileSystemInfos);
        }
        
        public static FileSystemInfoAssertions<IDirectoryInfo> Should(this IEnumerable<IDirectoryInfo> directoryInfos)
        {
            return new(directoryInfos);
        }
        
        public static FileSystemInfoAssertions<IFileInfo> Should(this IEnumerable<IFileInfo> fileInfos)
        {
            return new(fileInfos);
        }
    }
}