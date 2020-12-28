using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace TestApps.ConsoleApp
{
    /// <summary>
    ///    Class Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>The factory.</value>
        public static ILoggerFactory Factory { get; set; }
        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>The logger.</value>
        public static ILogger Logger { get; set; }

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string logFileName = AppName();
            string currentLogDirValue = AppPath();
            Factory = new LoggerFactory().AddFile(logFileName, currentLogDirValue);
            Logger = Factory.CreateLogger($"{logFileName}_{nameof(ConsoleApp)}");
            Logger.LogInformation(message: $"Information; Created Log File for {logFileName}.",
                includeLineInfo: true);
            Logger.LogWarning(message: $"Warning; Creating Log File for {logFileName}.",
                includeLineInfo: true);
            Logger.LogTrace(message: $"Trace; Created Log File for {logFileName}.",
                includeLineInfo: true);
            Logger.LogDebug(message: $"Debug; Created Log File for {logFileName}.",
                includeLineInfo: true);
            try
            {
                throw new Exception("Example Exception.");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, message: $"Error; Creating Log File for {logFileName}.",
                    includeLineInfo: true,
                    includeStackTrace: true);
            }
        }

        /// <summary>
        /// Applications the path.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string AppPath() => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Applications the name.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string AppName() => Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);

    }
}
