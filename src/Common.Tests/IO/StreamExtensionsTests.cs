using System.IO;
using FluentAssertions;
using Ploch.Common.IO;
using Xunit;

namespace Ploch.Common.Tests.IO
{
    public class StreamExtensionsTests
    {
        [Fact]
        public void ToBytes_should_read_entire_stream_to_bytes()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[]{1, 2, 3, 4, 5});
            
            stream.Position = 0;
            var bytes = stream.ToBytes();

            bytes.Should().BeEquivalentTo(new byte[] { 1, 2, 3, 4, 5 });
        }
    }
}