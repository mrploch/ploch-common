using System.IO;
using Dawn;
using JetBrains.Annotations;

namespace Ploch.Common.IO
{
    public static class PathUtils
    {
        /// <summary>
        /// Gets the (short) name of the directory.
        /// </summary>
        /// <param name="directoryPath">Directory path.</param>
        public static string GetDirectoryName([NotNull] string directoryPath)
        {
            Guard.Argument(directoryPath, nameof(directoryPath)).NotNull().NotEmpty();

            return new DirectoryInfo(directoryPath).Name;
        }


    }

}