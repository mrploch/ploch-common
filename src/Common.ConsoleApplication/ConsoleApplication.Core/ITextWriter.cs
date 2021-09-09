using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ploch.Common.Output
{
    public interface ITextWriter
    {
        IFormatProvider FormatProvider { get; }
        Encoding Encoding { get; }

        /// <summary>
        ///     Returns the line terminator string used by this TextWriter. The default line
        ///     terminator string is Environment.NewLine, which is platform specific.
        ///     On Windows this is a carriage return followed by a line feed ("\r\n").
        ///     On OSX and Linux this is a line feed ("\n").
        /// </summary>
        /// <remarks>
        ///     The line terminator string is written to the text stream whenever one of the
        ///     WriteLine methods are called. In order for text written by
        ///     the TextWriter to be readable by a TextReader, only one of the following line
        ///     terminator strings should be used: "\r", "\n", or "\r\n".
        /// </remarks>
        string NewLine { get; set; }

        void Close();
        void Dispose();
        ValueTask DisposeAsync();
        void Flush();
        void Write(char value);
        void Write(char[]? buffer);
        void Write(char[] buffer, int index, int count);
        void Write(ReadOnlySpan<char> buffer);
        void Write(bool value);
        void Write(int value);
        void Write(uint value);
        void Write(long value);
        void Write(ulong value);
        void Write(float value);
        void Write(double value);
        void Write(decimal value);
        void Write(string? value);
        void Write(object? value);

        /// <summary>
        ///     Equivalent to Write(stringBuilder.ToString()) however it uses the
        ///     StringBuilder.GetChunks() method to avoid creating the intermediate string
        /// </summary>
        /// <param name="value">The string (as a StringBuilder) to write to the stream</param>
        void Write(StringBuilder? value);

        void Write(string format, object? arg0);
        void Write(string format, object? arg0, object? arg1);
        void Write(string format, object? arg0, object? arg1, object? arg2);
        void Write(string format, params object?[] arg);
        void WriteLine();
        void WriteLine(char value);
        void WriteLine(char[]? buffer);
        void WriteLine(char[] buffer, int index, int count);
        void WriteLine(ReadOnlySpan<char> buffer);
        void WriteLine(bool value);
        void WriteLine(int value);
        void WriteLine(uint value);
        void WriteLine(long value);
        void WriteLine(ulong value);
        void WriteLine(float value);
        void WriteLine(double value);
        void WriteLine(decimal value);
        void WriteLine(string? value);

        /// <summary>
        ///     Equivalent to WriteLine(stringBuilder.ToString()) however it uses the
        ///     StringBuilder.GetChunks() method to avoid creating the intermediate string
        /// </summary>
        void WriteLine(StringBuilder? value);

        void WriteLine(object? value);
        void WriteLine(string format, object? arg0);
        void WriteLine(string format, object? arg0, object? arg1);
        void WriteLine(string format, object? arg0, object? arg1, object? arg2);
        void WriteLine(string format, params object?[] arg);
        Task WriteAsync(char value);
        Task WriteAsync(string? value);

        /// <summary>
        ///     Equivalent to WriteAsync(stringBuilder.ToString()) however it uses the
        ///     StringBuilder.GetChunks() method to avoid creating the intermediate string
        /// </summary>
        /// <param name="value">The string (as a StringBuilder) to write to the stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task WriteAsync(StringBuilder? value, CancellationToken cancellationToken = default);

        Task WriteAsync(char[]? buffer);
        Task WriteAsync(char[] buffer, int index, int count);
        Task WriteAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default);
        Task WriteLineAsync(char value);
        Task WriteLineAsync(string? value);

        /// <summary>
        ///     Equivalent to WriteLineAsync(stringBuilder.ToString()) however it uses the
        ///     StringBuilder.GetChunks() method to avoid creating the intermediate string
        /// </summary>
        /// <param name="value">The string (as a StringBuilder) to write to the stream</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        Task WriteLineAsync(StringBuilder? value, CancellationToken cancellationToken = default);

        Task WriteLineAsync(char[]? buffer);
        Task WriteLineAsync(char[] buffer, int index, int count);
        Task WriteLineAsync(ReadOnlyMemory<char> buffer, CancellationToken cancellationToken = default);
        Task WriteLineAsync();
        Task FlushAsync();
    }
}