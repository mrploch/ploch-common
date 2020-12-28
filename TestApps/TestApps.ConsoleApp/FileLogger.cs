using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace TestApps.ConsoleApp
{
    /// <summary>
    ///    Class FileLogger
    /// </summary>
    internal sealed class FileLogger : ILogger, IDisposable
    {
        /// <summary>
        /// The category
        /// </summary>
        private readonly string _category;
        /// <summary>
        /// The writer
        /// </summary>
        private volatile StreamWriter _writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public FileLogger()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="writer">The writer.</param>
        public FileLogger(string category, StreamWriter writer)
        {
            _category = category;
            _writer = writer;
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An <see cref="T:System.IDisposable" /> that ends the logical operation scope on dispose.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();

        /// <summary>
        /// Begins the scope.
        /// </summary>
        /// <typeparam name="TState">The type of the t state.</typeparam>
        /// <param name="state">The state.</param>
        /// <returns>IDisposable.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDisposable BeginScope<TState>(object state) => throw new NotImplementedException();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => _writer = null;

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Logs the specified log level.
        /// </summary>
        /// <typeparam name="TState">The type of the t state.</typeparam>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="formatter">The formatter.</param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            StreamWriter writer = _writer;
            if (writer == null)
                return;
            string message = formatter(arg1: state, arg2: exception);
            lock (writer)
            {
                writer.WriteLine(value: $"[{DateTime.Now}] <{_category}> ({logLevel.ToString()[0]}):");
                writer.WriteLine(value: message);
                if (exception != null)
                    writer.WriteLine(value: $"Exception: {exception}");
                writer.WriteLine();
                writer.Flush();
            }
        }
    }
}
