using System;
using System.IO;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class ConsoleOutput : IOutput
    {
        private readonly TextWriter _errorWriter;
        private readonly TextWriter _writer;

        public ConsoleOutput() : this(Console.Out, Console.Error)
        { }

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

        private IOutput WriteLine(TextWriter writer, object content, params object[] args)
        {
            var stringContents = GetStringContents(content);
            if (args?.Length > 0)
            {
                writer.WriteLine(stringContents, args);
            }
            else
            {
                writer.WriteLine(stringContents);
            }
            return this;
        }

        private IOutput Write(TextWriter writer, object? contents, params object[]? args)
        {
            string strContents = GetStringContents(contents);

            if (args != null && args.Length > 0)
            {
                writer.Write(strContents, args);
            }
            else
            {
                writer.Write(strContents);
            }

            return this;
        }

        private static string GetStringContents(object? contents)
        {
            return contents == null ? "<null>" : contents.ToString();
        }
    }
}