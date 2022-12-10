using System;
using System.Diagnostics;
using System.IO;

namespace Ploch.TestingSupport.TestData
{
    public class TestDataConfiguration
    {
        private string _basePath;

        public TestDataConfiguration()
        {
            SetupBasePathDataNotCopiedToOutput();
            //  SetTestDataFolder("TestData");
        }

        public string BasePath
        {
            get => _basePath;
            set
            {
                Debug.Assert(value != null, "value != null");
                _basePath = value.Trim('\\');
            }
        }

        public void SetupBasePathDataCopiedToOutput()
        {
            BasePath = Environment.CurrentDirectory;
        }

        public void SetupBasePathDataNotCopiedToOutput(string pathToProjectFiles = @"..\..\")
        {
            BasePath = pathToProjectFiles;
        }

        public void SetTestDataFolder(string folder)
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentException("Folder cannot be empty", "folder");
            }

            BasePath = Path.Combine(BasePath, folder);
        }
    }
}