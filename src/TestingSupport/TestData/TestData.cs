using System.IO;

namespace Ploch.TestingSupport.TestData
{
    public static class TestData
    {
        public static TestDataConfiguration Configuration { get; } = new();

        public static Stream OpenStream(string location)
        {
            return GetFile(location).OpenRead();
        }

        public static StreamReader OpenText(string location)
        {
            return new StreamReader(OpenStream(location));
        }

        public static byte[] ReadBytes(string location)
        {
            return File.ReadAllBytes(GetPathToData(location));
        }

        public static string ReadText(string location)
        {
            return OpenText(location).ReadToEnd();
        }

        private static string GetPathToData(string location)
        {
            return Path.Combine(Configuration.BasePath, location);
        }

        private static FileInfo GetFile(string location)
        {
            var file = new FileInfo(GetPathToData(location));

            return file;
        }
    }
}