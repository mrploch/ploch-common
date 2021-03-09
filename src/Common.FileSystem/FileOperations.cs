using System.IO;
using System.IO.Abstractions;

namespace Ploch.Common.FileSystem
{
    public class FileOperations
    {
        private readonly IFileSystem _fileSystem;

        public FileOperations(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }


        public void CopyDirectory(string source, string target, bool recursive)
        {
            IDirectoryInfo dir = _fileSystem.DirectoryInfo.FromDirectoryName(source);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + source);
            }

            
            // If the destination directory doesn't exist, create it.
            
            if (!_fileSystem.Directory.Exists(target))
            {
                _fileSystem.Directory.CreateDirectory(target);
            }

            // Get the files in the directory and copy them to the new location.
            foreach (var file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(target, file.Name);
                file.CopyTo(targetFilePath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (recursive)
            {
                foreach (IDirectoryInfo subDir in dir.GetDirectories())
                {
                    string subTarget = Path.Combine(target, subDir.Name);
                    CopyDirectory(subDir.FullName, subTarget, true);
                }
            }
        }
    }
}