using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;

namespace Ploch.TestingSupport.Configuration
{
    public class ConfigurationFileManager : IDisposable
    {
        private readonly string _targetPath;

        private readonly ICollection<string> _createdFilesPaths = new List<string>();

        public ConfigurationFileManager(string targetPath)
        {
            _targetPath = targetPath;
        }

        public void WriteFile(string fileName, byte[] fileContents)
        {
            var fullPath = Path.Combine(_targetPath, fileName);


            File.WriteAllBytes(fullPath, fileContents);

            _createdFilesPaths.Add(fullPath);
        }

        public void Dispose()
        {
            foreach (var filesPath in _createdFilesPaths)
            {
                File.Delete(filesPath);
            }
        }
    }
}