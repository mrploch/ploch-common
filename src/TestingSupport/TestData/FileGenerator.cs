using System;
using System.IO;
using System.Text;

namespace Ploch.TestingSupport.TestData
{
    public class FileGenerator
    {
        private readonly IContentGenerator _contentGenerator;
        private readonly IFileNamingConvention _fileNamingConvention;
        private readonly IFileNamingConvention _folderNamingConvention;
        private readonly string _targetFoler;

        public FileGenerator(IContentGenerator contentGenerator,
                             string targetFoler,
                             IFileNamingConvention fileNamingConvention,
                             IFileNamingConvention folderNamingConvention)
        {
            _contentGenerator = contentGenerator;
            _targetFoler = targetFoler;
            _fileNamingConvention = fileNamingConvention;
            _folderNamingConvention = folderNamingConvention;
        }

        public void Generate(int filesPerFolder, int folders)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < 100; i++)
            {
                sb.AppendLine(Guid.NewGuid().ToString());
            }

            if (!Directory.Exists(_targetFoler))
            {
                Directory.CreateDirectory(_targetFoler);
            }

            for (var folderNum = 1; folderNum <= folders; folderNum++)
            {
                var directoryName = _folderNamingConvention.GetName(folderNum);
                Directory.CreateDirectory(Path.Combine("test-files", directoryName));
                for (var fileNum = 1; fileNum <= filesPerFolder; fileNum++)
                {
                    var contents = _contentGenerator.Generate();
                    var path = Path.Combine(_targetFoler, directoryName, _fileNamingConvention.GetName(fileNum));
                    File.WriteAllBytes(path, contents);
                }
            }
        }
    }
}