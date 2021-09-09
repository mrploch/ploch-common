using System.IO;
using System.Text;

namespace Ploch.Common.ConsoleApplication.Core
{
    public class TextWriterOutputAdapter : TextWriter
    {
        private readonly IOutput _output;

        public TextWriterOutputAdapter(IOutput output) : this(output, Encoding.UTF8)
        { }

        public TextWriterOutputAdapter(IOutput output, Encoding encoding)
        {
            _output = output;
            Encoding = encoding;
        }

        /// <inheritdoc />
        public override Encoding Encoding { get; }

        /// <inheritdoc />
        public override void Write(char value)
        {
            _output.Write(value);
        }
    }
}