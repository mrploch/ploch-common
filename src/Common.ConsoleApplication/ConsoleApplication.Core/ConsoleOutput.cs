using System;
using System.IO;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class ConsoleOutput : IOutput
    {
        private readonly TextWriter _writer;
        private readonly TextWriter _errorWriter;

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
            return WriteOutput(_writer, content + Environment.NewLine, args);
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
            return WriteOutput(_writer, content, args);
        }

        /// <inheritdoc />
        public IOutput WriteErrorLine<TContent>(TContent content, params object[] args)
        {
            return WriteOutput(_errorWriter, content + Environment.NewLine, args);
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
            return WriteOutput(_writer, content + Environment.NewLine, args);
        }

        private IOutput WriteOutput(TextWriter writer, object? contents, params object[]? args)
        {
            string strContents = contents == null ? "<null>" : contents.ToString();
            
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

    }
}