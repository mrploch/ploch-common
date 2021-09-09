using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Ploch.Common.Reflection;

namespace Ploch.Common.ConsoleApplication.Runner.Configuration
{
    public static class DefaultConfigurationExtensions
    {
        /// <summary>
        /// Configures the default configuration with JSON configuration file and environment variables.
        /// </summary>
        /// <remarks>
        /// <c>appsettings.json</c> file will be used if nothing is specified in the <paramref name="jsonFiles"/> parameter.
        /// If <paramref name="jsonFiles"/> is specified, then file names from this parameter will be used instead.
        /// </remarks>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="jsonFiles">The names of JSON files to use. If not specified, then <c>appsettings.json</c> will be used.</param>r
        /// <returns></returns>
        public static IConfigurationBuilder UseDefaultConfiguration(this IConfigurationBuilder builder, params string[]? jsonFiles)
        {
            return UseDefaultConfiguration(builder, null, jsonFiles);
        }

        /// <summary>
        /// Configures the default configuration with JSON configuration file and environment variables.
        /// </summary>
        /// <remarks>
        /// <c>appsettings.json</c> file will be used if nothing is specified in the <paramref name="jsonFiles"/> parameter.
        /// If <paramref name="jsonFiles"/> is specified, then file names from this parameter will be used instead.
        /// </remarks>
        /// <param name="builder">The configuration builder.</param>
        /// <param name="jsonFiles">The names of JSON files to use. If not specified, then <c>appsettings.json</c> will be used.</param>r
        /// <returns></returns>
        public static IConfigurationBuilder UseDefaultConfiguration(this IConfigurationBuilder builder, string? basePath, params string[]? jsonFiles)
        {
            if (jsonFiles == null || jsonFiles.Length == 0)
            {
                jsonFiles = new[] {"appsettings.json" };
            }

            if (basePath == null)
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                basePath = entryAssembly == null ? Directory.GetCurrentDirectory() : entryAssembly.GetAssemblyDirectory();
            }

            builder = builder.SetBasePath(basePath);
            foreach (var jsonFile in jsonFiles)
            {
                builder = builder.AddJsonFile(jsonFile);
            }

            builder = builder.AddEnvironmentVariables();

            return builder;
        }
    }
}