using System.IO;
using Dawn;

namespace Ploch.Common.IO
{
    public static class PathUtils
    {
        /// <summary>
        ///     Gets the (short) name of the directory.
        /// </summary>
        /// <param name="directoryPath">Directory path.</param>
        /// <returns>The name of the directory.</returns>
        public static string GetDirectoryName(string directoryPath)
        {
            Guard.Argument(directoryPath, nameof(directoryPath)).NotNull().NotEmpty();

            return new DirectoryInfo(directoryPath).Name;
        }
    }
}