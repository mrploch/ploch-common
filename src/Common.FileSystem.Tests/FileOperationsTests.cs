using System.IO;
using Xunit;
using System.IO.Abstractions;

namespace Ploch.Common.FileSystem.Tests
{
    public class FileOperationsTests
    {
        [Fact]
        public void CopyDirectory_should_create_subfolders_for_target_hierarchy()
        {
            //TestCop
            var fileSystem = new System.IO.Abstractions.FileSystem();
            var sut = new FileOperations(fileSystem);

            var tempPath = Path.GetTempPath();
            var sourceDirectory = Directory.CreateDirectory(Path.Combine(tempPath,"source1"));

            sourceDirectory.CreateSubdirectory("source1-1");

            
        }

        
    }
}