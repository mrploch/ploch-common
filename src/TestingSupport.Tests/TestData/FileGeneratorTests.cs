using Ploch.TestingSupport.TestData;
using Xunit;

namespace Ploch.TestingSupport.Tests.TestData
{
    public class FileGeneratorTests
    {
        [Fact]
        public void GenerateTestFiles()
        {
            var generator = new FileGenerator(new RandomTextContentGenerator(10, 100),
                                              "test-files",
                                              new NewGuidNamingConvention("file-", null, ".txt"),
                                              new DelegatedNamingConvention(folderNum => $"folder-{folderNum}"));

            generator.Generate(100, 100);

        }
    }
}