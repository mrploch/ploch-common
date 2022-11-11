using System.IO;
using System.Reflection;
using Dawn;
using JetBrains.Annotations;

namespace Ploch.Common.Reflection
{
    /// <summary>
    ///     Extension utility methods for an <see cref="Assembly" />.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        ///     Gets the directory name where assembly is located
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <returns>The directory name where assembly is located.</returns>
        [NotNull]
        public static string GetAssemblyDirectory([NotNull] this Assembly assembly)
        {
            Guard.Argument(assembly, nameof(assembly)).NotNull();

            var agentAssemblyPath = assembly.Location;
            return Path.GetDirectoryName(agentAssemblyPath);
        }
    }
}