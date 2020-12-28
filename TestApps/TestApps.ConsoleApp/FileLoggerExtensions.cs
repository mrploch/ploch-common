using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace TestApps.ConsoleApp
{
    /// <summary>
    ///    Class FileLoggerExtensions
    /// </summary>
    public static class FileLoggerExtensions
    {
        /// <summary>
        /// Adds the file.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="name">The name.</param>
        /// <param name="logFolder">The log folder.</param>
        /// <returns>ILoggerFactory.</returns>
        public static ILoggerFactory AddFile(this ILoggerFactory factory, string name, string logFolder)
        {
            if (factory is null || name is null || logFolder is null)
                return null;

            factory.AddProvider(provider: new FileLoggerProvider(name: name, logFolder: logFolder));
            return factory;
        }

        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The arguments.</param>
        public static void Log(this ILogger logger, LogLevel logLevel, string message, params object[] args) => logger.Log(logLevel: logLevel, eventId: 0, exception: null, message: message, args: args);

        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> [include stack trace].</param>
        /// <param name="args">The arguments.</param>
        public static void Log(this ILogger logger, LogLevel logLevel, Exception exception, string message, bool includeStackTrace = false, params object[] args)
        {
            if (includeStackTrace)
                message += $"{Environment.NewLine}Stack Trace: {exception.StackTrace}";
            logger.Log(logLevel: logLevel, eventId: 0, exception: exception, message: message, args: args);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeLineInfo">if set to <c>true</c> [include line information].</param>
        /// <param name="includeStackTrace">if set to <c>true</c> [include stack trace].</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        /// <param name="callerLineNumber">The caller line number.</param>
        public static void LogError(this ILogger logger, Exception exception,
        string message, bool includeLineInfo, bool includeStackTrace,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (callerMemberName is not null && includeLineInfo != false)
                message += $"{Environment.NewLine}Project Source: {exception.Source}{Environment.NewLine}Caller Member Name: {callerMemberName}{Environment.NewLine}Caller File Path: {callerFilePath}{Environment.NewLine}Caller Line Number: {callerLineNumber}";
            Log(logger: logger, logLevel: LogLevel.Error, exception: exception, message: message, includeStackTrace: includeStackTrace, args: null);
        }

        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeLineInfo">if set to <c>true</c> [include line information].</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        /// <param name="callerLineNumber">The caller line number.</param>
        public static void LogInformation(this ILogger logger,
        string message, bool includeLineInfo,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (callerMemberName is not null && includeLineInfo != false)
                message += $"{Environment.NewLine}Caller Member Name: {callerMemberName}{Environment.NewLine}Caller File Path: {callerFilePath}{Environment.NewLine}Caller Line Number: {callerLineNumber}";
            Log(logger: logger, logLevel: LogLevel.Information, message: message, args: null);
        }

        /// <summary>
        /// Logs the trace.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeLineInfo">if set to <c>true</c> [include line information].</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        /// <param name="callerLineNumber">The caller line number.</param>
        public static void LogTrace(this ILogger logger,
        string message, bool includeLineInfo,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (callerMemberName is not null && includeLineInfo != false)
                message += $"{Environment.NewLine}Caller Member Name: {callerMemberName}{Environment.NewLine}Caller File Path: {callerFilePath}{Environment.NewLine}Caller Line Number: {callerLineNumber}";
            Log(logger: logger, logLevel: LogLevel.Trace, message: message, args: null);
        }

        /// <summary>
        /// Logs the debug.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeLineInfo">if set to <c>true</c> [include line information].</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        /// <param name="callerLineNumber">The caller line number.</param>
        public static void LogDebug(this ILogger logger,
        string message, bool includeLineInfo,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (callerMemberName is not null && includeLineInfo != false)
                message += $"{Environment.NewLine}Caller Member Name: {callerMemberName}{Environment.NewLine}Caller File Path: {callerFilePath}{Environment.NewLine}Caller Line Number: {callerLineNumber}";
            Log(logger: logger, logLevel: LogLevel.Debug, message: message, args: null);
        }

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="includeLineInfo">if set to <c>true</c> [include line information].</param>
        /// <param name="callerMemberName">Name of the caller member.</param>
        /// <param name="callerFilePath">The caller file path.</param>
        /// <param name="callerLineNumber">The caller line number.</param>
        public static void LogWarning(this ILogger logger,
        string message, bool includeLineInfo,
        [CallerMemberName] string callerMemberName = "",
        [CallerFilePath] string callerFilePath = "",
        [CallerLineNumber] int callerLineNumber = 0)
        {
            if (callerMemberName is not null && includeLineInfo != false)
                message += $"{Environment.NewLine}Caller Member Name: {callerMemberName}{Environment.NewLine}Caller File Path: {callerFilePath}{Environment.NewLine}Caller Line Number: {callerLineNumber}";
            Log(logger: logger, logLevel: LogLevel.Warning, message: message, args: null);
        }
    }
}
