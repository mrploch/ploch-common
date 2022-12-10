using System.IO;

namespace Ploch.Common.IO
{
    public static class StreamExtensions
    {
        public static byte[] ToBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.Position = 0;
                stream.CopyTo(ms);

                return ms.ToArray();
            }
        }
    }
}