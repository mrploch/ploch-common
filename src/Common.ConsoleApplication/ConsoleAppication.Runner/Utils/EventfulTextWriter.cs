using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ploch.Common.ConsoleApplication.Runner.Utils
{
    public enum WriteOperationType
    {
        Write,
        WriteLine,
        WriteAsync,
        WriteLineAsync
    }

    public class TextWriterEventArgs : EventArgs
    {
        /// <inheritdoc />
        public TextWriterEventArgs(WriteOperationType operationType, object? value, object[]? args = null)
        {
            OperationType = operationType;
            Value = value;
            Args = args;
        }

        public WriteOperationType OperationType { get; }
        public object? Value { get; }

        public object[]? Args { get; }
    }

    public class EventfulTextWriter : TextWriter
    {
        public override Encoding Encoding { get; } = Encoding.UTF8;

        public event EventHandler<TextWriterEventArgs>? WriteExecuted;

        /// <inheritdoc />
        public override void Write(string format, params object[]? arg)
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.Write, format, arg));
        }

        public override void Write(char value)
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.Write, value));
        }
        public override void Write(string? value)
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.Write, value));
        }

        /// <inheritdoc />
        public override void WriteLine(string format, params object[]? arg)
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.Write, format, arg));
        }

        public override void WriteLine()
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.WriteLine, null));
        }

        public override void WriteLine(string? value)
        {
            if (value == null)
                WriteLine();
            else
                OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.WriteLine, value));
        }

        public override Task WriteAsync(char value)
        {
            Write(value);
            return Task.CompletedTask;
        }

        public override Task WriteAsync(string value)
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.WriteAsync, value));
            return Task.CompletedTask;
        }

        public override Task WriteLineAsync(string value)
        {
            OnWriteExecuted(new TextWriterEventArgs(WriteOperationType.WriteLineAsync, value));
            return Task.CompletedTask;
        }


        /// <summary>
        /// Event executed when write operation occurs in a writer.
        /// </summary>
        /// <param name="e"></param>
        /// <exception cref="T:System.Exception">A delegate callback throws an exception.</exception>
        protected virtual void OnWriteExecuted(TextWriterEventArgs e)
        {
            WriteExecuted?.Invoke(this, e);
        }
    }
}