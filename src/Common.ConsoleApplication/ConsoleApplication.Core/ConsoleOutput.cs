using System;
using System.IO;

namespace Ploch.Common.ConsoleApplication.Core
{
    public class ConsoleOutput : IOutput
    {
        private readonly TextWriter _errorWriter;
        private readonly TextWriter _writer;

        /// <summary>
        ///     Creates a new instance of <c>ConsoleOutput</c> using default <see cref="Console.Out" /> and <see cref="Console.Error" /> writers.
        /// </summary>
        public ConsoleOutput() : this(Console.Out, Console.Error)
        { }

        /// <summary>
        ///     Creates a new instance of <c>ConsoleOutput</c> using specified <c>TextWriter</c> instances.
        /// </summary>
        /// <param name="writer"><c>TextWriter</c> for standard output</param>
        /// <param name="errorWriter"><c>TextWriter</c> for error ouput</param>
        public ConsoleOutput(TextWriter writer, TextWriter errorWriter)
        {
            _writer = writer;
            _errorWriter = errorWriter;
        }

        /// <inheritdoc />
        public IOutput WriteLine<TContent>(TContent content, params object[] args)
        {
            return WriteLine(_writer, content, args);
        }

        /// <inheritdoc />
        public IOutput WriteLine()
        {
            Console.WriteLine();
            return this;
        }

        /// <inheritdoc />
        public IOutput Write<TContent>(TContent content, params object[] args)
        {
            return Write(_writer, content, args);
        }

        /// <inheritdoc />
        public IOutput WriteErrorLine<TContent>(TContent content, params object[] args)
        {
            return Write(_errorWriter, content + Environment.NewLine, args);
        }

        /// <inheritdoc />
        public IOutput WriteErrorLine()
        {
            Console.Error.WriteLine();
            return this;
        }

        /// <inheritdoc />
        public IOutput WriteError<TContent>(TContent content, params object[] args)
        {
            return Write(_writer, content + Environment.NewLine, args);
        }

        private IOutput WriteLine(TextWriter writer, object? content, params object[] args)
        {
            var stringContents = GetStringContents(content, args);
            writer.WriteLine(stringContents);

            return this;
        }

        private IOutput Write(TextWriter writer, object? contents, params object[]? args)
        {
            string strContents = GetStringContents(contents, args);
            writer.Write(strContents);
            return this;
        }

        private static string GetStringContents(object? contents, params object[]? args)
        {
            string strContents = contents == null ? "<null>" : contents.ToString();
            if (args != null)
            {
                return string.Format(strContents, args);
            }

            return strContents;
        }
    }
}